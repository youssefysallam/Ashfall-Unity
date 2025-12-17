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
        if (playerStats == null || healthSlider == null || hungerSlider == null || oxygenSlider == null)
        {
            Debug.LogError("HUDController missing references (PlayerStats or Sliders).");
            enabled = false;
            return;
        }

        healthSlider.minValue = 0f;
        hungerSlider.minValue = 0f;
        oxygenSlider.minValue = 0f;

        healthSlider.maxValue = playerStats.maxHealth;
        hungerSlider.maxValue = playerStats.maxHunger;
        oxygenSlider.maxValue = playerStats.maxOxygen;

        // Start full
        healthSlider.value = playerStats.Health;
        hungerSlider.value = playerStats.Hunger;
        oxygenSlider.value = playerStats.Oxygen;
    }

    void Update()
    {
        healthSlider.value = playerStats.Health;
        hungerSlider.value = playerStats.Hunger;
        oxygenSlider.value = playerStats.Oxygen;

        if (GameManager.Instance != null && dayText != null)
            dayText.text = $"Day {GameManager.Instance.CurrentDay}";
    }
}
