using System.Collections.Generic;
using UnityEngine;

public class ProtectionScrollWeapon : MonoBehaviour, IWeapon
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
        player = transform.parent.parent.GetComponent<PlayerMovement>();

        MoveContainer();
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
        lastShoot = Time.time;
        GameObject newProjectile = pool.Get();
        StaticProjectile projectile = newProjectile.GetComponent<StaticProjectile>();

        if (projectile == null)
        {
            Debug.LogError("Couldn't get a projectile");
            return;
        }

        projectile.Init(this);
        projectiles.Add(projectile);

        for (int i = 0; i < projectiles.Count; i++)
        {
            projectiles[i].transform.position = player.transform.position + Quaternion.Euler(0, 0, 360 / projectiles.Count * i) * Vector2.up * projectileDistance;
        }
    }

    private void MoveContainer()
    {
        pool = ProjectilePoolManager.main.GetPool(ProjectileType.ProtectionScroll);
        pool.SetContainer(transform);
    }

    public void Kill(GameObject obj)
    {
        projectiles.Remove(obj.GetComponent<StaticProjectile>());
        ProjectilePoolManager.main.GetPool(ProjectileType.ProtectionScroll).Kill(obj);
    }
}
