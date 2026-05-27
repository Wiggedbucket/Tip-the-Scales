using System;
using UnityEngine;

[Serializable]
public struct EnemyData
{
    public GameObject enemy;
    public EnemyTier tier;
    public int weight;
    public int cost;

    public int minimumWave;
    public int maximumWave;

    [Tooltip("The chance of the enemy appearing between the min and max wave.\nAfter the max wave, the last chance point will be held for future waves.")]
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