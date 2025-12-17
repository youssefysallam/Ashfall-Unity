using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponPickup : MonoBehaviour
{
    public WeaponStats weapon;
    public float rotateSpeed = 60f;
    public KeyCode pickupKey = KeyCode.E;

    [Header("Visual")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Vector3 visualLocalScale = Vector3.one;
    private GameObject visualInstance;

    [Header("Prompt")]
    public string promptFormat = "Press [E] to pick up {0}";

    private bool canPickup;
    private PlayerWeapon cachedPlayer;
    private PickupPromptUI promptUI;

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

        promptUI = Object.FindFirstObjectByType<PickupPromptUI>(FindObjectsInactive.Include);
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);

        if (!canPickup || cachedPlayer == null) return;

        if (Input.GetKeyDown(pickupKey))
        {
            cachedPlayer.Equip(weapon);

            if (promptUI != null)
                promptUI.Hide();

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        cachedPlayer = other.GetComponent<PlayerWeapon>();
        if (cachedPlayer == null) return;

        canPickup = true;

        if (promptUI != null)
        {
            string name = weapon != null ? weapon.displayName : "Weapon";
            promptUI.Show(string.Format(promptFormat, name));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerWeapon>() == null) return;

        canPickup = false;
        cachedPlayer = null;

        if (promptUI != null)
            promptUI.Hide();
    }
}
