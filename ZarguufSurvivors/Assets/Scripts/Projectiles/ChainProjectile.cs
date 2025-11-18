using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChainProjectile : MonoBehaviour
{
    [SerializeField]
    private ProjectileType projectileType;

    [SerializeField]
    private ParticleSystem effect;

    private ProjectilePool pool;
    private float jumpRange;
    private int jumps;
    private float jumpDelay;
    private float startTime;
    private bool hasJumped;
    private Transform start;
    private Transform target;
    private float lifetime;
    private List<Transform> previousTargets;
    private bool inited = false;
    private float damage;
    private bool isKilled = false;

    public void Init(ProjectilePool pool, Transform start, Transform target, float lifetime, float jumpRange, float jumpDelay, float damage, int jumps, List<Transform> previousTargets)
    {
        this.pool = pool;
        this.jumpRange = jumpRange;
        this.jumps = jumps;
        this.jumpDelay = jumpDelay;
        startTime = Time.time;
        this.start = start;
        this.target = target;
        this.lifetime = lifetime;
        this.previousTargets = previousTargets;
        inited = true;
        hasJumped = false;
        this.damage = damage;
        PlayEffect();
    }

    void OnEnable()
    {
        this.inited = false;
        this.isKilled = false;
        this.hasJumped = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!inited || isKilled)
        {
            return;
        }

        if (!hasJumped && (Time.time - startTime > jumpDelay) && (jumps > 0))
        {
            if (target.gameObject.TryGetComponent<Damageable>(out Damageable enemy))
            {
                enemy.Hurt(damage);
                UIManager.main.ShowPoppingText($"{damage}", Color.red, target.position);
            }

            Jump();
        }

        if (Time.time - startTime > lifetime && !isKilled)
        {
            Kill();
        }
    }

    private void PlayEffect()
    {
        effect.transform.position = start.position;
        effect.Emit(3);
        effect.transform.position = Vector2.Lerp(start.position, target.position, 0.25f);
        effect.Emit(3);
        effect.transform.position = Vector2.Lerp(start.position, target.position, 0.5f);
        effect.Emit(3);
        effect.transform.position = Vector2.Lerp(start.position, target.position, 0.75f);
        effect.Emit(3);
        effect.transform.position = target.position;
        effect.Emit(3);
    }

    private void Jump()
    {
        hasJumped = true;

        // TODO: Closest enemy?
        if (start == null)
        {
            return;
        }
        Collider2D[] enemies = Physics2D.OverlapCircleAll(start.position, jumpRange, LayerMask.GetMask("Enemy Damage Receiver"));

        if (enemies.Length == 0)
        {
            return;
        }

        foreach (Collider2D enemy in enemies)
        {
            if (previousTargets.Any(x => x.transform.gameObject.GetInstanceID() == enemy.gameObject.GetInstanceID()))
            {
                continue;
            }

            if (enemy != null)
            {
                previousTargets.Add(enemy.transform);
                GameObject newProjectile = pool.Get();
                ChainProjectile projectile = newProjectile.GetComponent<ChainProjectile>();

                if (projectile == null)
                {
                    Debug.LogError("Couldn't get ChainProjectile from pool");
                    pool.Kill(newProjectile);
                }

                projectile.Init(pool, target, enemy.transform, lifetime, jumpRange, jumpDelay, damage, jumps - 1, previousTargets);
            }

            break;
        }
    }

    public void Kill()
    {
        isKilled = true;
        inited = false;
        pool.Kill(gameObject);
    }
}
