using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] Material red;
    [SerializeField] Material green;
    [SerializeField] Renderer rend;
    float currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        
        Debug.Log("Player took " + damage + " damage. Health :" + currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player is dead!");
            rend.material = red;
        } else
        {
            currentHealth -= damage;
        }
    }
    public float GetHealth()
    {
        return currentHealth;
    }
}
