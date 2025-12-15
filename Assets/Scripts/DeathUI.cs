using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathUI : MonoBehaviour
{
    [SerializeField] GameObject root;
    [SerializeField] TMP_Text titleText;

    void Awake()
    {
        Hide();
    }

    public void Show(int dayReached)
    {
        if (titleText != null)
            titleText.text = $"You Died!\nDay {dayReached}";

        root.SetActive(true);

        Time.timeScale = 0f;

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

        SceneManager.LoadScene("Gameplay_Overworld");
    }
}
