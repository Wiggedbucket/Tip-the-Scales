using UnityEngine;

[RequireComponent(typeof(Light))]
public class ScaleLighting : MonoBehaviour
{
    [Header("Angel Side (Scale = 1)")]
    [SerializeField] private Color angelColor = new Color(0.8f, 0.9f, 1f);
    [SerializeField] private float angelIntensity = 1.2f;

    [Header("Demon Side (Scale = -1)")]
    [SerializeField] private Color demonColor = new Color(1f, 0.15f, 0f);
    [SerializeField] private float demonIntensity = 2.5f;

    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 2f;

    private Light directionalLight;
    private float currentNormalized = 0.5f;

    private void Awake()
    {
        directionalLight = GetComponent<Light>();
    }

    private void Update()
    {
        if (!GameState.InstanceExists) return;

        float targetNormalized = Mathf.Clamp01((-GameState.Instance.Scale + 1f) / 2f);

        currentNormalized = Mathf.Lerp(currentNormalized, targetNormalized, smoothSpeed * Time.deltaTime);

        directionalLight.color = Color.Lerp(angelColor, demonColor, currentNormalized);
        directionalLight.intensity = Mathf.Lerp(angelIntensity, demonIntensity, currentNormalized);
    }
}