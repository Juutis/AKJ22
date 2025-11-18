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

    private bool initiated = false;

    [SerializeField]
    private int xpAmount = 5;

    private bool goToPlayer = false;
    private Transform player;

    void Start()
    {
        if (!initiated)
        {
            Initialize(xpAmount, player, transform.parent, transform.position);
        }

    }

    public void Initialize(int xpDrop, Transform playerTransform, Transform parent, Vector2 position)
    {
        xpDropAmount = xpDrop;
        foreach (var tier in tiers)
        {
            if (tier.MinimumXp <= xpDrop)
            {
                xpDropTier = tier;
            }
        }
        xpDropTier.Sprite.SetActive(true);
        initiated = true;
        player = playerTransform;
        XpDropManager.Instance.Drops.Add(this);
        transform.SetParent(parent);
        transform.position = position;
    }

    public void Update()
    {
        if (goToPlayer)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, 10.0f * Time.deltaTime);
        }
    }

    public void Kill()
    {
        XpDropManager.Instance.Drops.Remove(this);
        Destroy(gameObject);
    }

    public void GoToPlayer()
    {
        if (goToPlayer)
        {
            return;
        }
        goToPlayer = true;
    }
}

[System.Serializable]
public class XpDropTier
{
    public GameObject Sprite;
    public int MinimumXp = 0;
}