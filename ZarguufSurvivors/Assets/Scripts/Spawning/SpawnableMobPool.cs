using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnableMobPool : MonoBehaviour
{
    public static SpawnableMobPool main;

    [SerializeField]
    private SpawnableMob spawnableMobPrefab;
    // The pool holds plain GameObjects (you can swap this for any component type).
    [SerializeField]
    private Transform poolContainer;
    [SerializeField]
    private int defaultCapacity = 50;
    [SerializeField]
    private int objectsToInitializeAtStart = 50;
    [SerializeField]
    private int maxSize = 1000;
    private IObjectPool<SpawnableMob> pool;


    void Awake()
    {
        // Create a pool with the four core callbacks.
        pool = new ObjectPool<SpawnableMob>(
            createFunc: CreateItem,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: true,   // helps catch double-release mistakes
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
        var initializedObjects = new List<SpawnableMob>();
        for (int i = 0; i < objectsToInitializeAtStart; i += 1)
        {
            initializedObjects.Add(pool.Get());
        }
        foreach (var obj in initializedObjects)
        {
            pool.Release(obj);
        }
        main = this;
    }

    public SpawnableMob Get()
    {
        return pool.Get();
    }

    public void Kill(SpawnableMob mob)
    {
        try
        {
            pool.Release(mob);
        }
        catch (InvalidOperationException e)
        {
            // was killed already lols
        }
    }

    // Creates a new pooled GameObject the first time (and whenever the pool needs more).
    private SpawnableMob CreateItem()
    {
        SpawnableMob spawnableMob = Instantiate(spawnableMobPrefab, poolContainer);
        spawnableMob.name = "SpawnableMob (fromPool)";
        spawnableMob.Create();
        spawnableMob.gameObject.SetActive(false);
        return spawnableMob;
    }

    // Called when an item is taken from the pool.
    private void OnGet(SpawnableMob spawnableMob)
    {
        spawnableMob.gameObject.SetActive(true);
    }

    // Called when an item is returned to the pool.
    private void OnRelease(SpawnableMob spawnableMob)
    {
        spawnableMob.transform.parent = poolContainer;
        spawnableMob.gameObject.SetActive(false);
    }

    // Called when the pool decides to destroy an item (e.g., above max size).
    private void OnDestroyItem(SpawnableMob spawnableMob)
    {
        Destroy(spawnableMob.gameObject);
    }
}