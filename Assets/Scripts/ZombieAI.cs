using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("Chase")]
    public float detectRange = 30f;
    public float stopDistance = 1.8f;
    public float turnSpeed = 8f;

    [Header("Attack")]
    public float attackRange = 2.0f;
    public int attackDamage = 10;
    public float attackCooldown = 1.0f;

    Transform player;
    PlayerStats playerStats;
    NavMeshAgent agent;

    float attackTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stopDistance;
    }

    void Start()
    {
        // Assumes your player root is tagged Player or named PlayerRoot
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerStats = p.GetComponent<PlayerStats>();
        }
    }

    void Update()
    {
        if (player == null) return;

        float d = Vector3.Distance(transform.position, player.position);

        if (d <= detectRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            // face movement direction a bit nicer
            if (agent.velocity.sqrMagnitude > 0.1f)
            {
                Vector3 dir = agent.velocity.normalized;
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            }

            attackTimer -= Time.deltaTime;

            if (d <= attackRange && attackTimer <= 0f)
            {
                attackTimer = attackCooldown;

                if (playerStats != null)
                    playerStats.TakeDamage(attackDamage);
            }
        }
        else
        {
            agent.isStopped = true;
        }
    }
}
