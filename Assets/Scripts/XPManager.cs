using UnityEngine;
using UnityEngine.SceneManagement;

public class XPManager : MonoBehaviour
{
    [SerializeField] private int xpPerZombie = 80;
    [SerializeField] private XPToastUI toastUI;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        ZombieHealth.AnyZombieDied += OnZombieDied;
        SceneManager.sceneLoaded += OnSceneLoaded;
        RefreshUI();
    }

    void OnDisable()
    {
        ZombieHealth.AnyZombieDied -= OnZombieDied;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        toastUI = Object.FindFirstObjectByType<XPToastUI>(FindObjectsInactive.Include);
        Debug.Log($"[XP] RefreshUI toastUI={(toastUI != null ? toastUI.name : "NULL")}");
    }

    private void OnZombieDied(ZombieHealth zh)
    {
        Debug.Log("[XP] Zombie died -> XP awarded");

        if (toastUI != null)
            toastUI.ShowXP(xpPerZombie);
        else
            Debug.LogWarning("[XP] toastUI is NULL (XPToastUI not found in loaded scene)");
    }
}
