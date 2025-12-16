using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    [Header("Distance / Framing (GTA-ish)")]
    public float distance = 2.4f;
    public float height = 1.6f;
    public Vector3 shoulderOffset = new Vector3(0.6f, 0f, 0f); // right shoulder

    [Header("Look")]
    public float mouseSensitivity = 150f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    [Header("Smoothing")]
    public float followSmooth = 12f;

    float yaw;
    float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        // Mouse up -> look up, mouse down -> look down
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 desiredPos =
            target.position + rotation * (new Vector3(0, height, -distance) + shoulderOffset);

        transform.position = Vector3.Lerp(transform.position, desiredPos, followSmooth * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * (height * 0.5f));
    }
}
