using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    public System.Action<ZombieHealth> OnDied;

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
        OnDied?.Invoke(this);

        Destroy(gameObject);
    }
}
