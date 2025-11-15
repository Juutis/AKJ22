using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using UnityEngine;

public class StaticProjectile : MonoBehaviour
{
    [SerializeField]
    private ProjectileType projectileType;
    [SerializeField]
    private float lifetime;
    private float lifeStart;
    private float radiusCoef = 0.65f;
    IWeapon weapon;
    private DamageTracker damageTracker = new DamageTracker(1.0f);


    void Start()
    {
        Invoke("CleanUpDamageTrackers", 1.0f);
    }

    public void Init(IWeapon weapon)
    {
        this.weapon = weapon;
        lifeStart = Time.time;
    }

    public void Init(IWeapon weapon, Vector2 pos, float scale)
    {
        Init(weapon);
        transform.position = pos;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lifeStart > lifetime)
        {
            weapon.Kill(gameObject);
        }
        Collider2D collider = Physics2D.OverlapCircle(transform.position, transform.localScale.x * radiusCoef, LayerMask.GetMask("Enemy Damage Receiver"));

        if (collider != null && collider.TryGetComponent<Damageable>(out Damageable dmg))
        {
            if (damageTracker.CanHurt(dmg))
            {
                applyDamage(dmg, 1);
            }
        }
    }

    public void applyDamage(Damageable damageable, float damage)
    {
        damageable.Hurt(damage);
        damageTracker.TargetDamaged(damageable);
    }

    public void CleanUpDamageTrackers()
    {
        damageTracker.CleanUp();
        Invoke("CleanUpDamageTrackers", 1.0f);
    }
}

public class DamageTracker
{
    private float damageTickDelay = 1.0f;
    private Dictionary<Damageable, DamageTrackerEntry> damageTrackers = new Dictionary<Damageable, DamageTrackerEntry>();

    public DamageTracker(float damageTickDelay)
    {
        this.damageTickDelay = damageTickDelay;
    }

    public bool CanHurt(Damageable damageable)
    {
        DamageTrackerEntry tracker;
        if (damageTrackers.TryGetValue(damageable, out tracker))
        {
            return Time.time > tracker.DamagedAt + damageTickDelay;
        }
        return true;
    }

    public void TargetDamaged(Damageable damageable)
    {
        DamageTrackerEntry tracker;
        if (damageTrackers.TryGetValue(damageable, out tracker))
        {
            tracker.DamagedAt = Time.time;
        }
        else
        {
            tracker = new DamageTrackerEntry();
            tracker.Damageable = damageable;
            tracker.DamagedAt = Time.time;
            damageTrackers[damageable] = tracker;
        }
    }

    public void CleanUp()
    {
        damageTrackers.Where(it => Time.time > it.Value.DamagedAt + damageTickDelay * 2.0f)
            .Select(it => damageTrackers.Remove(it.Key));
    }
}

public class DamageTrackerEntry
{
    private Dictionary<Damageable, DamageTracker> damageTrackers = new Dictionary<Damageable, DamageTracker>();
    public float DamagedAt;
    public Damageable Damageable;
}
