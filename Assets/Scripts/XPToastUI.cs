using TMPro;
using UnityEngine;

public class XPToastUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text text;
    [SerializeField] private float showSeconds = 1.2f;

    private float timer;

    void Awake()
    {
        if (root == null) root = gameObject;
        Hide();
    }

    void Update()
    {
        if (timer <= 0f) return;

        timer -= Time.deltaTime;
        if (timer <= 0f) Hide();
    }

    public void ShowXP(int amount)
    {
        if (text != null) text.text = $"+{amount} XP";
        if (root != null) root.SetActive(true);
        timer = showSeconds;
    }

    private void Hide()
    {
        if (root != null) root.SetActive(false);
        timer = 0f;
    }
}
