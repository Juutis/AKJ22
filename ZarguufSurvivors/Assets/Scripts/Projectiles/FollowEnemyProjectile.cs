using UnityEngine;

public class FollowEnemyProjectile : MonoBehaviour
{
    [SerializeField]
    private ProjectileType projectileType;
    private float lifetime = 5;
    private float lifeStart;
    private Transform target;
    private float cooldown = 1;
    private float lastHit;
    private float damage;

    private DamageTracker damageTracker = new DamageTracker(100.0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Init(Transform target, float cooldown, float damage)
    {
        lifeStart = Time.time;
        this.target = target;
        lastHit = Time.time;
        this.cooldown = cooldown;
        this.damage = damage;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;

        if (Time.time - lifeStart >= lifetime) {
            ProjectilePoolManager.main.GetPool(projectileType).Kill(gameObject);
        }

        if (Time.time - lastHit > cooldown)
        {
            if (target != null && target.TryGetComponent<Damageable>(out Damageable dmg))
            {
                if (damageTracker.CanHurt(dmg))
                {
                    applyDamage(dmg, damage);
                    UIManager.main.ShowPoppingText($"{damage}", Color.red, transform.position);
                }
            }
        }
    }

    public void applyDamage(Damageable damageable, float damage)
    {
        damageable.Hurt(damage);
        damageTracker.TargetDamaged(damageable);
        ScreenShake.Instance.Shake(1.0f);
    }
}
