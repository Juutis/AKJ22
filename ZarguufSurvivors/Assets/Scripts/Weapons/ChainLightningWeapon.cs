using System;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightningWeapon : MonoBehaviour
{
    [SerializeField]
    private float lifetime;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private List<ChainLightningLevel> levels;

    private ChainLightningLevel currentLevel;
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
        currentLevel = levels[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (!SkillManager.main.isSkillActive(SkillType.ChainLightningProjectile))
        {
            return;
        }

        currentLevel = levels[Mathf.Min(levels.Count - 1, SkillManager.main.GetSkillLevel(SkillType.ChainLightningProjectile))];
        if (Time.time - lastShoot >= currentLevel.cooldown)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // TODO: Closest enemy?
        Collider2D enemy = Physics2D.OverlapCircle(player.transform.position, currentLevel.projectileRange, LayerMask.GetMask("Enemy Damage Receiver"));

        if (enemy != null)
        {
            GameObject newProjectile = pool.Get();
            ChainProjectile projectile = newProjectile.GetComponent<ChainProjectile>();

            if (projectile == null)
            {
                Debug.LogError("Couldn't get ChainProjectile from pool");
                pool.Kill(newProjectile);
            }
            projectile.Init(pool, transform, enemy.transform, lifetime, currentLevel.jumpRange, currentLevel.jumpDelay, currentLevel.damage, currentLevel.jumpAmount, new List<Transform> { enemy.transform });
        }

        lastShoot = Time.time;
    }
}

[Serializable]
public class ChainLightningLevel
{
    public float cooldown;
    public float jumpDelay;
    public float projectileRange;
    public float jumpRange;
    public int jumpAmount;
    public float projectileCount;
    public float damage;
}