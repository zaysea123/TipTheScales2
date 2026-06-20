using UnityEngine;

[System.Serializable]
public class StyleRankData
{
    public StyleRank rank;

    public float threshold;

    [Range(0f, 5f)]
    public float damageMultiplier = 1f;

    [Range(0f, 5f)]
    public float moveSpeedMultiplier = 1f;
}