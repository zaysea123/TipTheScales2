using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Horde System/Enemy Registry")]
public class EnemyRegistry : ScriptableObject
{
    public List<EnemyData> enemies;

    public EnemyData GetEnemy(int index)
    {
        if (index < 0 || index >= enemies.Count)
            return default;

        return enemies[index];
    }
}