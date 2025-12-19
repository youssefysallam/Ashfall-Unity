using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MedkitPickup : MonoBehaviour
{
    public float healAmount = 35f;
    public float rotateSpeed = 60f;

    [HideInInspector] public PickupSpawnManager spawnManager;
    [HideInInspector] public string spawnId;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        var stats = other.GetComponent<PlayerStats>();
        if (stats == null) return;

        var t = stats.GetType();

        var addHealth = t.GetMethod("AddHealth");
        if (addHealth != null)
        {
            addHealth.Invoke(stats, new object[] { healAmount });
        }
        else
        {
            var healthProp = t.GetProperty("Health");
            var maxHealthField = t.GetField("maxHealth");

            if (healthProp != null && healthProp.CanWrite && maxHealthField != null)
            {
                float max = (float)maxHealthField.GetValue(stats);
                float cur = (float)healthProp.GetValue(stats);
                healthProp.SetValue(stats, Mathf.Min(max, cur + healAmount));
            }
        }

        if (spawnManager != null && !string.IsNullOrEmpty(spawnId))
            spawnManager.MarkConsumed(spawnId);

        Destroy(gameObject);
    }
}
