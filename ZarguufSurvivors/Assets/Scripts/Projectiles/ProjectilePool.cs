using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    public ProjectileType PoolType { get { return poolType; } }

    [SerializeField]
    private ProjectileType poolType;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform poolContainer;
    [SerializeField]
    private int defaultCapacity;
    [SerializeField]
    private int objectsToInitializeAtStart;
    [SerializeField]
    private int maxSize;
    private IObjectPool<GameObject> pool;

    private Transform currentPool;

    void Awake()
    {
        currentPool = poolContainer;

        pool = new ObjectPool<GameObject>(
            createFunc: CreateItem,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        List<GameObject> initializedObjects = new();

        for (int i = 0; i < objectsToInitializeAtStart; i += 1)
        {
            initializedObjects.Add(pool.Get());
        }

        foreach (GameObject obj in initializedObjects)
        {
            pool.Release(obj);
        }
    }

    public GameObject Get()
    {
        return pool.Get();
    }

    public void SetContainer(Transform container)
    {
        foreach (Transform t in currentPool)
        {
            t.parent = container;
        }

        currentPool = container;
    }

    public void Kill(GameObject projectile)
    {
        pool.Release(projectile);
    }

    private GameObject CreateItem()
    {
        GameObject item = Instantiate(projectilePrefab, currentPool);
        item.name = "Projectile (fromPool)";
        item.gameObject.SetActive(false);
        return item;
    }

    private void OnGet(GameObject projectile)
    {
        projectile.SetActive(true);
    }

    private void OnRelease(GameObject projectile)
    {
        projectile.transform.parent = currentPool;
        projectile.SetActive(false);
    }

    private void OnDestroyItem(GameObject projectile)
    {
        Destroy(projectile);
    }
}

public enum ProjectileType
{
    MagicMissile,
    Fireball,
    ProtectionScroll
}