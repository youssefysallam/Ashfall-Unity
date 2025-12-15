using UnityEngine;
public enum Pickuptype
{
    Food,
    Oxygen
}

public class Pickup : MonoBehaviour
{
    public Pickuptype type;
    public float amount = 25f;
    public float rotateSpeed = 60f;
    public PickupSpawnManager spawnManager;
    public string spawnId;


    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }    

    void OnTriggerEnter(Collider other)
    {
        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats == null) return;

        if (type == Pickuptype.Food)
            stats.AddFood(amount);
        else if (type == Pickuptype.Oxygen)
            stats.AddOxygen(amount);
        
        if (spawnManager != null && !string.IsNullOrEmpty(spawnId))
        {
            spawnManager.MarkConsumed(spawnId);
        }


        Destroy(gameObject);
    }
}
