using UnityEngine;
using UnityEngine.UIElements;

public class BalanceBar : MonoBehaviour
{
    [SerializeField]
    private UIDocument document;

    private VisualElement winningOverlay;
    private VisualElement dangerOverlay;

    private VisualElement balanceDemonicBarFill;
    private VisualElement balanceAngelicBarFill;

    private Label balancePointsLabel;
    private Label demonPoints;
    private Label angelPoints;

    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float pulseMinOpacity = 0f;
    [SerializeField] private float pulseMaxOpacity = 0.4f;

    private void Awake()
    {
        var root = document.rootVisualElement;

        winningOverlay = root.Q<VisualElement>("WinningOverlay");
        dangerOverlay = root.Q<VisualElement>("DangerOverlay");

        balanceDemonicBarFill = root.Q<VisualElement>("BalanceDemonicBarFill");
        balanceAngelicBarFill = root.Q<VisualElement>("BalanceAngelicBarFill");

        balancePointsLabel = root.Q<Label>("BalancePointsLabel");
        demonPoints = root.Q<Label>("DemonPoints");
        angelPoints = root.Q<Label>("AngelPoints");
    }

    private void Update()
    {
        UpdateProgressBar();
        UpdateOverlays();
    }

    private void UpdateProgressBar()
    {
        float scale = Mathf.Clamp(GameState.Instance.Scale, -1f, 1f);

        float angelPercent = (scale + 1f) * 0.5f;
        float demonPercent = 1f - angelPercent;

        balanceAngelicBarFill.style.width = Length.Percent(angelPercent * 100f);
        balanceDemonicBarFill.style.width = Length.Percent(demonPercent * 100f);

        balancePointsLabel.text = $"Balance: {scale:0.00}";

        demonPoints.text = $"{GameState.Instance.GlobalCombatPoints.demonPoints}";
        angelPoints.text = $"{GameState.Instance.GlobalCombatPoints.angelPoints}";
    }

    private void UpdateOverlays()
    {
        float scale = GameState.Instance.Scale;

        float threshold = Mathf.Abs(GameState.Instance.ScaleTreshold);

        float intensity = Mathf.InverseLerp(threshold, 1f, Mathf.Abs(scale));

        float currentPulseSpeed = pulseSpeed * Mathf.Lerp(1f, 2f, intensity);

        float pulse = Mathf.Lerp(pulseMinOpacity, pulseMaxOpacity, (Mathf.Sin(Time.time * currentPulseSpeed) + 1f) * 0.5f);

        dangerOverlay.style.opacity = scale <= -threshold ? pulse : 0f;
        winningOverlay.style.opacity = scale >= threshold ? pulse : 0f;
    }
}
