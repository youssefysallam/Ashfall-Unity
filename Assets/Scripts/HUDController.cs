using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public PlayerStats playerStats;
    public Slider healthSlider;
    public Slider hungerSlider;
    public Slider oxygenSlider;
    public TextMeshProUGUI dayText;

    void Start()
    {
        if (playerStats == null) { Debug.LogError("HUDController missing PlayerStats"); enabled = false; return; }

        healthSlider.maxValue = playerStats.maxHealth;
        hungerSlider.maxValue = playerStats.maxHunger;
        oxygenSlider.maxValue = playerStats.maxOxygen;
    }

    void Update()
    {
        if (playerStats == null) return;

        healthSlider.value = playerStats.Health;
        hungerSlider.value = playerStats.Hunger;
        oxygenSlider.value = playerStats.Oxygen;

        if (GameManager.Instance != null)
        {
            dayText.text = $"Day {GameManager.Instance.CurrentDay}";
        }
    
    }
}
