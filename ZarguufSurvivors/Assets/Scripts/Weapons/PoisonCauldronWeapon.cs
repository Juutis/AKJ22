using System.Collections.Generic;
using UnityEngine;

public class PoisonCauldronWeapon : MonoBehaviour, IWeapon
{
    [SerializeField]
    private float cooldown;
    [SerializeField]
    private float projectileSize;
    [SerializeField]
    private float spawnRange;

    private float lastShoot;
    private PlayerMovement player;

    private ProjectilePool pool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastShoot = 0;
        player = transform.parent.parent.GetComponent<PlayerMovement>();
        pool = ProjectilePoolManager.main.GetPool(ProjectileType.PoisonCauldron);
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
        lastShoot = Time.time;
        GameObject newProjectile = pool.Get();
        StaticProjectile projectile = newProjectile.GetComponent<StaticProjectile>();

        if (projectile == null) {
            Debug.LogError("Couldn't get a projectile");
            return;
        }

        projectile.Init(this, Random.insideUnitCircle * spawnRange, projectileSize);
    }

    public void Kill(GameObject obj)
    {
        ProjectilePoolManager.main.GetPool(ProjectileType.PoisonCauldron).Kill(obj);
    }
}
