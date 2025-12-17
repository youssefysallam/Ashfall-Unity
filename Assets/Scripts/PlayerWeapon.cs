using System;
using UnityEngine;

public enum WeaponType
{
    None,
    Gun,
    Melee
}

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

    [Header("Shooting")]
    [SerializeField] private float fireRate = 0.18f;
    [SerializeField] private float range = 120f;
    [SerializeField] private int damage = 25;
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Audio")]
    [SerializeField] private AudioClip pistolShot;

    [Header("Runtime")]
    [SerializeField] private GameObject currentWeapon;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private WeaponType currentWeaponType = WeaponType.None;


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

        if (animator != null)
        {
            bool ads = currentWeaponType == WeaponType.Gun && Input.GetButton(adsButton);
            animator.SetBool("ADS", ads);
        }

        if (Input.GetButtonDown(fireButton))
        {
            if (currentWeaponType == WeaponType.None)
            {
                animator.SetTrigger("Punch");
                return;
            }

            if (currentWeaponType == WeaponType.Melee)
            {
                animator.SetTrigger("Melee");
                return;
            }
        }
        if (currentWeaponType == WeaponType.Gun)
        {
            if (Input.GetButton(fireButton) && fireTimer <= 0f)
            {
                fireTimer = fireRate;
                Shoot();
            }
        }
    }

    public void Equip(GameObject weaponPrefab, AudioClip shotClip = null)
    {
        if (weaponHolder == null || weaponPrefab == null) return;

        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }

        currentWeapon = Instantiate(weaponPrefab, weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.transform.localScale = Vector3.one;

        pistolShot = shotClip != null ? shotClip : pistolShot;

        muzzleFlash = currentWeapon.GetComponentInChildren<ParticleSystem>(true);
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        currentWeaponType = WeaponType.Gun;
        
        if (animator != null)
        {
            animator.SetBool("HasPistol", true);
        }
    }

    public void Unequip()
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }

        muzzleFlash = null;

        currentWeaponType = WeaponType.None;

        if (animator != null)
        {
            animator.SetBool("HasPistol", false);
            animator.SetBool("ADS", false);
        }
    }

    private void Shoot()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash.Play(true);
        }

        if (sfxSource != null && pistolShot != null)
        {
            sfxSource.PlayOneShot(pistolShot);
        }

        if (aimCamera == null) return;

        Debug.DrawRay(aimCamera.transform.position, aimCamera.transform.forward * range, Color.red, 1f);

        Ray ray = new Ray(aimCamera.transform.position, aimCamera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, range, hitMask, QueryTriggerInteraction.Collide);
        if (hits == null || hits.Length == 0) return;

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (IsSelfCollider(hit.collider))
                continue;

            ZombieHealth zh = hit.collider.GetComponentInParent<ZombieHealth>();
            if (zh != null)
            {
                zh.TakeDamage(damage);
            }
            Debug.Log($"[Hit] {hit.collider.name} layer={LayerMask.LayerToName(hit.collider.gameObject.layer)} trigger={hit.collider.isTrigger}");

            break;
        }
    }

    private bool IsSelfCollider(Collider col)
    {
        if (col == null || selfColliders == null) return false;

        for (int i = 0; i < selfColliders.Length; i++)
        {
            if (selfColliders[i] == col)
                return true;
        }

        return false;
    }
}
