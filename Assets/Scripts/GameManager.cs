using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Day Settings")]
    public float dayLengthSeconds = 120f; //2 mins per day for now
    public int CurrentDay { get; private set; } = 1;

    private float dayTimer;

    [Header("Difficulty Scaling")]
    public float hungerDecayIncreasePerDay = 0.2f;
    public float oxygenDecayIncreasePerDay = 0.2f;

    private PlayerStats playerStats;
    [SerializeField] private DeathUI deathUI;
    private bool isDead = false;
    
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Boot")
            SceneManager.LoadScene("Gameplay_Overworld");
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
        else
            Debug.Log("No PlayerStats found in scene " + scene.name);

        deathUI = FindFirstObjectByType<DeathUI>();
        if (deathUI != null) deathUI.Hide();

        isDead = false;
        Time.timeScale = 1f;
    }


    void Update()
    {
        if (playerStats == null) return;

        dayTimer += Time.deltaTime;

        if (dayTimer >= dayLengthSeconds)
        {
            dayTimer = 0f;
            CurrentDay++;
            OnNewDay();
        }
    }

    void OnNewDay()
    {
        //Ramp difficulty
        playerStats.hungerDecayPerSecond += hungerDecayIncreasePerDay;
        playerStats.oxygenDecayPerSecond += oxygenDecayIncreasePerDay;

        Debug.Log($"New Day: {CurrentDay}. Hunger Decay: {playerStats.hungerDecayPerSecond}. Oxygen: {playerStats.oxygenDecayPerSecond}");
    }

    void HandlePlayerDeath()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Run ended. Resetting to Day 1.");
        CurrentDay = 1;
        dayTimer = 0f;

        if (deathUI != null)
            deathUI.Show(CurrentDay);
        else
            SceneManager.LoadScene("Gameplay_Overworld");
    }
}
