using System.Collections.Generic;
using UnityEngine;

public class ProjectilePoolManager : MonoBehaviour
{
    public static ProjectilePoolManager main;

    private Dictionary<ProjectileType, ProjectilePool> pools = new();

    void Awake()
    {
        main = this;

        foreach (Transform t in transform)
        {
            if (t.TryGetComponent<ProjectilePool>(out ProjectilePool pool))
            {
                pools.Add(pool.PoolType, pool);
            }
        }
    }

    public ProjectilePool GetPool(ProjectileType type)
    {
        if (pools.TryGetValue(type, out ProjectilePool pool))
        {
            return pool;
        }
        else
        {
            Debug.LogError($"Couldn't find projectile pool with type {type}!!");
            return null;
        }
    }

}
