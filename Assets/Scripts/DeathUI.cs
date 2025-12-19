using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text statsText;

    [Header("Scene Names")]
    [SerializeField] private string gameplaySceneName = "Ashfall_City";

    void Awake()
    {
        Hide();
    }

    public void Show(int dayReached, string statsSummary)
    {
        if (titleText != null)
            titleText.text = $"You Died!\nDay {dayReached}";

        if (statsText != null)
            statsText.text = statsSummary;

        if (root != null) root.SetActive(true);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Hide()
    {
        if (root != null) root.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (SaveLoadManager.Instance != null)
            SaveLoadManager.Instance.StartNewGame();
        else
            SceneManager.LoadScene(gameplaySceneName);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (SaveLoadManager.Instance != null)
            SaveLoadManager.Instance.ReturnToMainMenu();
        else
            SceneManager.LoadScene("Boot");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
