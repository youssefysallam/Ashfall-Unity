using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] stepClips;

    [Header("Timing (seconds)")]
    public float walkStepInterval = 0.55f;
    public float runStepInterval  = 0.35f;

    [Header("Detection")]
    public float minMoveSpeed = 0.1f;

    // If you have one of these, assign it (otherwise it uses transform movement)
    public CharacterController controller;
    public Rigidbody rb;

    public bool isRunning; // set this from your movement script (Shift, stamina, etc.)

    float nextStepTime;

    void Reset()
    {
        source = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float speed = GetSpeed();
        bool moving = speed > minMoveSpeed;

        if (!moving) return;

        float interval = isRunning ? runStepInterval : walkStepInterval;

        if (Time.time >= nextStepTime)
        {
            PlayStep();
            nextStepTime = Time.time + interval;
        }
    }

    float GetSpeed()
    {
        if (controller != null) return controller.velocity.magnitude;
        if (rb != null) return rb.linearVelocity.magnitude;
        // fallback: not perfect, but works
        return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).magnitude;
    }

    void PlayStep()
    {
        if (source == null || stepClips == null || stepClips.Length == 0) return;
        var clip = stepClips[Random.Range(0, stepClips.Length)];
        source.PlayOneShot(clip, 0.8f);
    }
}
