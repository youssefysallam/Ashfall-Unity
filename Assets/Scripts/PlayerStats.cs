using UnityEngine;
using System;
public class PlayerStats : MonoBehaviour
{
   [Header("Max Values")]
   public float maxHealth = 100f;
   public float maxHunger = 100f;
   public float maxOxygen = 100f;

   [Header("Base Decay Rates/sec")]
   public float hungerDecayPerSecond = 1f;
   public float oxygenDecayPerSecond = 1f;

   [Header("Damage When Starving/Suffocating")]
   public float damageperSecondWhenZero = 10f;

   public float Health { get; private set; }
   public float Hunger { get; private set; }
   public float Oxygen { get; private set; }

   public event Action OnPlayerDied;

   void Start()
    {
        Health = maxHealth;
        Hunger = maxHunger;
        Oxygen = maxOxygen;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        //Drain hunger & oxygen
        Hunger = Mathf.Max(0f, Hunger - hungerDecayPerSecond * dt);
        Oxygen = Mathf.Max(0f, Oxygen - oxygenDecayPerSecond * dt);

        //If either is zero, damage health
        if (Hunger <= 0f || Oxygen <= 0f)
        {
            Health -= damageperSecondWhenZero * dt;
        }

        Health = Mathf.Clamp(Health, 0f, maxHealth);

        if (Health <= 0f)
        {
            Die();
        }    
    }

    void Die()
    {
        Debug.Log("Player Died");
        OnPlayerDied?.Invoke();
    }

    public void AddFood(float amount)
    {
        Hunger = Mathf.Clamp(Hunger + amount, 0f, maxHunger);
    }

    public void AddOxygen(float amount)
    {
        Oxygen = Mathf.Clamp(Oxygen + amount, 0f, maxOxygen);
    }

    public void TakeDamage(float amount)
    {
        Health = Mathf.Clamp(Health - amount, 0f, maxHealth);
        if (Health <= 0f)
        {
            Die();
        }
    }
}
