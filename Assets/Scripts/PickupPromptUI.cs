using TMPro;
using UnityEngine;

public class PickupPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text text;

    void Awake()
    {
        if (root == null) root = gameObject;
        Hide();
    }

    public void Show(string message)
    {
        if (text != null) text.text = message;
        if (root != null) root.SetActive(true);
    }

    public void Hide()
    {
        if (root != null) root.SetActive(false);
    }
}
