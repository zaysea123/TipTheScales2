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

    private VisualElement fillStyleBar;

    private Label stylePointsLabel;

    private VisualElement styleFeed;

    private Label freshnessLabel;

    private readonly List<StyleFeedEntry> activeEntries = new();

    private EventBinding<StyleGainEvent> styleGainBinding;
    private EventBinding<FreshnessChangedEvent> freshnessChangedBinding;

    private void Awake()
    {
        var root = document.rootVisualElement;

        rankLabel = root.Q<Label>("RankLabel");
        fillStyleBar = root.Q<VisualElement>("FillStyleBar");
        stylePointsLabel = root.Q<Label>("StylePointsLabel");
        styleFeed = root.Q<VisualElement>("StyleFeed");
        freshnessLabel = root.Q<Label>("FreshnessLabel");
    }

    private void Start()
    {
        freshnessLabel.text = $"{styleMeter.CurrentFreshness:0.0}x {styleMeter.FreshnessState}";
    }

    private void OnEnable()
    {
        styleGainBinding = new EventBinding<StyleGainEvent>(OnStyleGain);
        EventBus<StyleGainEvent>.Register(styleGainBinding);

        freshnessChangedBinding = new EventBinding<FreshnessChangedEvent>(OnFreshnessChanged);
        EventBus<FreshnessChangedEvent>.Register(freshnessChangedBinding);
    }

    private void OnDisable()
    {
        EventBus<StyleGainEvent>.Deregister(styleGainBinding);
        EventBus<FreshnessChangedEvent>.Deregister(freshnessChangedBinding);
    }

    private void Update()
    {
        rankLabel.text = styleMeter.CurrentRank.ToString();
        rankLabel.style.color = GetRankColor(styleMeter.CurrentRank);

        UpdateProgressBar();

        UpdateFeed();
    }

    private StyleColor GetRankColor(StyleRank rank)
    {
        return rank switch
        {
            StyleRank.F => (StyleColor)Color.gray,
            StyleRank.E => (StyleColor)Color.gray,
            StyleRank.D => (StyleColor)Color.green,
            StyleRank.C => (StyleColor)Color.cyan,
            StyleRank.B => (StyleColor)Color.blue,
            StyleRank.A => (StyleColor)Color.purple,
            StyleRank.S => (StyleColor)Color.orange,
            StyleRank.SS => (StyleColor)Color.red,
            StyleRank.SSS => (StyleColor)Color.gold,
            StyleRank.ImTippingIt => (StyleColor)Color.lightGoldenRodYellow,
            _ => (StyleColor)Color.gray,
        };
    }

    private void UpdateProgressBar()
    {
        if (styleMeter.CurrentRank == StyleRank.ImTippingIt)
        {
            fillStyleBar.style.width = Length.Percent(100);
            stylePointsLabel.text = "Style Points: MAX";
            return;
        }

        float currentThreshold = styleMeter.GetCurrentRankThreshold();
        float nextThreshold = styleMeter.GetNextRankThreshold();

        float pointsIntoRank = styleMeter.CurrentStyle - currentThreshold;
        float pointsRequired = nextThreshold - currentThreshold;
        float progress = pointsRequired <= 0 ? 1f : pointsIntoRank / pointsRequired;

        progress = Mathf.Clamp01(progress);

        fillStyleBar.style.width = Length.Percent(progress * 100f);
        stylePointsLabel.text = $"Style Points: {Mathf.FloorToInt(pointsIntoRank)}/{Mathf.FloorToInt(pointsRequired)}";
    }

    private void OnStyleGain(StyleGainEvent e)
    {
        Label label = new()
        {
            text = $"+{e.Amount * styleMeter.CurrentFreshness:0} {e.Reason}"
        };

        label.style.color = e.TextColor;
        label.style.fontSize = 40f;

        styleFeed.Insert(0, label);

        activeEntries.Add(
            new StyleFeedEntry
            {
                Element = label,
                RemainingTime = feedLifetime
            });
    }

    private void OnFreshnessChanged(FreshnessChangedEvent e)
    {
        freshnessLabel.text = $"{e.Multiplier:0.0}x {styleMeter.FreshnessState}";
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