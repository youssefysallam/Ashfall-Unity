using System;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Attach")]
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Camera aimCamera;
    [SerializeField] private AudioSource sfxSource;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string adsButton = "Fire2";
    [SerializeField] private string fireButton = "Fire1";

    [Header("Hit")]
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Runtime")]
    [SerializeField] private GameObject currentWeapon;
    [SerializeField] private WeaponStats currentStats;
    [SerializeField] private ParticleSystem muzzleFlash;
    [Header("Melee")]
    [SerializeField] private float meleeCooldown = 0.55f;
    [SerializeField] private float meleeRadius = 0.85f;

    private float meleeTimer;

    private float fireTimer;
    private Collider[] selfColliders;

    private void Awake()
    {
        if (aimCamera == null) aimCamera = Camera.main;
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();

        selfColliders = GetComponentsInChildren<Collider>(true);
    }

    private void Update()
    {
        fireTimer -= Time.deltaTime;
        meleeTimer -= Time.deltaTime;

        if (animator != null)
        {
            bool ads = currentStats != null
                       && currentStats.type == WeaponType.Gun
                       && currentStats.allowADS
                       && Input.GetButton(adsButton);

            animator.SetBool("ADS", ads);
        }

        if (Input.GetButtonDown(fireButton))
        {
            if (currentStats == null)
            {
                if (animator != null) animator.SetTrigger("Punch");
                return;
            }

            if (currentStats.type == WeaponType.Melee)
            {
                Debug.Log("[MELEE] Triggered melee input");

                if (animator != null) animator.SetTrigger("Melee");

                if (meleeTimer <= 0f)
                {
                    meleeTimer = meleeCooldown;
                    DoMeleeHit();
                }

                return;
            }

        }

        if (currentStats == null) return;

        if (currentStats.type == WeaponType.Gun)
        {
            bool wantsFire = currentStats.automatic ? Input.GetButton(fireButton) : Input.GetButtonDown(fireButton);

            if (wantsFire && fireTimer <= 0f)
            {
                fireTimer = Mathf.Max(0.01f, currentStats.fireRate);
                ShootGun();
            }
        }
    }

    public void Equip(WeaponStats stats)
    {
        if (weaponHolder == null || stats == null || stats.weaponPrefab == null) return;

        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }

        currentStats = stats;

        currentWeapon = Instantiate(stats.weaponPrefab, weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.transform.localScale = Vector3.one;

        muzzleFlash = currentWeapon.GetComponentInChildren<ParticleSystem>(true);
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (animator != null)
        {
            animator.SetBool("HasPistol", stats.type == WeaponType.Gun);
        }
    }

    public void Unequip()
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }

        currentStats = null;
        muzzleFlash = null;

        if (animator != null)
        {
            animator.SetBool("HasPistol", false);
            animator.SetBool("ADS", false);
        }
    }

    private void ShootGun()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash.Play(true);
        }

        if (sfxSource != null && currentStats != null && currentStats.shotClip != null)
        {
            sfxSource.PlayOneShot(currentStats.shotClip);
        }

        if (aimCamera == null || currentStats == null) return;

        float range = Mathf.Max(1f, currentStats.range);
        int damage = Mathf.Max(0, currentStats.damage);

        Ray ray = new Ray(aimCamera.transform.position, aimCamera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, range, hitMask, QueryTriggerInteraction.Collide);
        if (hits == null || hits.Length == 0) return;

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (IsSelfCollider(hit.collider)) continue;

            ZombieHealth zh = hit.collider.GetComponentInParent<ZombieHealth>();
            if (zh != null)
            {
                zh.TakeDamage(damage);
            }

            break;
        }
    }

    private bool IsSelfCollider(Collider col)
    {
        if (col == null || selfColliders == null) return false;

        for (int i = 0; i < selfColliders.Length; i++)
        {
            if (selfColliders[i] == col) return true;
        }

        return false;
    }

    private void DoMeleeHit()
    {
        if (currentStats == null || currentStats.type != WeaponType.Melee)
        {
            Debug.Log("[MELEE] Abort: no melee stats");
            return;
        }

        if (aimCamera == null)
        {
            Debug.Log("[MELEE] Abort: no aimCamera");
            return;
        }

        float range = Mathf.Max(0.5f, currentStats.meleeRange);
        int dmg = Mathf.Max(0, currentStats.meleeDamage);

        Vector3 origin = transform.position + Vector3.up * 1.2f + transform.forward * 0.8f;
        Vector3 dir = transform.forward;
        dir.y = 0f;
        dir.Normalize();

        RaycastHit[] hits = Physics.SphereCastAll(
            origin,
            meleeRadius,
            dir,
            currentStats.meleeRange,
            ~0,
            QueryTriggerInteraction.Collide
        );

        Debug.Log($"[MELEE] SphereCast origin={origin} dir={dir} range={range} dmg={dmg}");

        Debug.DrawRay(origin, dir * currentStats.meleeRange, Color.green, 1f);

        Debug.Log($"[MELEE] Hits found: {(hits == null ? 0 : hits.Length)}");

        if (hits == null || hits.Length == 0) return;

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];

            Debug.Log($"[MELEE] Hit {hit.collider.name} (layer={LayerMask.LayerToName(hit.collider.gameObject.layer)})");

            if (IsSelfCollider(hit.collider))
            {
                Debug.Log("[MELEE] Skipped self collider");
                continue;
            }

            ZombieHealth zh = hit.collider.GetComponentInParent<ZombieHealth>();
            if (zh != null)
            {
                Debug.Log($"[MELEE] DAMAGING zombie for {dmg}");
                zh.TakeDamage(dmg);
                break;
            }
        }
    }
}
