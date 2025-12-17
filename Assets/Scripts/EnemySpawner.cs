using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject zombiePrefab;

    [Header("Spawn Rules")]
    public int maxAliveFromThisSpawner = 10;
    public float spawnInterval = 3f;
    public float spawnRadius = 20f;
    public float minDistanceFromPlayer = 25f;

    private Transform player;
    private int alive;
    private float timer;

    void Start()
    {
        var stats = FindFirstObjectByType<PlayerStats>();
        player = stats != null ? stats.transform : null;
    }

    void Update()
    {
        if (player == null || zombiePrefab == null) return;

        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer = 0f;

        if (alive >= maxAliveFromThisSpawner) return;

        Vector3 p = RandomPointNearSpawner();
        if (Vector3.Distance(p, player.position) < minDistanceFromPlayer) return;

        var z = Instantiate(zombiePrefab, p, Quaternion.identity);
        alive++;

        Destroy(z, 120f);
    }

    Vector3 RandomPointNearSpawner()
    {
        Vector2 r = Random.insideUnitCircle * spawnRadius;
        return transform.position + new Vector3(r.x, 0f, r.y);
    }
}
