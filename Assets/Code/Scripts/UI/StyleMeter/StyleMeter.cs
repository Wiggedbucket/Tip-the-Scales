using UnityEngine;

public class StyleMeter : MonoBehaviour
{
    [SerializeField]
    private StyleMeterConfig config;

    public float CurrentStyle { get; private set; }

    public StyleRank CurrentRank { get; private set; }

    public float DamageMultiplier
    {
        get
        {
            return GetCurrentRankData().damageMultiplier;
        }
    }

    public bool HasEnemiesAlive { get; set; } = true;

    private float lastGainTime;

    private bool decayPaused;

    private EventBinding<StyleGainEvent> styleGainEventBinding;

    private void OnEnable()
    {
        styleGainEventBinding = new EventBinding<StyleGainEvent>(GainStyle);
        EventBus<StyleGainEvent>.Register(styleGainEventBinding);
    }

    private void OnDisable()
    {
        EventBus<StyleGainEvent>.Deregister(styleGainEventBinding);
    }

    private StyleRankData GetCurrentRankData()
    {
        foreach (var rank in config.ranks)
        {
            if (rank.rank == CurrentRank)
                return rank;
        }

        return config.ranks[0];
    }

    private void Update()
    {
        UpdateDecay();

        UpdateRank();
    }

    private void GainStyle(StyleGainEvent e)
    {
        CurrentStyle += e.Amount;

        CurrentStyle = Mathf.Clamp(CurrentStyle, 0, config.maxStyle);

        lastGainTime = Time.time;

        // TODO: Show e.Reason in the style meter list
    }

    private void UpdateDecay()
    {
        if (decayPaused)
            return;

        if (Time.time < lastGainTime + config.decayDelay)
            return;

        float decay = GetCurrentDecay();

        CurrentStyle -= decay * Time.deltaTime;

        CurrentStyle = Mathf.Max(CurrentStyle, 0);
    }

    private float GetCurrentDecay()
    {
        float floor = GetFloorThreshold();

        float decay =
            CurrentStyle > floor
                ? config.normalDecay
                : config.floorDecay;

        if (!HasEnemiesAlive)
        {
            decay *= config.noEnemyDecayMultiplier;
        }

        return decay;
    }

    private float GetFloorThreshold()
    {
        switch (CurrentRank)
        {
            case StyleRank.ImTippingIt:
                return GetThreshold(StyleRank.SSS);

            case StyleRank.SSS:
                return GetThreshold(StyleRank.SS);

            case StyleRank.SS:
                return GetThreshold(StyleRank.S);

            case StyleRank.S:
                return GetThreshold(StyleRank.A);

            case StyleRank.A:
                return GetThreshold(StyleRank.B);

            case StyleRank.B:
                return GetThreshold(StyleRank.C);

            default:
                return 0;
        }
    }

    private void UpdateRank()
    {
        StyleRank rank = StyleRank.F;

        foreach (var data in config.ranks)
        {
            if (CurrentStyle >= data.threshold)
            {
                rank = data.rank;
            }
        }

        CurrentRank = rank;
    }

    private float GetThreshold(StyleRank rank)
    {
        foreach (var data in config.ranks)
        {
            if (data.rank == rank)
                return data.threshold;
        }

        return 0;
    }

    public void PauseDecay()
    {
        decayPaused = true;
    }

    public void ResumeDecay()
    {
        decayPaused = false;
    }
}