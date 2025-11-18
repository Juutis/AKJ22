using System.Text.RegularExpressions;
using UnityEngine;

public class SpawnableMob : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private MoveTowardsPlayerEnemy mtpEnemy;
    private Damageable damageable;
    private SpriteAnimator spriteAnim;
    private EnemyConfig config;
    private CircleCollider2D coll;

    public EnemyConfig Config {get {return config;}}

    [SerializeField]
    private XpDrop xpDropPrefab;

    public int GetDamageDoneToPlayer()
    {
        return config.DamageDoneToPlayer;
    }

    public void SetPosition(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    public void Create()
    {
        mtpEnemy = GetComponent<MoveTowardsPlayerEnemy>();
        damageable = GetComponentInChildren<Damageable>();
        spriteAnim = GetComponentInChildren<SpriteAnimator>();
        coll = GetComponent<CircleCollider2D>();
    }

    public void Initialize(EnemyConfig config, int mobIndex, int waveIndex, Transform parent, int indexInGroup, int groupSize)
    {
        name = $"Mob[W{waveIndex} - {mobIndex}]";
        transform.parent = parent;

        this.config = config;
        mtpEnemy.Init(config, indexInGroup, groupSize);
        damageable.Init(config.Health);
        spriteAnim.Init(config.Sprites);
        coll.radius = config.ColliderRadius;
        if (config.IsFlying)
        {
            gameObject.layer = LayerMask.NameToLayer("FlyingEnemy");
            GetComponentInChildren<Renderer>().sortingLayerName = "Flying";
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            GetComponentInChildren<Renderer>().sortingLayerName = "Default";
        }
    }

    public void Begin()
    {
        // set up mob stats like health, xp drops etc based on some config
        mtpEnemy.Begin();
    }

    public void Kill()
    {
        // reset mob stats like health etc. here
        /*var xpDrop = Instantiate(xpDropPrefab);
        xpDrop.transform.position = transform.position;
        xpDrop.Initialize(config.XpDrop);*/
        MessageBus.Publish(new XpDropIsCreatedEvent(config.XpDrop, transform.position));
        SpawnableMobPool.main.Kill(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
