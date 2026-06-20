using UnityEngine;

public static class EnemyRegistryDatabase
{
    private static EnemyRegistry registry;

    public static EnemyRegistry Registry
    {
        get
        {
            if (registry == null)
            {
                registry = Resources.Load<EnemyRegistry>("EnemyRegistry");

#if UNITY_EDITOR
                if (registry == null)
                {
                    Debug.LogError(
                        "EnemyRegistry asset missing. Create one in Assets/Resources/"
                    );
                }
#endif
            }

            return registry;
        }
    }

    public static EnemyData GetEnemy(int index)
    {
        if (Registry == null) return default;
        return Registry.GetEnemy(index);
    }
}