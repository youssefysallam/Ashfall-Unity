using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class XPBarUI : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text xpText;

    private XPManager xp;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Bind();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Unbind();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Bind();
    }

    private void Bind()
    {
        Unbind();

        xp = XPManager.Instance != null
            ? XPManager.Instance
            : FindFirstObjectByType<XPManager>(FindObjectsInactive.Include);

        if (xp != null)
            xp.OnXPChanged += Refresh;

        Refresh();
    }

    private void Unbind()
    {
        if (xp != null)
            xp.OnXPChanged -= Refresh;

        xp = null;
    }

    private void Refresh()
    {
        if (xpSlider == null || levelText == null || xpText == null) return;

        int lvl = 1;
        int cur = 0;
        int req = 999;

        if (xp != null)
        {
            lvl = xp.Level;
            cur = xp.XpInLevel;
            req = xp.XpRequired;
        }

        levelText.text = lvl.ToString();
        xpText.text = $"{cur}/{req}";

        xpSlider.minValue = 0f;
        xpSlider.maxValue = req;
        xpSlider.value = cur;
    }
}
