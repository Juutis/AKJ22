using UnityEngine;
using System.Collections.Generic;

public class FireCurseWeapon : MonoBehaviour
{
    [SerializeField]
    private List<FireCurseLevel> levels;

    private float lastShoot;
    private float startTime;
    private PlayerMovement player;
    private ProjectilePool pool;
    private FireCurseLevel currentLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLevel = levels[0];
        lastShoot = 0;
        player = transform.parent.parent.GetComponent<PlayerMovement>();
        pool = ProjectilePoolManager.main.GetPool(ProjectileType.FireCurseProjectile);
    }

    // Update is called once per frame
    void Update()
    {
        if (!SkillManager.main.IsSkillActive(SkillType.FireCurseProjectile))
        {
            return;
        }


        currentLevel = levels[Mathf.Min(levels.Count - 1, SkillManager.main.GetSkillLevel(SkillType.FireCurseProjectile))];
        if (Time.time - lastShoot >= currentLevel.cooldown)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Collider2D enemy = Physics2D.OverlapCircle(player.transform.position, currentLevel.projectileRange, LayerMask.GetMask("Enemy Damage Receiver"));

        if (enemy != null)
        {
            GameObject newProjectile = pool.Get();
            FollowEnemyProjectile projectile = newProjectile.GetComponent<FollowEnemyProjectile>();

            if (projectile == null)
            {
                Debug.LogError("Couldn't get ChainProjectile from pool");
                pool.Kill(newProjectile);
            }

            projectile.Init(enemy.transform, currentLevel.dotCooldown, currentLevel.damage);
        }

        lastShoot = Time.time;
    }
}

[System.Serializable]
public class FireCurseLevel
{
    public float cooldown;
    public float projectileRange;
    public float lifetime;
    public float dotCooldown;
    public float damage;
}
