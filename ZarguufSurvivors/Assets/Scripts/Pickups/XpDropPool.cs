using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class XpDropPool : MonoBehaviour
{
    public static XpDropPool main;

    [SerializeField]
    private XpDrop xpDropPrefab;
    // The pool holds plain GameObjects (you can swap this for any component type).
    [SerializeField]
    private Transform poolContainer;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private int defaultCapacity = 50;
    [SerializeField]
    private int objectsToInitializeAtStart = 50;
    [SerializeField]
    private int maxSize = 1000;
    private IObjectPool<XpDrop> pool;


    void Awake()
    {
        // Create a pool with the four core callbacks.
        pool = new ObjectPool<XpDrop>(
            createFunc: CreateItem,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: true,   // helps catch double-release mistakes
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
        var initializedObjects = new List<XpDrop>();
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

    public XpDrop Get()
    {
        return pool.Get();
    }

    public void Kill(XpDrop mob)
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
    private XpDrop CreateItem()
    {
        XpDrop xpDrop = Instantiate(xpDropPrefab, poolContainer);
        xpDrop.name = "XpDrop (fromPool)";
        xpDrop.gameObject.SetActive(false);
        return xpDrop;
    }

    // Called when an item is taken from the pool.
    private void OnGet(XpDrop xpDrop)
    {
        xpDrop.gameObject.SetActive(true);
    }

    // Called when an item is returned to the pool.
    private void OnRelease(XpDrop xpDrop)
    {
        xpDrop.transform.parent = poolContainer;
        xpDrop.gameObject.SetActive(false);
    }

    // Called when the pool decides to destroy an item (e.g., above max size).
    private void OnDestroyItem(XpDrop xpDrop)
    {
        Destroy(xpDrop.gameObject);
    }

    private void OnEnable()
    {
        MessageBus.Subscribe<XpDropIsCreatedEvent>(OnXpDropIsCreatedEvent);
    }

    private void OnDisable()
    {
        MessageBus.Unsubscribe<XpDropIsCreatedEvent>(OnXpDropIsCreatedEvent);
    }

    private void OnXpDropIsCreatedEvent(XpDropIsCreatedEvent e)
    {
        // Format the time and update the UI text
        var xpDrop = Get();
        xpDrop.Initialize(e.XpDropAmount, playerTransform, poolContainer, e.Position);
    }

}


public struct XpDropIsCreatedEvent : IEvent
{
    public int XpDropAmount { get; }
    public Vector2 Position { get; }

    public XpDropIsCreatedEvent(int xpDropAmount, Vector2 position)
    {
        XpDropAmount = xpDropAmount;
        Position = position;
    }
}