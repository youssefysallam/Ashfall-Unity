using UnityEngine;

public class DeathSceneController : MonoBehaviour
{
    [SerializeField] private DeathUI deathUI;

    void Start()
    {
        if (deathUI == null)
            deathUI = FindFirstObjectByType<DeathUI>();

        string summary =
            $"Reached Day: {RunStats.dayReached}\n" +
            $"Zombies Killed: {RunStats.zombiesKilled}\n";

        if (deathUI != null)
            deathUI.Show(RunStats.dayReached, summary);
    }
}
