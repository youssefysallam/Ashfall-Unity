using UnityEngine;

public enum WeaponType
{
    Gun,
    Melee
}

public enum GunAnimSet
{
    Pistol,
    Rifle
}

[CreateAssetMenu(menuName = "Ashfall/Weapon Stats", fileName = "WeaponStats")]
public class WeaponStats : ScriptableObject
{
    [Header("Animation")]
    public GunAnimSet gunAnimSet = GunAnimSet.Pistol;
    [Header("Identity")]
    public string displayName = "Weapon";
    public WeaponType type = WeaponType.Gun;

    [Header("Prefab")]
    public GameObject weaponPrefab;

    [Header("Gun Settings")]
    public float fireRate = 0.18f;
    public float range = 120f;
    public int damage = 33;
    public bool allowADS = true;
    public bool automatic = true;
    public AudioClip shotClip;
    
    [Header("Ammo Settings (Guns Only)")]
    public int magazineSize = 30;
    public int maxReserveAmmo = 120;
    public float reloadTime = 1.6f;
    public AudioClip reloadClip;
    public Sprite weaponIcon;


    [Header("Melee Settings")]
    public int meleeDamage = 25;
    public float meleeRange = 2.2f;

    public Vector3 localPosOffset;
    public Vector3 localRotOffset;
}
