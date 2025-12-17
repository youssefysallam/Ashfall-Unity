using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GunPickup : MonoBehaviour
{
    public GameObject weaponPrefab;
    public AudioClip shotClip;      
    public float rotateSpeed = 60f;

    void Reset()
    {
        // Ensure trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        var pw = other.GetComponent<PlayerWeapon>();
        if (pw == null) return;

        pw.Equip(weaponPrefab, shotClip);
        Destroy(gameObject);
    }
}
