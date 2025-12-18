using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string gameplaySceneName = "Ashfall_City";
    [SerializeField] private string bootSceneName = "Boot";

    private const string KEY_HAS_SAVE = "SF_HAS_SAVE";
    private const string KEY_DAY = "SF_DAY";
    private const string KEY_HEALTH = "SF_HEALTH";
    private const string KEY_HUNGER = "SF_HUNGER";
    private const string KEY_OXYGEN = "SF_OXYGEN";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool HasSave()
    {
        return PlayerPrefs.GetInt(KEY_HAS_SAVE, 0) == 1;
    }

    public void ClearSave()
    {
        PlayerPrefs.DeleteKey(KEY_HAS_SAVE);
        PlayerPrefs.DeleteKey(KEY_DAY);
        PlayerPrefs.DeleteKey(KEY_HEALTH);
        PlayerPrefs.DeleteKey(KEY_HUNGER);
        PlayerPrefs.DeleteKey(KEY_OXYGEN);
        PlayerPrefs.Save();
    }

    public void SaveGame()
    {
        var gm = GameManager.Instance;
        var ps = FindFirstObjectByType<PlayerStats>();

        if (gm == null || ps == null)
        {
            Debug.LogWarning("[SaveLoad] Missing GameManager or PlayerStats; save skipped.");
            return;
        }

        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.SetInt(KEY_DAY, gm.CurrentDay);

        PlayerPrefs.SetFloat(KEY_HEALTH, ps.Health);
        PlayerPrefs.SetFloat(KEY_HUNGER, ps.Hunger);
        PlayerPrefs.SetFloat(KEY_OXYGEN, ps.Oxygen);

        PlayerPrefs.Save();
        Debug.Log("[SaveLoad] Saved.");
    }

    public void LoadGame()
    {
        if (!HasSave())
        {
            Debug.LogWarning("[SaveLoad] No save found.");
            return;
        }

        SceneManager.sceneLoaded += ApplyLoadedStateOnSceneLoaded;
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void StartNewGame()
    {
        ClearSave();
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(bootSceneName);
    }

    private void ApplyLoadedStateOnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= ApplyLoadedStateOnSceneLoaded;

        var gm = GameManager.Instance;
        var ps = FindFirstObjectByType<PlayerStats>();

        if (gm == null || ps == null)
        {
            Debug.LogWarning("[SaveLoad] Missing GameManager or PlayerStats; load apply skipped.");
            return;
        }

        int savedDay = PlayerPrefs.GetInt(KEY_DAY, 1);

        var dayProp = typeof(GameManager).GetProperty("CurrentDay");
        if (dayProp != null && dayProp.CanWrite)
        {
            dayProp.SetValue(gm, savedDay);
        }
        else
        {
            var dayField = typeof(GameManager).GetField("<CurrentDay>k__BackingField",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic);

            if (dayField != null)
                dayField.SetValue(gm, savedDay);
        }

        float h = PlayerPrefs.GetFloat(KEY_HEALTH, ps.maxHealth);
        float hu = PlayerPrefs.GetFloat(KEY_HUNGER, ps.maxHunger);
        float ox = PlayerPrefs.GetFloat(KEY_OXYGEN, ps.maxOxygen);

        var healthProp = typeof(PlayerStats).GetProperty("Health");
        var hungerProp = typeof(PlayerStats).GetProperty("Hunger");
        var oxygenProp = typeof(PlayerStats).GetProperty("Oxygen");

        if (healthProp != null && healthProp.CanWrite) healthProp.SetValue(ps, h);
        if (hungerProp != null && hungerProp.CanWrite) hungerProp.SetValue(ps, hu);
        if (oxygenProp != null && oxygenProp.CanWrite) oxygenProp.SetValue(ps, ox);

        var hField = typeof(PlayerStats).GetField("<Health>k__BackingField",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        var huField = typeof(PlayerStats).GetField("<Hunger>k__BackingField",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        var oxField = typeof(PlayerStats).GetField("<Oxygen>k__BackingField",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (hField != null) hField.SetValue(ps, h);
        if (huField != null) huField.SetValue(ps, hu);
        if (oxField != null) oxField.SetValue(ps, ox);

        Debug.Log("[SaveLoad] Loaded and applied.");
    }

    void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name == gameplaySceneName)
            SaveGame();
    }
}
