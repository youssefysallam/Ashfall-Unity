using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Spawn Rules")]
    public int maxAlive = 20;
    public float spawnInterval = 2f;
    public float minDistanceFromPlayer = 15f;

    Transform player;
    float timer;
    readonly List<GameObject> alive = new();

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        // Auto-collect child spawner points if list empty
        if (spawnPoints.Count == 0)
        {
            foreach (Transform child in transform)
                spawnPoints.Add(child);
        }
    }

    void Update()
    {
        CleanupDead();

        if (zombiePrefab == null || spawnPoints.Count == 0) return;
        if (alive.Count >= maxAlive) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = spawnInterval;

        Transform sp = PickSpawnPoint();
        if (sp == null) return;

        GameObject z = Instantiate(zombiePrefab, sp.position, sp.rotation);
        alive.Add(z);

        // hook kill stats
        var zh = z.GetComponent<ZombieHealth>();
        if (zh != null)
        {
            zh.OnDied += _ =>
            {
                // if you add GameManager kill tracking:
                // FindObjectOfType<GameManager>()?.RegisterZombieKill();
            };
        }
    }

    void CleanupDead()
    {
        for (int i = alive.Count - 1; i >= 0; i--)
            if (alive[i] == null) alive.RemoveAt(i);
    }

    Transform PickSpawnPoint()
    {
        if (player == null) return spawnPoints[Random.Range(0, spawnPoints.Count)];

        // try a few times to find one not near player
        for (int tries = 0; tries < 10; tries++)
        {
            Transform sp = spawnPoints[Random.Range(0, spawnPoints.Count)];
            if (Vector3.Distance(sp.position, player.position) >= minDistanceFromPlayer)
                return sp;
        }

        return null;
    }
}
