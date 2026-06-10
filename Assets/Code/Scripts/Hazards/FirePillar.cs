using UnityEngine;
using System.Collections;

public class FirePillar : MonoBehaviour
    

{
    [Header("damage")]
    [SerializeField] float damagePerTick = 10f;
    [SerializeField] float damageInterval = 0.5f;

    [Header("Particles")]
    [SerializeField] ParticleSystem warningParticles;
    [SerializeField] ParticleSystem activeParticles;

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
            //rest phase
            if (warningParticles != null) warningParticles.Stop();
            if (activeParticles != null) activeParticles.Stop();
            float currentInterval = Mathf.Lerp(maxInterval, minInterval, scaleMultiplier);
            yield return new WaitForSeconds(currentInterval);

            //warning phase
            if (warningParticles != null) warningParticles.Play();
            Debug.Log(gameObject.name + " warning!");
            yield return new WaitForSeconds(warningDuration);

            //active phase
            if (warningParticles != null) warningParticles.Stop();
            if (activeParticles != null) activeParticles.Play();
            isActive = true;
            float currentActiveDuration = Mathf.Lerp(minActiveDuration, maxActiveDuration, scaleMultiplier);
            Debug.Log(gameObject.name + " ACTIVE!");
            yield return new WaitForSeconds(currentActiveDuration);
            isActive = false;
            if (activeParticles != null) activeParticles.Stop();
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
