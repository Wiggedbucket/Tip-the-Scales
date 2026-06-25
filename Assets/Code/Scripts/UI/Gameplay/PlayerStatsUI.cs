using UnityEngine;
using UnityEngine.UIElements;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField]
    private UIDocument document;

    private Label ammoLabel;
    private Label healthLabel;

    private VisualElement crosshair;

    private VisualElement dashBarFill;
    private VisualElement healthBarFill;

    [SerializeField]
    private PlayerCharacterController playerController;

    [SerializeField]
    private PlayerShooting shooting;

    [SerializeField]
    private Health health;

    [SerializeField]
    private float crosshairAppearTime = 0.1f;

    private float crosshairTimer = 0f;

    private EventBinding<TookDamageEvent> tookDamageBinding;

    private void OnEnable()
    {
        tookDamageBinding = new EventBinding<TookDamageEvent>(ShowHitIndicator);
        EventBus<TookDamageEvent>.Register(tookDamageBinding);
    }

    private void OnDisable()
    {
        EventBus<TookDamageEvent>.Deregister(tookDamageBinding);
    }

    private void Awake()
    {
        var root = document.rootVisualElement;

        ammoLabel = root.Q<Label>("AmmoLabel");
        healthLabel = root.Q<Label>("HealthLabel");
        crosshair = root.Q<VisualElement>("HitIndicator");

        dashBarFill = root.Q<VisualElement>("DashBarFill");
        healthBarFill = root.Q<VisualElement>("HealthBarFill");
    }

    private void Update()
    {
        if (shooting != null)
            ammoLabel.text = $"Ammo: {shooting.CurrentAmmo}/{shooting.CurrentWeapon.MaxAmmo}";

        if (crosshairTimer < crosshairAppearTime)
            crosshairTimer += Time.deltaTime;
        else
            crosshair.AddToClassList("hidden");

        UpdateDashBar();

        UpdateHealthBar();
    }

    private void UpdateDashBar()
    {
        float dashPercent = playerController.DashCooldownProgress;

        dashBarFill.style.width = Length.Percent(dashPercent * 100f);

        if (dashPercent >= 1f)
            dashBarFill.AddToClassList("dash-filled");
        else
            dashBarFill.RemoveFromClassList("dash-filled");
    }

    private void UpdateHealthBar()
    {
        if (health == null)
            return;

        healthLabel.text = $"Health: {health.GetHealth()}/{health.GetMaxHealth()}";

        float healthPercent = (float)health.GetHealth() / health.GetMaxHealth();

        healthBarFill.style.width = Length.Percent(healthPercent * 100f);
    }

    private void ShowHitIndicator(TookDamageEvent e)
    {
        if (!e.IsEnemy)
            return;

        if (e.Died)
            crosshair.AddToClassList("red");
        else
            crosshair.RemoveFromClassList("red");

        crosshair.RemoveFromClassList("hidden");

        crosshairTimer = 0f;
    }
}
