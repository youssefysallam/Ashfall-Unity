using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 4f;
    public float height = 2f;
    public float mouseSensitivity = 150f;

    private float yaw;
    private float pitch;

    public float minPitch = -20f;
    public float maxPitch = 60f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (target == null) return;

        //Mouse input
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch += Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //Clamp vert angle
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        //Build rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        //Desired cam postion
        Vector3 desiredPos = target.position + rotation * new Vector3(0, height, -distance);

        //Apply position & rotation
        transform.position = desiredPos;
        transform.LookAt(target.position + Vector3.up * height * 0.5f);
    }
}
