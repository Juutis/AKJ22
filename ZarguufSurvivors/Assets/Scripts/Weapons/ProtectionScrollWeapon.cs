using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class ProtectionScrollWeapon : MonoBehaviour
{
    [SerializeField]
    private float cooldown;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private int maxProjectiles;
    [SerializeField]
    private float projectileDistance;

    private float lastShoot;
    private PlayerMovement player;

    private ProjectilePool pool;

    private List<StaticProjectile> projectiles = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastShoot = 0;
        player = transform.parent.GetComponent<PlayerMovement>();

        pool = ProjectilePoolManager.main.GetPool(ProjectileType.ProtectionScroll);
        pool.SetContainer(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastShoot >= cooldown && projectiles.Count < maxProjectiles)
        {
            Shoot();
        }
        
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        GameObject newProjectile = pool.Get();
        StaticProjectile projectile = newProjectile.GetComponent<StaticProjectile>();
        projectiles.Add(projectile);

        for (int i = 0; i < projectiles.Count; i++) {
            projectiles[i].transform.position = player.transform.position + Quaternion.Euler(0, 0, 360 / projectiles.Count * i) * Vector2.up * projectileDistance;
        }

        lastShoot = Time.time;
    }
}
