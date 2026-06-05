using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StyleMeterUI : MonoBehaviour
{
    [SerializeField]
    private UIDocument document;

    [SerializeField]
    private StyleMeter styleMeter;

    [SerializeField]
    private float feedLifetime = 3f;

    private Label rankLabel;

    private ProgressBar styleBar;

    private VisualElement styleFeed;

    private readonly List<StyleFeedEntry> activeEntries = new();

    private EventBinding<StyleGainEvent> styleGainBinding;

    private void Awake()
    {
        var root = document.rootVisualElement;

        rankLabel = root.Q<Label>("RankLabel");
        styleBar = root.Q<ProgressBar>("StyleBar");
        styleFeed = root.Q<VisualElement>("StyleFeed");
    }

    private void OnEnable()
    {
        styleGainBinding = new EventBinding<StyleGainEvent>(OnStyleGain);
        EventBus<StyleGainEvent>.Register(styleGainBinding);
    }

    private void OnDisable()
    {
        EventBus<StyleGainEvent>.Deregister(styleGainBinding);
    }

    private void Update()
    {
        rankLabel.text = styleMeter.CurrentRank.ToString();

        styleBar.value = styleMeter.CurrentStyle;

        UpdateFeed();
    }

    private void OnStyleGain(StyleGainEvent e)
    {
        Label label = new()
        {
            text = $"+{e.Amount:0} {e.Reason}"
        };

        label.style.color = e.TextColor;

        styleFeed.Insert(0, label);

        activeEntries.Add(
            new StyleFeedEntry
            {
                Element = label,
                RemainingTime = feedLifetime
            });
    }

    private void UpdateFeed()
    {
        for (int i = activeEntries.Count - 1; i >= 0; i--)
        {
            StyleFeedEntry entry = activeEntries[i];

            entry.RemainingTime -= Time.deltaTime;

            if (entry.RemainingTime <= 0)
            {
                entry.Element.RemoveFromHierarchy();

                activeEntries.RemoveAt(i);
            }
        }
    }
}