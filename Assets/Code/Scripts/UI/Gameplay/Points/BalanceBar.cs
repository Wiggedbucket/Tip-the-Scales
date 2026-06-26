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

    [SerializeField] private float barLerpSpeed = 5f;

    private float displayedScale;

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

    private void Start()
    {
        displayedScale = GameState.Instance.Scale;
    }

    private void Update()
    {
        UpdateProgressBar();
        UpdateOverlays();
    }

    private void UpdateProgressBar()
    {
        displayedScale = Mathf.Lerp(displayedScale, GameState.Instance.Scale, Time.deltaTime * barLerpSpeed);

        // Prevent tiny floating point differences from never reaching the target
        if (Mathf.Abs(displayedScale - GameState.Instance.Scale) < 0.001f)
            displayedScale = GameState.Instance.Scale;

        float angelPercent = (displayedScale + 1f) * 0.5f;
        float demonPercent = 1f - angelPercent;

        balanceAngelicBarFill.style.width = Length.Percent(angelPercent * 100f);
        balanceDemonicBarFill.style.width = Length.Percent(demonPercent * 100f);

        balancePointsLabel.text = $"Balance: {displayedScale:0.00}";

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
