using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Day/Wave Settings")]
    public int CurrentDay { get; private set; } = 1;

    [Tooltip("How many zombies total spawn on Day 1.")]
    public int zombiesPerDayBase = 12;

    [Tooltip("How many extra zombies are added each new day.")]
    public int zombiesPerDayIncrease = 4;

    [Tooltip("Seconds to wait after clearing a day before starting the next day.")]
    public float intermissionSeconds = 4f;

    [Header("Difficulty Scaling")]
    public float hungerDecayIncreasePerDay = 0.2f;
    public float oxygenDecayIncreasePerDay = 0.2f;

    private PlayerStats playerStats;
    [SerializeField] private DeathUI deathUI;

    private bool isDead;

    private int zombiesToSpawnThisDay;
    private int zombiesSpawnedThisDay;
    private int zombiesAlive;

    private bool dayActive;
    private float intermissionTimer;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Boot")
            SceneManager.LoadScene("Ashfall_City");
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Boot") return;

        if (playerStats != null)
            playerStats.OnPlayerDied -= HandlePlayerDeath;

        playerStats = FindFirstObjectByType<PlayerStats>();

        if (playerStats != null)
            playerStats.OnPlayerDied += HandlePlayerDeath;

        deathUI = FindFirstObjectByType<DeathUI>();
        if (deathUI != null) deathUI.Hide();

        isDead = false;
        Time.timeScale = 1f;

        ResetRunAndStartDay1();
    }

    void Update()
    {
        if (isDead) return;

        if (!dayActive)
        {
            intermissionTimer -= Time.deltaTime;
            if (intermissionTimer <= 0f)
            {
                StartDay();
            }
        }
    }

    private void ResetRunAndStartDay1()
    {
        CurrentDay = 1;
        zombiesSpawnedThisDay = 0;
        zombiesAlive = 0;

        StartDay();
    }

    private void StartDay()
    {
        dayActive = true;
        intermissionTimer = 0f;

        zombiesToSpawnThisDay = zombiesPerDayBase + (CurrentDay - 1) * zombiesPerDayIncrease;
        zombiesSpawnedThisDay = 0;
        zombiesAlive = 0;

        Debug.Log($"[Day/Wave] Day {CurrentDay} started. Quota={zombiesToSpawnThisDay}");
    }

    private void EndDay()
    {
        dayActive = false;
        intermissionTimer = intermissionSeconds;

        CurrentDay++;
        OnNewDay();

        Debug.Log($"[Day/Wave] Day cleared. Next Day={CurrentDay} in {intermissionSeconds:0.0}s");
    }

    void OnNewDay()
    {
        if (playerStats == null) return;

        playerStats.hungerDecayPerSecond += hungerDecayIncreasePerDay;
        playerStats.oxygenDecayPerSecond += oxygenDecayIncreasePerDay;
    }

    public bool CanSpawnZombie()
    {
        if (isDead) return false;
        if (!dayActive) return false;
        return zombiesSpawnedThisDay < zombiesToSpawnThisDay;
    }

    public void NotifyZombieSpawned()
    {
        zombiesSpawnedThisDay++;
        zombiesAlive++;
    }

    public void NotifyZombieDied()
    {
        zombiesAlive = Mathf.Max(0, zombiesAlive - 1);

        if (dayActive && zombiesSpawnedThisDay >= zombiesToSpawnThisDay && zombiesAlive == 0)
        {
            EndDay();
            if (SaveLoadManager.Instance != null)
                SaveLoadManager.Instance.SaveGame();
        }
    }

    void HandlePlayerDeath()
    {
        if (isDead) return;
        isDead = true;

        int dayReached = CurrentDay;

        CurrentDay = 1;
        zombiesSpawnedThisDay = 0;
        zombiesAlive = 0;
        dayActive = false;
        intermissionTimer = 0f;

        if (deathUI != null)
        {
            string summary = BuildRunSummary(dayReached);
            deathUI.Show(dayReached, summary);
        }
    }

    string BuildRunSummary(int dayReached)
    {
        if (playerStats == null)
            return $"Reached Day {dayReached}";

        return
            $"Reached Day: {dayReached}\n" +
            $"Health: {playerStats.Health:0}\n" +
            $"Hunger: {playerStats.Hunger:0}\n" +
            $"Oxygen: {playerStats.Oxygen:0}\n";
    }

    public float ZombieDamageMultiplier()
    {
        return 1f + (CurrentDay - 1) * 0.15f;
    }
}
