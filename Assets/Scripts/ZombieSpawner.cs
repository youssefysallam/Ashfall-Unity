using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject zombiePrefab;
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Spawn Rules")]
    public int maxAlive = 20;
    public float spawnInterval = 2f;
    public float minDistanceFromPlayer = 15f;

    private readonly List<GameObject> alive = new();
    private float timer;
    private Transform player;

    void Awake()
    {
        // Auto-fill spawn points from children if none assigned
        if (spawnPoints == null) spawnPoints = new List<Transform>();
        if (spawnPoints.Count == 0)
        {
            foreach (Transform child in transform)
                spawnPoints.Add(child);
        }

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (zombiePrefab == null) return;

        // If there are ANY compile/runtime errors, you’ll see them in Console
        // but this keeps the list clean.
        alive.RemoveAll(z => z == null);

        if (spawnPoints == null || spawnPoints.Count == 0) return;
        if (alive.Count >= maxAlive) return;
        if (player == null) return;

        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer = 0f;

        TrySpawn();
    }

    void TrySpawn()
    {
        // Try a few random points
        for (int i = 0; i < 8; i++)
        {
            Transform sp = spawnPoints[Random.Range(0, spawnPoints.Count)];
            if (sp == null) continue;

            if (Vector3.Distance(sp.position, player.position) < minDistanceFromPlayer)
                continue;

            // Snap to navmesh so agent can move
            Vector3 pos = sp.position;
            if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                pos = hit.position;

            GameObject z = Instantiate(zombiePrefab, pos, Quaternion.identity);
            alive.Add(z);
            return;
        }
    }
}
