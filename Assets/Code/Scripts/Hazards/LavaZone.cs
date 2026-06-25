using UnityEngine;

public class LavaZone : MonoBehaviour
{
    [SerializeField] private float damagePerTick = 10f;

    [Header("Scale-driven interval")]
    [SerializeField] private float maxDamageInterval = 1.5f;
    [SerializeField] private float minDamageInterval = 0.4f;

    [Header("Light")]
    [SerializeField] private Light hazardLight;
    [SerializeField] private float lightBaseIntensity = 1.5f;
    [SerializeField] private float lightPulseStrength = 0.4f;
    [SerializeField] private float lightPulseSpeed = 2f;

    private float damageInterval = 1.5f;
    private float nextDamageTime;

    private void Update()
    {
        if (GameState.InstanceExists)
        {
            float normalized = Mathf.Clamp01((-GameState.Instance.Scale + 1f) / 2f);
            damageInterval = Mathf.Lerp(maxDamageInterval, minDamageInterval, normalized);
        }

        if (hazardLight != null)
        {
            float pulse = Mathf.Sin(Time.time * lightPulseSpeed) * lightPulseStrength;
            hazardLight.intensity = lightBaseIntensity + pulse;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        nextDamageTime = 0f;
        Debug.Log("Player entered lava zone.");
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time >= nextDamageTime)
        {
            Health health = other.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damagePerTick);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Player exited lava zone.");
    }
}