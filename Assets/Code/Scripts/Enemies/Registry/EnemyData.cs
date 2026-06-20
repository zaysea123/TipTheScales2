using System;
using UnityEngine;

[Serializable]
public struct EnemyData
{
    [Header("Prefab")]
    public GameObject enemy;

    [Header("Classification")]
    public EnemyTier tier;

    [Header("Balancing")]
    public int cost;
    public int weight;

    [Header("Wave Progression")]

    [Tooltip("Wave where this enemy starts appearing.")]
    public int startWave;

    [Tooltip("Wave where the appearance curve reaches its end.")]
    public int curveEndWave;

    [Tooltip("Optional wave where the enemy stops spawning entirely. 0 = never.")]
    public int stopSpawningWave;

    [Tooltip(
        "Controls spawn chance progression between Start Wave and Curve End Wave.\n" +
        "After Curve End Wave, the final curve value is held."
    )]
    public AnimationCurve appearanceChance;
}

public enum EnemyTier
{
    Knight,
    Baron,
    Viscount,
    Count,
    Duke,
    King,
    Emperor,
}