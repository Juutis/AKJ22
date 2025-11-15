using UnityEngine;

public class SpawnableMob : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private MoveTowardsPlayerEnemy mtpEnemy;
    private Damageable damageable;
    private SpriteAnimator spriteAnim;
    private EnemyConfig config;

    public void SetPosition(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    public void Create()
    {
        mtpEnemy = GetComponent<MoveTowardsPlayerEnemy>();
        damageable = GetComponentInChildren<Damageable>();
        spriteAnim = GetComponentInChildren<SpriteAnimator>();
    }

    public void Initialize(EnemyConfig config, int mobIndex, int waveIndex, Transform parent)
    {
        name = $"Mob[W{waveIndex} - {mobIndex}]";
        transform.parent = parent;

        this.config = config;
        mtpEnemy.Init(config);
        damageable.Init(config.Health);
        spriteAnim.Init(config.Sprites);
    }

    public void Begin()
    {
        // set up mob stats like health, xp drops etc based on some config
    }

    public void Kill()
    {
        // reset mob stats like health etc. here
        SpawnableMobPool.main.Kill(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
