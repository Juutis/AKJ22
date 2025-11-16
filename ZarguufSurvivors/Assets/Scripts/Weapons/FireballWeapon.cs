using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireballWeapon : MonoBehaviour
{
    [SerializeField]
    private List<FireballLevel> levels;

    private float lastShoot;
    private PlayerMovement player;
    private FireballLevel currentLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLevel = levels[0];
        lastShoot = 0;
        player = transform.parent.parent.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!SkillManager.main.IsSkillActive(SkillType.Fireball))
        {
            return;
        }

        currentLevel = levels[Mathf.Min(levels.Count - 1, SkillManager.main.GetSkillLevel(SkillType.Fireball))];
        float currentCooldown = currentLevel.cooldown * SkillManager.main.GetAttackCooldownMultiplier();

        if (Time.time - lastShoot >= currentLevel.cooldown)
        {
            int currentProjectileCount = 1 + SkillManager.main.GetProjectileCountAddition();

            for (int i = 0; i < currentProjectileCount; i++)
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, 6f, LayerMask.GetMask("Enemy Damage Receiver"));
        Collider2D enemy = enemies?.OrderBy(x => System.Guid.NewGuid())?.FirstOrDefault();

        if (enemy == null)
        {
            lastShoot = Time.time;
            return;
        }

        GameObject newProjectile = ProjectilePoolManager.main.GetPool(ProjectileType.Fireball).Get();
        float currentDamage = currentLevel.damage * SkillManager.main.GetAttackDamageMultiplier();

        Vector2 randomPos2 = UnityEngine.Random.insideUnitCircle.normalized * 0.2f;
        Vector3 randomPos = new Vector3(randomPos2.x, randomPos2.y, 0);
        Vector3 offsetPos = new Vector3(player.MoveDir.x, player.MoveDir.y, 0) * 0.5f;
        Vector3 start3 = transform.position + offsetPos + randomPos;
        Vector2 start = new(start3.x, start3.y);
        Vector2 target = new(enemy.transform.position.x, enemy.transform.position.y);
        Vector2 dir = (target - start).normalized;

        newProjectile.GetComponent<StraightFlyingProjectile>().Init(dir, currentLevel.projectileSpeed, currentDamage, currentLevel.hitCount);

        newProjectile.transform.position = transform.position + offsetPos + randomPos;
        lastShoot = Time.time;
    }
}

[Serializable]
public class FireballLevel
{
    public float cooldown;
    public float projectileSpeed;
    public float damage;
    public int hitCount;
}