using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource source;

    [Header("Loop Clips")]
    public AudioClip walkLoop;
    public AudioClip runLoop;

    [Header("Detection")]
    public float minMoveSpeed = 0.1f;

    public CharacterController controller;
    public Rigidbody rb;

    public bool isRunning;

    void Reset()
    {
        source = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float speed = GetPlanarSpeed();
        bool moving = speed > minMoveSpeed && controller != null && controller.isGrounded;

        if (!moving)
        {
            StopLoop();
            return;
        }

        AudioClip target = isRunning ? runLoop : walkLoop;
        StartOrSwitchLoop(target);
    }

    float GetPlanarSpeed()
    {
        if (controller != null)
        {
            Vector3 v = controller.velocity;
            v.y = 0f;
            return v.magnitude;
        }

        if (rb != null)
        {
            Vector3 v = rb.linearVelocity;
            v.y = 0f;
            return v.magnitude;
        }

        return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).magnitude;
    }

    void StartOrSwitchLoop(AudioClip clip)
    {
        if (source == null || clip == null) return;

        if (source.clip == clip && source.isPlaying) return;

        source.loop = true;
        source.clip = clip;
        source.Play();
    }

    void StopLoop()
    {
        if (source == null) return;
        if (!source.isPlaying) return;

        source.Stop();
        source.clip = null;
        source.loop = false;
    }
}
