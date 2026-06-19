using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    Health health;

    private void Start()
    {
        health = GetComponent<Health>();
        health.OnDeath += HandleDeath;
    }

    private void HandleDeath()
    {
        //Destroy(gameObject);
    }
}