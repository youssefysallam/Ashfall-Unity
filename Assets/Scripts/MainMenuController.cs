using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Wiring")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button quitButton;

    void Start()
    {
        if (playButton != null) playButton.onClick.AddListener(PlayNew);
        if (loadButton != null) loadButton.onClick.AddListener(Load);
        if (quitButton != null) quitButton.onClick.AddListener(Quit);

        Refresh();
    }

    public void Refresh()
    {
        if (loadButton != null)
        {
            bool canLoad = SaveLoadManager.Instance != null && SaveLoadManager.Instance.HasSave();
            loadButton.interactable = canLoad;
        }
    }

    public void PlayNew()
    {
        if (SaveLoadManager.Instance != null)
            SaveLoadManager.Instance.StartNewGame();
    }

    public void Load()
    {
        if (SaveLoadManager.Instance != null)
            SaveLoadManager.Instance.LoadGame();
    }

    public void Quit()
    {
        Application.Quit();
    }
}

