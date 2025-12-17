using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Attach")]
    public Transform weaponHolder;    
    public Camera aimCamera;           
    public AudioSource sfxSource;      

    [Header("Gun Runtime")]
    public GameObject currentWeapon;
    public Transform muzzlePoint;      
    public AudioClip pistolShot;

    [Header("Shooting")]
    public float fireRate = 0.18f;
    public float range = 120f;
    public int damage = 25;
    public LayerMask hitMask = ~0;     

    float fireTimer;

    void Awake()
    {
        if (aimCamera == null) aimCamera = Camera.main;
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        if (currentWeapon == null) return;

        // Left mouse / Fire1
        if (Input.GetButton("Fire1") && fireTimer <= 0f)
        {
            fireTimer = fireRate;
            Shoot();
        }
    }

    public void Equip(GameObject weaponPrefab, AudioClip shotClip = null, Transform muzzle = null)
    {
        if (weaponHolder == null)
        {
            Debug.LogError("[PlayerWeapon] weaponHolder not set.");
            return;
        }

        // Remove old gun
        if (currentWeapon != null) Destroy(currentWeapon);

        // Spawn new gun
        currentWeapon = Instantiate(weaponPrefab, weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.transform.localScale = Vector3.one;

        if (shotClip != null) pistolShot = shotClip;
        if (muzzle != null) muzzlePoint = muzzle;
    }

    void Shoot()
    {
        if (aimCamera == null) aimCamera = Camera.main;

        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            // Damage zombie if present on hit object or parent
            var zh = hit.collider.GetComponentInParent<ZombieHealth>();
            if (zh != null)
            {
                zh.TakeDamage(damage);
            }

        }

        // Audio
        if (sfxSource != null && pistolShot != null)
            sfxSource.PlayOneShot(pistolShot);
    }
}
