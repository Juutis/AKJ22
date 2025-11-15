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

    void Start()
    {
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
            dmg.Hurt(1);
        }
    }
}
