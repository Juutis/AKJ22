using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChainProjectile : MonoBehaviour
{
    [SerializeField]
    private ProjectileType projectileType;
    [SerializeField]
    private LineRenderer lineRenderer;

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

    public void Init(ProjectilePool pool, Transform start, Transform target, float lifetime, float jumpRange, float jumpDelay, int jumps, List<Transform> previousTargets)
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
        Debug.Log($"Init {transform.name}");
        inited = true;
        hasJumped = false;
    }

    void OnEnable()
    {
        lineRenderer.SetPositions(new Vector3[] { new Vector3(9999, 9999), new Vector3(9999, 9999) });
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!inited)
        {
            return;
        }

        // TODO: start or target == null
        lineRenderer.SetPositions(new Vector3[] { start.position, target.position });

        if (!hasJumped && (Time.time - startTime > jumpDelay) && (jumps > 0))
        {
            Jump();
        }

        if (Time.time - startTime > lifetime)
        {
            Debug.Log($"Died {transform.name}");

            if (target.gameObject.TryGetComponent<Damageable>(out Damageable enemy))
            {
                var damageToDo = 1;
                enemy.Hurt(damageToDo);
                UIManager.main.ShowPoppingText($"{damageToDo}", Color.red, transform.position);
            }

            Kill();
        }
    }

    private void Jump()
    {
        hasJumped = true;

        // TODO: Closest enemy?
        Collider2D[] enemies = Physics2D.OverlapCircleAll(start.position, jumpRange, LayerMask.GetMask("Enemy Damage Receiver"));

        if (enemies.Length == 0)
        {
            return;
        }

        foreach (Collider2D enemy in enemies)
        {
            if (previousTargets.Contains(enemy.transform))
            {
                continue;
            }

            previousTargets.Add(enemy.transform);

            if (enemy != null)
            {
                GameObject newProjectile = pool.Get();
                ChainProjectile projectile = newProjectile.GetComponent<ChainProjectile>();

                if (projectile == null)
                {
                    Debug.LogError("Couldn't get ChainProjectile from pool");
                    pool.Kill(newProjectile);
                }

                projectile.Init(pool, target, enemy.transform, lifetime, jumpRange, jumpDelay, jumps - 1, previousTargets);
            }

            break;
        }
    }

    public void Kill()
    {
        inited = false;
        pool.Kill(gameObject);
    }
}
