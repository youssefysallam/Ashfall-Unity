using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public float rotationSpeed = 10f;
    public float sprintMultiplier = 1.8f;

    [Header("Camera Reference")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;

    [Header("Air Control")]
    public float airControl = 0.25f;     // 0.0 = no steering, 1.0 = full steering
    public float groundAccel = 20f;      // how fast you reach target speed on ground
    public float airAccel = 6f;          // how fast you steer in air

    [Header("Bunny Hop / Jump Assist")]
    public float coyoteTime = 0.12f;        // jump shortly after leaving ground
    public float jumpBufferTime = 0.12f;    // press jump slightly early
    public float landingFrictionDelay = 0.08f; // keep momentum briefly on landing
    public float groundFriction = 18f;      // how quickly you slow down when no input

    [Header("Slide (LCTRL)")]
    public float slideSpeed = 12f;
    public float slideDuration = 0.55f;
    public float slideCooldown = 0.6f;
    public float slideControllerHeight = 1.0f;

    private Vector3 horizVel;
    private float verticalVel;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private float landingFrictionTimer;

    private bool isSliding;
    private float slideTimer;
    private float slideCooldownTimer;
    private float defaultControllerHeight;
    private Vector3 slideDirection;
    public Animator animator;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        defaultControllerHeight = controller.height;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // --- Grounding / timers ---
        bool grounded = controller.isGrounded;

        if (grounded)
        {
            coyoteTimer = coyoteTime;

            // small grace period before friction kicks in (bunny-hop feel)
            landingFrictionTimer = Mathf.Max(landingFrictionTimer - dt, 0f);
        }
        else
        {
            coyoteTimer -= dt;
        }

        jumpBufferTimer -= dt;
        if (Input.GetButtonDown("Jump"))
            jumpBufferTimer = jumpBufferTime;

        // --- Input direction relative to camera ---
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraTransform.forward; forward.y = 0f; forward.Normalize();
        Vector3 right   = cameraTransform.right;   right.y = 0f; right.Normalize();

        Vector3 moveDir = (forward * v + right * h);
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        bool sprinting = Input.GetKey(KeyCode.LeftShift);
        float speed = moveSpeed * (sprinting ? sprintMultiplier : 1f);

        // --- Slide start (LCTRL) ---
        if (!isSliding && slideCooldownTimer <= 0f && grounded && sprinting && Input.GetKeyDown(KeyCode.LeftControl))
        {
            isSliding = true;
            slideTimer = slideDuration;
            slideCooldownTimer = slideCooldown;

            slideDirection = (horizVel.sqrMagnitude > 0.1f) ? horizVel.normalized : forward;

            controller.height = slideControllerHeight;
            // optional: lower center so you don't float
            controller.center = new Vector3(controller.center.x, slideControllerHeight * 0.5f, controller.center.z);
        }

        slideCooldownTimer -= dt;

        // --- Desired horizontal velocity ---
        Vector3 desiredHoriz = Vector3.zero;

        if (isSliding)
        {
            desiredHoriz = slideDirection * slideSpeed;
            slideTimer -= dt;

            if (slideTimer <= 0f)
            {
                isSliding = false;
                controller.height = defaultControllerHeight;
                controller.center = new Vector3(controller.center.x, defaultControllerHeight * 0.5f, controller.center.z);

                // prevent immediate harsh friction on slide end
                landingFrictionTimer = landingFrictionDelay;
            }
        }
        else
        {
            desiredHoriz = moveDir * speed;
        }

        // --- Acceleration + air control ---
        float accel = grounded ? groundAccel : airAccel;
        float control = grounded ? 1f : airControl;

        // if airborne, blend toward desired slowly (momentum)
        Vector3 blendedDesired = Vector3.Lerp(horizVel, desiredHoriz, control);
        horizVel = Vector3.MoveTowards(horizVel, blendedDesired, accel * dt);

        // --- Friction when no input (only when NOT sliding) ---
        bool hasInput = moveDir.sqrMagnitude > 0.001f;
        if (grounded && !isSliding && !hasInput && landingFrictionTimer <= 0f)
        {
            horizVel = Vector3.MoveTowards(horizVel, Vector3.zero, groundFriction * dt);
        }

        // --- Jump (buffer + coyote) ---
        if (!isSliding && jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            verticalVel = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        // --- Gravity ---
        if (grounded && verticalVel < 0f)
            verticalVel = -2f; // stick to ground

        verticalVel += gravity * dt;

        // --- Apply movement ---
        Vector3 velocity = horizVel + Vector3.up * verticalVel;
        controller.Move(velocity * dt);

        // --- Rotate character toward movement (not sliding) ---
        if (!isSliding && horizVel.sqrMagnitude > 0.05f)
        {
            Quaternion targetRot = Quaternion.LookRotation(horizVel.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * dt);
        }

        if (animator != null)
        {
            float planarSpeed = new Vector3(horizVel.x, 0f, horizVel.z).magnitude;
            float maxSpeed = moveSpeed * sprintMultiplier;

            animator.SetFloat("Speed", Mathf.Clamp01(planarSpeed / maxSpeed));
            animator.SetBool("IsGrounded", controller.isGrounded);
        }
    }
}
