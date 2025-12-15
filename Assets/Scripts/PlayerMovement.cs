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

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (cameraTransform == null)
        {
            Debug.LogWarning("Player Movement: No cameraTrasnform assigned!");
            return;
        }

        float h = Input.GetAxis("Horizontal"); // A/D, Left/Right
        float v = Input.GetAxis("Vertical"); // W/S, Up/Down

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir = camForward * v + camRight * h;
        
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= sprintMultiplier;
        }

        if (moveDir.sqrMagnitude > 0.001f)
        {
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        //gravity
        if (controller.isGrounded && velocity.y < 0){ velocity.y = -2f;}
        if (Input.GetButtonDown("Jump") && controller.isGrounded) { velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);}

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
