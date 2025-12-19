using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 30;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private float destroyDelay = 0.02f;

    private bool consumed;

    public PickupSpawnManager spawnManager;
    public string spawnId;

    private void OnTriggerEnter(Collider other)
    {
        if (consumed) return;
        if (!other.CompareTag("Player")) return;

        PlayerWeapon pw = other.GetComponentInParent<PlayerWeapon>();
        if (pw == null) return;

        bool added = pw.TryAddReserveAmmo(ammoAmount);
        if (!added) return;

        consumed = true;

        // Mark this spawn point as consumed for the run (same behavior as food/o2/medkit).
        spawnManager?.MarkConsumed(spawnId);

        AudioSource src = pw.GetComponent<AudioSource>();
        if (src != null && pickupClip != null)
            src.PlayOneShot(pickupClip);

        Destroy(gameObject, destroyDelay);
    }
}
