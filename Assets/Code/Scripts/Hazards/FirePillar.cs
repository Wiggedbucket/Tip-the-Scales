using UnityEngine;
using System.Collections;
using AudioSystem;

public class FirePillar : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damagePerTick = 10f;
    [SerializeField] private float damageInterval = 0.5f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem warningParticles;
    [SerializeField] private ParticleSystem activeParticles;

    [Header("Timing")]
    [SerializeField] private float warningDuration = 3f;
    [SerializeField] private float minActiveDuration = 5f;
    [SerializeField] private float maxActiveDuration = 10f;
    [SerializeField] private float minInterval = 1f;
    [SerializeField] private float maxInterval = 4f;

    [Header("Light")]
    [SerializeField] private Light hazardLight;
    [SerializeField] private float lightBaseIntensity = 3f;
    [SerializeField] private float lightFlickerStrength = 0.8f;
    [SerializeField] private float lightFlickerSpeed = 8f;

    private float scaleMultiplier = 0f;
    private bool isActive = false;
    private float nextDamageTime;

    private void Start()
    {
        if (hazardLight != null) hazardLight.enabled = false;
        StartCoroutine(FireCycle());
    }

    private void Update()
    {
        if (GameState.InstanceExists)
        {
            float normalized = Mathf.Clamp01((-GameState.Instance.Scale + 1f) / 2f);
            SetScaleMultiplier(normalized);
        }

        // Flicker while active
        if (isActive && hazardLight != null)
        {
            float flicker = Mathf.Sin(Time.time * lightFlickerSpeed) * lightFlickerStrength;
            hazardLight.intensity = lightBaseIntensity + flicker;
        }
    }

    private IEnumerator FireCycle()
    {
        yield return new WaitForSeconds(Random.Range(0f, maxInterval));
        while (true)
        {
            // Rest phase
            if (warningParticles != null) warningParticles.Stop();
            if (activeParticles != null) activeParticles.Stop();
            if (hazardLight != null) hazardLight.enabled = false;
            float currentInterval = Mathf.Lerp(maxInterval, minInterval, scaleMultiplier);
            yield return new WaitForSeconds(currentInterval);

            // Warning phase
            if (warningParticles != null) warningParticles.Play();
            yield return new WaitForSeconds(warningDuration);

            // Active phase
            if (warningParticles != null) warningParticles.Stop();
            if (activeParticles != null) activeParticles.Play();
            if (hazardLight != null) hazardLight.enabled = true;
            isActive = true;
            float currentActiveDuration = Mathf.Lerp(minActiveDuration, maxActiveDuration, scaleMultiplier);
            SoundManager.instance.CreateSound().WithSoundData("LavaPillar").WithPosition(transform.position).WithrandomPitch().Play();
            yield return new WaitForSeconds(currentActiveDuration);
            isActive = false;
            if (hazardLight != null) hazardLight.enabled = false;
            if (activeParticles != null) activeParticles.Stop();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isActive) return;
        if (!other.CompareTag("Player")) return;
        if (Time.time < nextDamageTime) return;

        Health health = other.GetComponentInParent<Health>();
        if (health != null)
        {
            health.TakeDamage(damagePerTick);
            nextDamageTime = Time.time + damageInterval;
        }
    }

    public void SetScaleMultiplier(float normalizedScale)
    {
        scaleMultiplier = Mathf.Clamp01(normalizedScale);
    }
}