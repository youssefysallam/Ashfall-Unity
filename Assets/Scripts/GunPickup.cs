using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GunPickup : MonoBehaviour
{
    public WeaponStats weapon;
    public float rotateSpeed = 60f;
    [SerializeField] private Vector3 visualLocalScale = Vector3.one;
    [SerializeField] private Transform visualRoot;
    private GameObject visualInstance;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Awake()
    {
        if (visualRoot == null) visualRoot = transform;

        if (weapon != null && weapon.weaponPrefab != null)
        {
            visualInstance = Instantiate(weapon.weaponPrefab, visualRoot);
            visualInstance.transform.localPosition = Vector3.zero;
            visualInstance.transform.localRotation = Quaternion.identity;
            visualInstance.transform.localScale = visualLocalScale;

            foreach (var c in visualInstance.GetComponentsInChildren<Collider>(true))
                c.enabled = false;
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        var pw = other.GetComponent<PlayerWeapon>();
        if (pw == null) return;

        pw.Equip(weapon);
        Destroy(gameObject);
    }
}
