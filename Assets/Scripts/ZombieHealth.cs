using UnityEngine;
using System;

public class ZombieHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    public Action<ZombieHealth> OnDied;
    public static event Action<ZombieHealth> AnyZombieDied;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0) Die();
    }

    void Die()
    {
        Debug.Log("[ZombieHealth] Die() fired");
        OnDied?.Invoke(this);
        AnyZombieDied?.Invoke(this);
        Destroy(gameObject);
    }
}
