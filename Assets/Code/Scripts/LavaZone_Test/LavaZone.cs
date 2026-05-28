using UnityEngine;

public class LavaZone : MonoBehaviour
{
    [SerializeField] float damagePerTick = 10f;
    [SerializeField] float damageInterval = 1f;
    float nextDamageTime;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        nextDamageTime = 0f;
        Debug.Log("Player entered lava zone.");
    }

    void OnTriggerStay(Collider other)
    { 
        if (!other.CompareTag("Player")) return;
        if (Time.time >= nextDamageTime)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damagePerTick);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Player exited lava zone.");
    }
}
