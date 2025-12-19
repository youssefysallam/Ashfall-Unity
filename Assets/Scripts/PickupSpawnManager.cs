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
    public Transform foodSpawnsParent;     
    public Transform oxygenSpawnsParent;   

    [Tooltip("Randomly chosen for each Medkit spawn point.")]
    public List<GameObject> medkitPrefabs = new();

    [Tooltip("Weapon pickup prefab that has WeaponPickup.cs on it.")]
    public GameObject weaponPickupPrefab;

    [Tooltip("Randomly chosen WeaponStats for each gun spawn point.")]
    public List<WeaponStats> gunWeaponOptions = new();

    public Transform medkitSpawnsParent;  
    public Transform gunSpawnsParent;     

    public GameObject[] ammoPrefabs;
    public Transform ammoSpawnsParent;



    private readonly HashSet<string> consumedSpawnIds = new();

    private void Start()
    {
        SpawnAllOnce();
    }

    public void SpawnAllOnce()
    {
        SpawnFromParent(foodSpawnsParent, PickupType.Food, foodPrefabs);
        SpawnFromParent(oxygenSpawnsParent, PickupType.Oxygen, oxygenPrefabs);
        SpawnMedkitsOnce();
        SpawnGunsOnce();
        SpawnAmmoOnce();
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
                continue; 

            GameObject chosen = prefabs[Random.Range(0, prefabs.Count)];
            GameObject instance = Instantiate(chosen, spawn.position, spawn.rotation, spawn);

            
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

    public void MarkConsumed(string spawnId)
    {
        consumedSpawnIds.Add(spawnId);
    }

    private string MakeSpawnId(Transform spawn, PickupType type)
    {
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
    
    private void SpawnMedkitsOnce()
    {
        SpawnMedkitsFromParent(medkitSpawnsParent, medkitPrefabs);
    }

    private void SpawnGunsOnce()
    {
        SpawnGunsFromParent(gunSpawnsParent, weaponPickupPrefab, gunWeaponOptions);
    }

    private void SpawnMedkitsFromParent(Transform parent, List<GameObject> prefabs)
    {
        if (parent == null) { Debug.LogError($"[{nameof(PickupSpawnManager)}] Missing parent for Medkit spawns."); return; }
        if (prefabs == null || prefabs.Count == 0) { Debug.LogError($"[{nameof(PickupSpawnManager)}] No prefabs set for Medkits."); return; }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform spawn = parent.GetChild(i);
            string spawnId = $"Medkit:{GetPath(spawn)}";

            if (consumedSpawnIds.Contains(spawnId))
                continue;

            GameObject chosen = prefabs[Random.Range(0, prefabs.Count)];
            GameObject instance = Instantiate(chosen, spawn.position, spawn.rotation, spawn);

            var mk = instance.GetComponent<MedkitPickup>();
            if (mk == null)
            {
                Debug.LogError($"Prefab '{chosen.name}' is missing MedkitPickup.cs.");
                continue;
            }

            mk.spawnManager = this;
            mk.spawnId = spawnId;
        }
    }

    private void SpawnGunsFromParent(Transform parent, GameObject pickupPrefab, List<WeaponStats> options)
    {
        if (parent == null) { Debug.LogError($"[{nameof(PickupSpawnManager)}] Missing parent for Gun spawns."); return; }
        if (pickupPrefab == null) { Debug.LogError($"[{nameof(PickupSpawnManager)}] weaponPickupPrefab not set."); return; }
        if (options == null || options.Count == 0) { Debug.LogError($"[{nameof(PickupSpawnManager)}] No WeaponStats options set for guns."); return; }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform spawn = parent.GetChild(i);
            string spawnId = $"Gun:{GetPath(spawn)}";

            if (consumedSpawnIds.Contains(spawnId))
                continue;

            WeaponStats chosen = options[Random.Range(0, options.Count)];
            GameObject instance = Instantiate(pickupPrefab, spawn.position, spawn.rotation, spawn);

            var wp = instance.GetComponent<WeaponPickup>();
            if (wp == null)
            {
                Debug.LogError($"weaponPickupPrefab '{pickupPrefab.name}' is missing WeaponPickup.cs.");
                continue;
            }

            wp.SetWeapon(chosen);
        }
    }

    private void SpawnAmmoOnce()
    {
        SpawnAmmoFromParent(ammoSpawnsParent, ammoPrefabs);
    }

    private void SpawnAmmoFromParent(Transform parent, GameObject[] prefabs)
    {
        if (parent == null) { Debug.LogError($"[{nameof(PickupSpawnManager)}] Missing parent for Ammo spawns."); return; }
        if (prefabs == null || prefabs.Length == 0) { Debug.LogError($"[{nameof(PickupSpawnManager)}] No prefabs set for Ammo."); return; }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform spawn = parent.GetChild(i);
            string spawnId = $"Ammo:{GetPath(spawn)}";

            if (consumedSpawnIds.Contains(spawnId))
                continue;

            GameObject chosen = prefabs[Random.Range(0, prefabs.Length)];
            GameObject instance = Instantiate(chosen, spawn.position, spawn.rotation, spawn);

            var ap = instance.GetComponent<AmmoPickup>();
            if (ap == null)
            {
                Debug.LogError($"Ammo prefab '{chosen.name}' is missing AmmoPickup.cs.");
                continue;
            }

            ap.spawnManager = this;
            ap.spawnId = spawnId;
        }
    }
}
