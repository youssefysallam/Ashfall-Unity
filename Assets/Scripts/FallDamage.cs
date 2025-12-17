using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FallDamage : MonoBehaviour
{
    [Header("Tuning")]
    public float minImpactSpeed = 12f;      // below this = no damage
    public float maxImpactSpeed = 30f;      // at/above this = max damage
    public float maxDamage = 60f;           // damage at maxImpactSpeed

    private CharacterController cc;
    private PlayerStats stats;

    private bool wasGrounded;
    private float lowestYVelocity; // most negative (fastest falling)

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>(); // must be on same object
        wasGrounded = cc.isGrounded;
        lowestYVelocity = 0f;
    }

    void Update()
    {
        // Approx vertical velocity using position delta
        float yVel = (transform.position.y - _lastY) / Mathf.Max(Time.deltaTime, 0.0001f);
        _lastY = transform.position.y;

        bool grounded = cc.isGrounded;

        if (!grounded)
        {
            // Track fastest downward velocity
            if (yVel < lowestYVelocity) lowestYVelocity = yVel;
        }

        // Landing moment
        if (!wasGrounded && grounded)
        {
            float impactSpeed = -lowestYVelocity; // convert negative fall speed to positive impact

            if (impactSpeed > minImpactSpeed && stats != null)
            {
                float t = Mathf.InverseLerp(minImpactSpeed, maxImpactSpeed, impactSpeed);
                float dmg = Mathf.Lerp(0f, maxDamage, t);

                stats.TakeDamage(dmg);
            }

            lowestYVelocity = 0f;
        }

        wasGrounded = grounded;
    }

    private float _lastY;
}
