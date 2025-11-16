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

    private DamageTracker damageTracker = new DamageTracker(100.0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Init(Transform target, float cooldown)
    {
        lifeStart = Time.time;
        this.target = target;
        lastHit = Time.time;
        this.cooldown = cooldown;
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
                    var damageToDo = 1;
                    applyDamage(dmg, damageToDo);
                    UIManager.main.ShowPoppingText($"{damageToDo}", Color.red, transform.position);
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
