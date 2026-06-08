using UnityEngine;
using System.Collections;

public class FirePillar : MonoBehaviour

{
    [Header("damage")]
    [SerializeField] float damagePerTick = 10f;
    [SerializeField] float damageInterval = 0.5f;

    [Header("Timing")]
    [SerializeField] float warningDuration = 3f;
    [SerializeField] float minActiveDuration = 5f;
    [SerializeField] float maxActiveDuration = 10f;
    [SerializeField] float minInterval = 1f;
    [SerializeField] float maxInterval = 4f;

    [Header("Scale")]
    float scaleMultiplier = 0f;
    bool isActive = false;
    float nextDamageTime;

    void Start()
    {
        StartCoroutine(FireCycle());
    }

    IEnumerator FireCycle()
    {
        while (true)
        {
            float currentInterval = Mathf.Lerp(maxInterval, minInterval, scaleMultiplier);
            yield return new WaitForSeconds(currentInterval);
            Debug.Log(gameObject.name + " warning!");
            yield return new WaitForSeconds(warningDuration);
            isActive = true;
            float currentActiveDuration = Mathf.Lerp(minActiveDuration, maxActiveDuration, scaleMultiplier);
            Debug.Log(gameObject.name + " ACTIVE!");
            yield return new WaitForSeconds(currentActiveDuration);
            isActive = false;
            Debug.Log(gameObject.name + " off.");

        }
    }
    void OnTriggerStay(Collider other)
    {
        if(!isActive) return;
        if(!other.CompareTag("Player")) return;
        if(Time.time < nextDamageTime) return;

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damagePerTick);
            nextDamageTime = Time.time + damageInterval;
        }
    }
    public void SetScaleMultiplier(float normalizedscale)
    {
        scaleMultiplier = Mathf.Clamp01(normalizedscale);
    }
}
