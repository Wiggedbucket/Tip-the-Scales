using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    Health health;

    void Start()
    {
        health = GetComponent<Health>();
        health.OnDeath += HandleDeath;
    }

    void HandleDeath()
    {
        Destroy(gameObject);
    }
}