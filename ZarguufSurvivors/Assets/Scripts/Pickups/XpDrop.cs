using System.Collections.Generic;
using UnityEngine;

public class XpDrop : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private List<XpDropTier> tiers = new();
    
    private XpDropTier xpDropTier;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private int xpDropAmount;
    public int XpDropAmount {get {return xpDropAmount;}}

    void Start()
    {
        #if UNITY_EDITOR
        Initialize(10);
        #endif
    }

    public void Initialize(int xpDrop)
    {
        xpDropAmount = xpDrop;
        foreach (var tier in tiers)
        {
            if (tier.MinimumXp <= xpDrop)
            {
                xpDropTier = tier;
            }
        }
        spriteRenderer.sprite = xpDropTier.Sprite;
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}

[System.Serializable]
public class XpDropTier
{
    public Sprite Sprite;
    public int MinimumXp = 0;
}