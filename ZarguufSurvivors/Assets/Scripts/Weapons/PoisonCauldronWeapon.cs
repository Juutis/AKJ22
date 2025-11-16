using System.Collections.Generic;
using UnityEngine;

public class PoisonCauldronWeapon : MonoBehaviour, IWeapon
{
    [SerializeField]
    private List<PoisonCauldronLevel> levels;

    private float lastShoot;
    private PlayerMovement player;

    private ProjectilePool pool;
    private PoisonCauldronLevel currentLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLevel = levels[0];
        lastShoot = 0;
        player = transform.parent.parent.GetComponent<PlayerMovement>();
        pool = ProjectilePoolManager.main.GetPool(ProjectileType.PoisonCauldron);
    }

    // Update is called once per frame
    void Update()
    {
        if (!SkillManager.main.IsSkillActive(SkillType.PoisonCauldron))
        {
            return;
        }

        currentLevel = levels[Mathf.Min(levels.Count - 1, SkillManager.main.GetSkillLevel(SkillType.PoisonCauldron))];
        if (Time.time - lastShoot >= currentLevel.cooldown)
        {
            Shoot();
        }
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

        projectile.Init(this, new Vector2(player.transform.position.x, player.transform.position.y) + Random.insideUnitCircle * currentLevel.spawnRange, currentLevel.projectileSize, currentLevel.damage);
    }

    public void Kill(GameObject obj)
    {
        ProjectilePoolManager.main.GetPool(ProjectileType.PoisonCauldron).Kill(obj);
    }
}

[System.Serializable]
public class PoisonCauldronLevel
{
    public float cooldown;
    public float projectileSize;
    public float spawnRange;
    public float damage;
}