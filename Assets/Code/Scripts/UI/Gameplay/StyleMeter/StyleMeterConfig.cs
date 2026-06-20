using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Style Meter/Config")]
public class StyleMeterConfig : ScriptableObject
{
    public List<StyleRankData> ranks;

    public float decayDelay = 2f;

    public float normalDecay = 100f;

    public float floorDecay = 15f;

    public float noEnemyDecayMultiplier = 0.25f;

    public float maxStyle = 10000f;
}