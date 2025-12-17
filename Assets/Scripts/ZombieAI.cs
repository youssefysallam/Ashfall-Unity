using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private Animator anim;

    [Header("Chase")]
    public float detectRange = 30f;
    public float stopDistance = 1.8f;
    public float turnSpeed = 8f;

    [Header("Attack")]
    public float attackRange = 2.0f;
    public int attackDamage = 10;
    public float attackCooldown = 1.0f;

    private Transform player;
    private PlayerStats playerStats;
    private NavMeshAgent agent;

    private float attackTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stopDistance;
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        AcquirePlayer();
    }

    void Update()
    {
        if (player == null || playerStats == null)
            AcquirePlayer();

        if (player == null)
        {
            agent.isStopped = true;
            return;
        }

        float d = Vector3.Distance(transform.position, player.position);

        if (d <= detectRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            if (d <= stopDistance)
            {
                Vector3 dir = (player.position - transform.position);
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
                }
            }

            if (anim != null)
            {
                float speed01 = agent.velocity.magnitude;
                anim.SetFloat("Speed", speed01);
            }

            attackTimer -= Time.deltaTime;

            float mult = GameManager.Instance != null ? GameManager.Instance.ZombieDamageMultiplier() : 1f;
            float dmg = attackDamage * mult;

            if (d <= attackRange && attackTimer <= 0f)
            {
                attackTimer = attackCooldown;

                if (playerStats != null)
                    playerStats.TakeDamage(dmg);
            }
        }
        else
        {
            agent.isStopped = true;
        }
    }

    private void AcquirePlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p == null)
        {
            GameObject byName = GameObject.Find("PlayerRoot");
            if (byName != null) p = byName;
        }

        if (p == null)
        {
            player = null;
            playerStats = null;
            return;
        }

        player = p.transform;

        playerStats = p.GetComponent<PlayerStats>();
        if (playerStats == null)
            playerStats = p.GetComponentInChildren<PlayerStats>(true);
        if (playerStats == null)
            playerStats = p.GetComponentInParent<PlayerStats>();
        if (playerStats == null)
            playerStats = Object.FindFirstObjectByType<PlayerStats>();
    }
}

