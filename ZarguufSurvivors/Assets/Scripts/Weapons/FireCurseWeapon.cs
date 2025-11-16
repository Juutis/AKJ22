using UnityEngine;

public class FireCurseWeapon : MonoBehaviour
{
    [SerializeField]
    private float cooldown;
    [SerializeField]
    private float projectileRange;
    [SerializeField]
    private float lifetime;
    [SerializeField]
    private float dotCooldown;
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
        pool = ProjectilePoolManager.main.GetPool(ProjectileType.FireCurseProjectile);
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
        Collider2D enemy = Physics2D.OverlapCircle(player.transform.position, projectileRange, LayerMask.GetMask("Enemy Damage Receiver"));

        if (enemy != null)
        {
            GameObject newProjectile = pool.Get();
            FollowEnemyProjectile projectile = newProjectile.GetComponent<FollowEnemyProjectile>();

            if (projectile == null)
            {
                Debug.LogError("Couldn't get ChainProjectile from pool");
                pool.Kill(newProjectile);
            }

            projectile.Init(enemy.transform, dotCooldown);
        }

        lastShoot = Time.time;
    }
}
