using System.Collections.Generic;
using UnityEngine;

public enum PickupType { Food, Oxygen }

public class PickupSpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Randomly chosen for each Food spawn point.")]
    public List<GameObject> foodPrefabs = new();

    [Tooltip("Randomly chosen for each Oxygen spawn point.")]
    public List<GameObject> oxygenPrefabs = new();

    [Header("Spawn Parents")]
    public Transform foodSpawnsParent;     // WORLD/Pickups/FoodSpawns
    public Transform oxygenSpawnsParent;   // WORLD/Pickups/OxygenSpawns

    // Scarcity: once collected, it's gone for the run.
    private readonly HashSet<string> consumedSpawnIds = new();

    private void Start()
    {
        SpawnAllOnce();
    }

    public void SpawnAllOnce()
    {
        SpawnFromParent(foodSpawnsParent, PickupType.Food, foodPrefabs);
        SpawnFromParent(oxygenSpawnsParent, PickupType.Oxygen, oxygenPrefabs);
    }

    private void SpawnFromParent(Transform parent, PickupType type, List<GameObject> prefabs)
    {
        if (parent == null) { Debug.LogError($"[{nameof(PickupSpawnManager)}] Missing parent for {type} spawns."); return; }
        if (prefabs == null || prefabs.Count == 0) { Debug.LogError($"[{nameof(PickupSpawnManager)}] No prefabs set for {type}."); return; }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform spawn = parent.GetChild(i);
            string spawnId = MakeSpawnId(spawn, type);

            if (consumedSpawnIds.Contains(spawnId))
                continue; // already collected this run

            GameObject chosen = prefabs[Random.Range(0, prefabs.Count)];
            GameObject instance = Instantiate(chosen, spawn.position, spawn.rotation, spawn);

            // Tell the pickup what it is + where it came from, so it can report back.
            var pickup = instance.GetComponent<Pickup>();
            if (pickup == null)
            {
                Debug.LogError($"Prefab '{chosen.name}' is missing Pickup.cs. Add Pickup.cs to the prefab.");
                continue;
            }

            pickup.spawnManager = this;
            pickup.spawnId = spawnId;
            pickup.type = (type == PickupType.Food)
                ? Pickuptype.Food
                : Pickuptype.Oxygen;
        }
    }

    // Called by Pickup.cs when collected
    public void MarkConsumed(string spawnId)
    {
        consumedSpawnIds.Add(spawnId);
    }

    private string MakeSpawnId(Transform spawn, PickupType type)
    {
        // Stable ID per spawn point within this scene hierarchy.
        // Example: Food:/WORLD/Pickups/FoodSpawns/Food_01
        return $"{type}:{GetPath(spawn)}";
    }

    private string GetPath(Transform t)
    {
        var parts = new List<string>();
        while (t != null)
        {
            parts.Add(t.name);
            t = t.parent;
        }
        parts.Reverse();
        return "/" + string.Join("/", parts);
    }
}
