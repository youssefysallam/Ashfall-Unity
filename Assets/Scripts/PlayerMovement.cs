using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public PlayerFootsteps footsteps;
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public float rotationSpeed = 10f;
    public float sprintMultiplier = 1.8f;

    [Header("Camera Reference")]
    public Transform cameraTransform;

    [Header("Air Control")]
    public float airControl = 0.25f;
    public float groundAccel = 20f;
    public float airAccel = 6f;

    [Header("Ground Friction")]
    public float groundFriction = 18f;

    [Header("Fall Damage")]
    [SerializeField] private bool enableFallDamage = true;
    [SerializeField] private float safeImpactSpeed = 10f;
    [SerializeField] private float damagePerSpeed = 6f;
    [SerializeField] private float maxFallDamage = 95f;
    [SerializeField] private float minFallHeight = 2.5f;

    public Animator animator;

    private CharacterController controller;

    private Vector3 horizVel;
    private float verticalVel;

    private bool wasGrounded;
    private float mostNegativeYVel;
    private float fallStartY;

    private void TryApplyDamage(float amount)
    {
        SendMessage("TakeDamage", amount, SendMessageOptions.DontRequireReceiver);
    }

    private void OnLanded()
    {
        if (!enableFallDamage) return;

        float fallHeight = fallStartY - transform.position.y;
        float impactSpeed = -mostNegativeYVel;

        if (fallHeight < minFallHeight) return;
        if (impactSpeed <= safeImpactSpeed) return;

        float damage = (impactSpeed - safeImpactSpeed) * damagePerSpeed;
        damage = Mathf.Clamp(damage, 0f, maxFallDamage);

        TryApplyDamage(damage);
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (footsteps == null)
            footsteps = GetComponent<PlayerFootsteps>();
    }

    void Update()
    {
        float dt = Time.deltaTime;
        bool grounded = controller.isGrounded;

        // Fall tracking
        if (!grounded && wasGrounded)
        {
            fallStartY = transform.position.y;
            mostNegativeYVel = 0f;
        }

        if (!grounded)
        {
            mostNegativeYVel = Mathf.Min(mostNegativeYVel, verticalVel);
        }

        if (grounded && !wasGrounded)
        {
            OnLanded();
        }

        wasGrounded = grounded;

        // Input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * v + right * h;
        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        bool sprinting = Input.GetKey(KeyCode.LeftShift);
        float speed = moveSpeed * (sprinting ? sprintMultiplier : 1f);

        if (footsteps != null)
            footsteps.isRunning = sprinting && moveDir.sqrMagnitude > 0.01f;

        // Horizontal movement
        Vector3 desiredHoriz = moveDir * speed;
        float accel = grounded ? groundAccel : airAccel;
        float control = grounded ? 1f : airControl;

        Vector3 blendedDesired = Vector3.Lerp(horizVel, desiredHoriz, control);
        horizVel = Vector3.MoveTowards(horizVel, blendedDesired, accel * dt);

        // Friction
        if (grounded && moveDir.sqrMagnitude < 0.001f)
        {
            horizVel = Vector3.MoveTowards(horizVel, Vector3.zero, groundFriction * dt);
        }

        // Jump (GROUND ONLY)
        if (grounded && Input.GetButtonDown("Jump"))
        {
            verticalVel = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        if (grounded && verticalVel < 0f)
            verticalVel = -2f;

        verticalVel += gravity * dt;

        // Apply movement
        Vector3 velocity = horizVel + Vector3.up * verticalVel;
        controller.Move(velocity * dt);

        // Rotate character
        if (horizVel.sqrMagnitude > 0.05f)
        {
            Quaternion targetRot = Quaternion.LookRotation(horizVel.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * dt);
        }

        // Animator
        if (animator != null)
        {
            float planarSpeed = new Vector3(horizVel.x, 0f, horizVel.z).magnitude;
            float maxSpeed = moveSpeed * sprintMultiplier;

            animator.SetFloat("Speed", Mathf.Clamp01(planarSpeed / maxSpeed));
            animator.SetBool("IsGrounded", grounded);
        }
    }
}
