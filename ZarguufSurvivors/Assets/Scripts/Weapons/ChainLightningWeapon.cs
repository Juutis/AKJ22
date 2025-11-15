using UnityEngine;

public class ChainLightningWeapon : MonoBehaviour
{
    [SerializeField]
    private float cooldown;
    [SerializeField]
    private float jumpDelay;
    [SerializeField]
    private float projectileRange;
    [SerializeField]
    private float jumpRange;
    [SerializeField]
    private int jumpAmount;
    [SerializeField]
    private float lifetime;
    [SerializeField]
    private GameObject projectilePrefab;

    private float lastShoot;
    private float startTime;
    private PlayerMovement player;
    private ProjectilePool pool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastShoot = 0;
        player = transform.parent.parent.GetComponent<PlayerMovement>();
        pool = ProjectilePoolManager.main.GetPool(ProjectileType.ChainLightningProjectile);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastShoot >= cooldown)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // TODO: Closest enemy?
        Collider2D enemy = Physics2D.OverlapCircle(player.transform.position, projectileRange, LayerMask.GetMask("Enemy Damage Receiver"));

        if (enemy != null)
        {
            GameObject newProjectile = pool.Get();
            ChainProjectile projectile = newProjectile.GetComponent<ChainProjectile>();

            if (projectile == null)
            {
                Debug.LogError("Couldn't get ChainProjectile from pool");
                pool.Kill(newProjectile);
            }
            projectile.Init(pool, transform, enemy.transform, lifetime, jumpRange, jumpDelay, jumpAmount, new());
        }

        lastShoot = Time.time;
    }
}
