using UnityEngine;

public enum WeaponType
{
    Gun,
    Melee
}

[CreateAssetMenu(menuName = "Ashfall/Weapon Stats", fileName = "WeaponStats")]
public class WeaponStats : ScriptableObject
{
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

    [Header("Melee Settings")]
    public int meleeDamage = 25;
    public float meleeRange = 2.2f;
}
