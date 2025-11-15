using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class UIPoppingTextPool : MonoBehaviour
{
    public static UIPoppingTextPool main;

    [SerializeField]
    private UIPoppingText uiPoppingTextPrefab;

    [SerializeField]
    private Transform poolContainer;
    [SerializeField]
    private int defaultCapacity = 50;
    [SerializeField]
    private int objectsToInitializeAtStart = 50;
    [SerializeField]
    private int maxSize = 1000;
    private IObjectPool<UIPoppingText> pool;


    void Awake()
    {
        // Create a pool with the four core callbacks.
        pool = new ObjectPool<UIPoppingText>(
            createFunc: CreateItem,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: true,   // helps catch double-release mistakes
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
        var initializedObjects = new List<UIPoppingText>();
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

    public UIPoppingText Get()
    {
        return pool.Get();
    }

    public void Kill(UIPoppingText mob)
    {
        pool.Release(mob);
    }

    // Creates a new pooled GameObject the first time (and whenever the pool needs more).
    private UIPoppingText CreateItem()
    {
        UIPoppingText uiPoppingText = Instantiate(uiPoppingTextPrefab, poolContainer);
        uiPoppingText.name = "UIPoppingText (fromPool)";
        uiPoppingText.gameObject.SetActive(false);
        return uiPoppingText;
    }

    // Called when an item is taken from the pool.
    private void OnGet(UIPoppingText uiPoppingText)
    {
        uiPoppingText.gameObject.SetActive(true);
    }

    // Called when an item is returned to the pool.
    private void OnRelease(UIPoppingText uiPoppingText)
    {
        uiPoppingText.transform.SetParent(poolContainer);
        uiPoppingText.gameObject.SetActive(false);
    }

    // Called when the pool decides to destroy an item (e.g., above max size).
    private void OnDestroyItem(UIPoppingText uiPoppingText)
    {
        Destroy(uiPoppingText.gameObject);
    }
}