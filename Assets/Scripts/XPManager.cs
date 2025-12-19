using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set; }

    [Header("Zombie XP")]
    [SerializeField] private int xpPerZombie = 80;
    [SerializeField] private XPToastUI toastUI;

    [Header("Progress")]
    [SerializeField] private int level = 1;
    [SerializeField] private int xpInLevel = 0;

    [Header("Level Requirements (per level)")]
    [SerializeField] private int[] xpRequiredByLevel = new int[]
    {
        999,   // L1
        1500,  // L2
        2200,  // L3
        3100,  // L4
        4200,  // L5
        5500,  // L6
        7000,  // L7
        9000,  // L8
        11500, // L9
        14500  // L10
    };

    public int Level => level;
    public int XpInLevel => xpInLevel;
    public int XpRequired => GetXpRequired(level);

    public event Action OnXPChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        level = Mathf.Max(1, level);
        xpInLevel = Mathf.Max(0, xpInLevel);
    }

    private void OnEnable()
    {
        ZombieHealth.AnyZombieDied += OnZombieDied;
        SceneManager.sceneLoaded += OnSceneLoaded;
        RefreshUI();
        OnXPChanged?.Invoke();
    }

    private void OnDisable()
    {
        ZombieHealth.AnyZombieDied -= OnZombieDied;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshUI();
        OnXPChanged?.Invoke();
    }

    public void RefreshUI()
    {
        toastUI = FindFirstObjectByType<XPToastUI>(FindObjectsInactive.Include);
        Debug.Log($"[XP] RefreshUI toastUI={(toastUI != null ? toastUI.name : "NULL")}");
    }

    private void OnZombieDied(ZombieHealth zh)
    {
        Debug.Log("[XP] Zombie died -> XP awarded");

        AddXP(xpPerZombie);

        if (toastUI != null)
            toastUI.ShowXP(xpPerZombie);
        else
            Debug.LogWarning("[XP] toastUI is NULL (XPToastUI not found in loaded scene)");
    }

    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        xpInLevel += amount;

        while (xpInLevel >= XpRequired)
        {
            xpInLevel -= XpRequired;
            level++;
        }

        OnXPChanged?.Invoke();
    }

    public void SetProgress(int newLevel, int newXpInLevel)
    {
        level = Mathf.Max(1, newLevel);
        xpInLevel = Mathf.Max(0, newXpInLevel);
        OnXPChanged?.Invoke();
    }

    public void ResetProgress()
    {
        level = 1;
        xpInLevel = 0;
        OnXPChanged?.Invoke();
    }

    private int GetXpRequired(int forLevel)
    {
        if (xpRequiredByLevel != null && xpRequiredByLevel.Length > 0)
        {
            int index = Mathf.Clamp(forLevel - 1, 0, xpRequiredByLevel.Length - 1);
            int last = xpRequiredByLevel[xpRequiredByLevel.Length - 1];

            if (forLevel - 1 < xpRequiredByLevel.Length)
                return Mathf.Max(1, xpRequiredByLevel[index]);

            int extraLevels = (forLevel - 1) - (xpRequiredByLevel.Length - 1);
            float scaled = last * Mathf.Pow(1.18f, extraLevels);
            return Mathf.Max(1, Mathf.RoundToInt(scaled));
        }

        return 999;
    }
}

