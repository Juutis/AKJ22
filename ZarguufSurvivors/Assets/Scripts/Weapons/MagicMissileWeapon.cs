using System;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissileWeapon : MonoBehaviour
{
    [SerializeField]
    private List<MagicMissileLevel> levels;

    private float lastShoot;
    private PlayerMovement player;
    private MagicMissileLevel currentLevel;

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
        if (!SkillManager.main.IsSkillActive(SkillType.MagicMissile))
        {
            return;
        }

        currentLevel = levels[Mathf.Min(levels.Count - 1, SkillManager.main.GetSkillLevel(SkillType.MagicMissile))];

        float currentCooldown = currentLevel.cooldown * SkillManager.main.GetAttackCooldownMultiplier();
        
        if (Time.time - lastShoot >= currentCooldown)
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
        GameObject newProjectile = ProjectilePoolManager.main.GetPool(ProjectileType.MagicMissile).Get();
        newProjectile.GetComponent<StraightFlyingProjectile>().Init(player.MoveDir, currentLevel.projectileSpeed, currentLevel.damage, currentLevel.hitCount);

        Vector2 randomPos2 = UnityEngine.Random.insideUnitCircle.normalized * 0.25f;
        Vector3 randomPos = new Vector3(randomPos2.x, randomPos2.y, 0);
        Vector3 offsetPos = new Vector3(player.MoveDir.x, player.MoveDir.y, 0) * 0.5f;

        newProjectile.transform.position = transform.position + offsetPos + randomPos;
        lastShoot = Time.time;
        SoundManager.main.PlaySound(GameSoundType.ShootProjectile);
    }
}

[Serializable]
public class MagicMissileLevel
{
    public float cooldown;
    public float projectileSpeed;
    public float damage;
    public int hitCount;
}
