using System.Collections.Generic;
using UnityEngine;

public class ProtectionScrollWeapon : MonoBehaviour, IWeapon
{
    [SerializeField]
    private List<ProtectionScrollLevel> levels;

    private float lastShoot;
    private PlayerMovement player;

    private ProjectilePool pool;

    private List<StaticProjectile> projectiles = new();
    private ProtectionScrollLevel currentLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLevel = levels[0];
        lastShoot = 0;
        player = transform.parent.parent.GetComponent<PlayerMovement>();

        MoveContainer();
    }

    // Update is called once per frame
    void Update()
    {
        if (!SkillManager.main.IsSkillActive(SkillType.ProtectionScroll))
        {
            return;
        }

        currentLevel = levels[Mathf.Min(levels.Count - 1, SkillManager.main.GetSkillLevel(SkillType.ProtectionScroll))];
        if (Time.time - lastShoot >= currentLevel.cooldown && projectiles.Count < currentLevel.maxProjectiles)
        {
            Shoot();
        }

        transform.Rotate(0f, 0f, currentLevel.rotateSpeed * Time.deltaTime);
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

        projectile.Init(this, currentLevel.damage, currentLevel.hitCount);
        projectiles.Add(projectile);

        for (int i = 0; i < projectiles.Count; i++)
        {
            projectiles[i].transform.position = player.transform.position + Quaternion.Euler(0, 0, 360 / projectiles.Count * i) * Vector2.up * currentLevel.projectileDistance;
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


[System.Serializable]
public class ProtectionScrollLevel
{
    public float cooldown;
    public float rotateSpeed;
    public int maxProjectiles;
    public float projectileDistance;
    public float damage;
    public int hitCount;
}
