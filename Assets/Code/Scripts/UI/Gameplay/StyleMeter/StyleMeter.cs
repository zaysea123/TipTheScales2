using System.Collections.Generic;
using UnityEngine;

public class StyleMeter : MonoBehaviour
{
    [SerializeField]
    private StyleMeterConfig config;

    public float CurrentStyle { get; private set; }
    public StyleRank CurrentRank { get; private set; }

    public bool hasEnemiesAlive = true;

    public float DamageMultiplier => GetCurrentRankData().damageMultiplier;

    private float lastGainTime;
    private bool decayPaused;

    private string currentWeapon;

    [SerializeField]
    private float maxFreshness = 2f;

    [SerializeField]
    private float minFreshness = 0.5f;

    [SerializeField]
    private float freshnessLossPerShot = 0.1f;

    public float CurrentFreshness { get; private set; } = 2f;

    public string FreshnessState
    {
        get
        {
            if (CurrentFreshness >= 1.5f)
                return "FRESH";

            if (CurrentFreshness >= 1.0f)
                return "NORMAL";

            return "STALE";
        }
    }

    private EventBinding<StyleGainEvent> styleGainEventBinding;
    private EventBinding<WeaponFiredEvent> weaponFiredEventBinding;

    #region Singleton Setup
    public static StyleMeter Instance { get; private set; }
    public static bool InstanceExists => Instance != null;

    void Awake()
    {
        if (InstanceExists)
        {
            Destroy(gameObject);
        }
        else
        {
            // Registers the first valid instance before the rest of the scene startup flow.
            Instance = this;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    #endregion

    private void OnEnable()
    {
        styleGainEventBinding = new EventBinding<StyleGainEvent>(GainStyle);
        EventBus<StyleGainEvent>.Register(styleGainEventBinding);

        weaponFiredEventBinding = new EventBinding<WeaponFiredEvent>(UpdateFreshness);
        EventBus<WeaponFiredEvent>.Register(weaponFiredEventBinding);
    }

    private void OnDisable()
    {
        EventBus<StyleGainEvent>.Deregister(styleGainEventBinding);
        EventBus<WeaponFiredEvent>.Deregister(weaponFiredEventBinding);
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

    public float GetCurrentRankThreshold()
    {
        foreach (var rank in config.ranks)
        {
            if (rank.rank == CurrentRank)
                return rank.threshold;
        }

        return 0;
    }

    public float GetNextRankThreshold()
    {
        for (int i = 0; i < config.ranks.Count; i++)
        {
            if (config.ranks[i].rank == CurrentRank)
            {
                if (i + 1 < config.ranks.Count)
                    return config.ranks[i + 1].threshold;

                return config.maxStyle;
            }
        }

        return config.maxStyle;
    }

    private void GainStyle(StyleGainEvent e)
    {
        CurrentStyle += e.Amount * CurrentFreshness;

        CurrentStyle = Mathf.Clamp(CurrentStyle, 0, config.maxStyle);

        lastGainTime = Time.time;
    }

    private void UpdateFreshness(WeaponFiredEvent e)
    {
        // Weapon swap
        if (currentWeapon != e.Weapon)
        {
            currentWeapon = e.Weapon;
            CurrentFreshness = maxFreshness;

            EventBus<FreshnessChangedEvent>.Raise(new FreshnessChangedEvent
            {
                Multiplier = CurrentFreshness
            });

            return;
        }

        // Same weapon used again
        CurrentFreshness -= freshnessLossPerShot;
        CurrentFreshness = Mathf.Clamp(CurrentFreshness, minFreshness, maxFreshness);

        EventBus<FreshnessChangedEvent>.Raise(new FreshnessChangedEvent
        {
            Multiplier = CurrentFreshness
        });
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

        if (!hasEnemiesAlive)
        {
            decay *= config.noEnemyDecayMultiplier;
        }

        return decay;
    }

    private float GetFloorThreshold()
    {
        return CurrentRank switch
        {
            StyleRank.ImTippingIt => GetThreshold(StyleRank.SSS),
            StyleRank.SSS => GetThreshold(StyleRank.SS),
            StyleRank.SS => GetThreshold(StyleRank.S),
            StyleRank.S => GetThreshold(StyleRank.A),
            StyleRank.A => GetThreshold(StyleRank.B),
            StyleRank.B => GetThreshold(StyleRank.C),
            _ => 0,
        };
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