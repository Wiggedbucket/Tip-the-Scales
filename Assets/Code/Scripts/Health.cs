using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    float currentHealth;
    bool isDead = false;
    public event Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            OnDeath?.Invoke();
        }
    }

    public float GetHealth()
    {
        return currentHealth;
    }

}
