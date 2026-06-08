using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] Material red;
    [SerializeField] Material green;
    [SerializeField] Renderer rend;
    bool isDead = false;
    float currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            Debug.Log("Player is dead!");
            rend.material = red;
            Destroy(gameObject, 1f);
        } else 
        {
            Debug.Log("Player took " + damage + " damage. Health :" + currentHealth);
        }
    }
    public float GetHealth()
    {
        return currentHealth;
    }
}
