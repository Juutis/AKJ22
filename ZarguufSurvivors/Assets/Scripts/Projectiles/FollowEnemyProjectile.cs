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
    private bool isKilled;

    private DamageTracker damageTracker = new DamageTracker(100.0f);
    private Damageable targetDamageable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void OnEnable()
    {
        lifeStart = Time.time;
        lastHit = Time.time;
        isKilled = false;
    }

    public void Init(Transform target, float cooldown, float damage)
    {
        lifeStart = Time.time;
        this.target = target;
        lastHit = Time.time;
        this.cooldown = cooldown;
        this.damage = damage;

        if (target != null && target.TryGetComponent<Damageable>(out Damageable dmg))
        {
            targetDamageable = dmg;
        }
        else
        {
            Kill();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isKilled)
        {
            return;
        }

        if (targetDamageable.IsKilled())
        {
            Kill();
        }

        transform.position = target.position;

        if (Time.time - lifeStart >= lifetime)
        {
            isKilled = true;
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

                    if (dmg.IsKilled())
                    {
                        Kill();
                    }
                }
            }

            lastHit = Time.time;
        }
    }

    public void applyDamage(Damageable damageable, float damage)
    {
        damageable.Hurt(damage);
        damageTracker.TargetDamaged(damageable);
        ScreenShake.Instance.Shake(1.0f);
    }

    private void Kill()
    {
        isKilled = true;
        ProjectilePoolManager.main.GetPool(projectileType).Kill(gameObject);
    }
}
