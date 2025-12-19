using System;
using System.Collections;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Attach")]
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Camera aimCamera;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string adsButton = "Fire2";
    [SerializeField] private string fireButton = "Fire1";
    [SerializeField] private KeyCode reloadKey = KeyCode.R;

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

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;

    [Tooltip("Fallback pistol shot if WeaponStats.shotClip is not set.")]
    [SerializeField] private AudioClip pistolShot;

    [Tooltip("Fallback rifle shot if WeaponStats.shotClip is not set.")]
    [SerializeField] private AudioClip rifleShot;

    [Tooltip("Global reload sound used if WeaponStats.reloadClip is not set.")]
    [SerializeField] private AudioClip reloadClip;

    [Tooltip("Global empty-mag click used for all guns when trying to fire at 0 ammo.")]
    [SerializeField] private AudioClip emptyClip;

    [Header("Ammo (guns only)")]
    [SerializeField] private AmmoUI ammoUI;
    [SerializeField] private bool autoReloadWhenEmpty = false;

    private float fireTimer;
    private Collider[] selfColliders;

    private int ammoInMag;
    private int ammoReserve;
    private bool isReloading;

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

        if (currentStats == null)
        {
            if (ammoUI != null) ammoUI.SetVisible(false);

            if (Input.GetButtonDown(fireButton))
            {
                if (animator != null) animator.SetTrigger("Punch");
            }

            return;
        }

        if (currentStats.type == WeaponType.Melee)
        {
            if (ammoUI != null) ammoUI.SetVisible(false);

            if (Input.GetButtonDown(fireButton))
            {
                if (animator != null) animator.SetTrigger("Melee");

                if (meleeTimer <= 0f)
                {
                    meleeTimer = meleeCooldown;
                    DoMeleeHit();
                }
            }

            return;
        }

        if (currentStats.type == WeaponType.Gun)
        {
            if (ammoUI != null) ammoUI.SetVisible(true);

            if (Input.GetKeyDown(reloadKey))
            {
                TryReload();
            }

            bool wantsFire = currentStats.automatic ? Input.GetButton(fireButton) : Input.GetButtonDown(fireButton);

            if (wantsFire && fireTimer <= 0f)
            {
                fireTimer = Mathf.Max(0.01f, currentStats.fireRate);

                if (isReloading)
                    return;

                if (ammoInMag <= 0)
                {
                    PlayEmptyClick();

                    if (autoReloadWhenEmpty)
                        TryReload();

                    return;
                }

                ammoInMag--;
                UpdateAmmoUI();
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
        currentWeapon.transform.localPosition = stats.localPosOffset;
        currentWeapon.transform.localRotation = Quaternion.Euler(stats.localRotOffset);
        currentWeapon.transform.localScale = Vector3.one;

        muzzleFlash = currentWeapon.GetComponentInChildren<ParticleSystem>(true);
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (animator != null)
        {
            if (stats.type == WeaponType.Gun)
            {
                bool isPistol = stats.gunAnimSet == GunAnimSet.Pistol;
                animator.SetBool("HasPistol", isPistol);
                animator.SetBool("HasRifle", !isPistol);
            }
            else
            {
                animator.SetBool("HasPistol", false);
                animator.SetBool("HasRifle", false);
            }

            animator.SetBool("ADS", false);
        }

        isReloading = false;

        if (stats.type == WeaponType.Gun)
        {
            int magSize = Mathf.Max(1, stats.magazineSize);
            ammoInMag = magSize;
            ammoReserve = Mathf.Max(0, stats.maxReserveAmmo);
            UpdateAmmoUI();
        }
        else
        {
            ammoInMag = 0;
            ammoReserve = 0;
            if (ammoUI != null) ammoUI.SetVisible(false);
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

        ammoInMag = 0;
        ammoReserve = 0;
        isReloading = false;

        if (ammoUI != null) ammoUI.SetVisible(false);

        if (animator != null)
        {
            animator.SetBool("HasPistol", false);
            animator.SetBool("HasRifle", false);
            animator.SetBool("ADS", false);
        }
    }

    private void TryReload()
    {
        if (isReloading) return;
        if (currentStats == null || currentStats.type != WeaponType.Gun) return;

        int magSize = Mathf.Max(1, currentStats.magazineSize);
        if (ammoInMag >= magSize) return;
        if (ammoReserve <= 0) return;

        StartCoroutine(ReloadRoutine(magSize));
    }

    private IEnumerator ReloadRoutine(int magSize)
    {
        isReloading = true;

        PlayReload();

        float t = Mathf.Max(0.05f, currentStats != null ? currentStats.reloadTime : 1.2f);
        yield return new WaitForSeconds(t);

        int needed = magSize - ammoInMag;
        int take = Mathf.Min(needed, ammoReserve);

        ammoInMag += take;
        ammoReserve -= take;

        isReloading = false;
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (ammoUI == null)
            return;

        if (currentStats == null || currentStats.type != WeaponType.Gun)
        {
            ammoUI.SetVisible(false);
            return;
        }

        ammoUI.SetVisible(true);
        int magSize = Mathf.Max(1, currentStats.magazineSize);
        ammoUI.SetWeapon(currentStats);
        ammoUI.SetAmmo(ammoInMag, ammoReserve, magSize);
    }

    private void ShootGun()
    {
        if (animator != null)
        {
            animator.SetTrigger("Fire");
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash.Play(true);
        }

        PlayShot();

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

    private void PlayShot()
    {
        if (sfxSource == null || currentStats == null) return;

        AudioClip clip = currentStats.shotClip;

        if (clip == null)
        {
            bool isPistol = currentStats.gunAnimSet == GunAnimSet.Pistol;
            clip = isPistol ? pistolShot : rifleShot;
        }

        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    private void PlayReload()
    {
        if (sfxSource == null || currentStats == null) return;

        AudioClip clip = currentStats.reloadClip != null ? currentStats.reloadClip : reloadClip;
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    private void PlayEmptyClick()
    {
        if (sfxSource == null) return;
        if (emptyClip != null)
            sfxSource.PlayOneShot(emptyClip);
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
            return;

        if (aimCamera == null)
            return;

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
            range,
            ~0,
            QueryTriggerInteraction.Collide
        );

        if (hits == null || hits.Length == 0) return;

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            if (IsSelfCollider(hit.collider))
                continue;

            ZombieHealth zh = hit.collider.GetComponentInParent<ZombieHealth>();
            if (zh != null)
            {
                zh.TakeDamage(dmg);
                break;
            }
        }
    }

    public bool TryAddReserveAmmo(int amount)
    {
        if (currentStats == null) return false;
        if (currentStats.type != WeaponType.Gun) return false;

        int maxReserve = Mathf.Max(0, currentStats.maxReserveAmmo);
        if (maxReserve <= 0) return false;

        int before = ammoReserve;
        ammoReserve = Mathf.Clamp(ammoReserve + Mathf.Max(0, amount), 0, maxReserve);

        if (ammoReserve == before) return false;

        UpdateAmmoUI();
        return true;
    }

    public (int inMag, int reserve, int magSize) GetAmmoState()
    {
        if (currentStats == null || currentStats.type != WeaponType.Gun)
            return (0, 0, 0);

        return (ammoInMag, ammoReserve, Mathf.Max(1, currentStats.magazineSize));
    }
}
