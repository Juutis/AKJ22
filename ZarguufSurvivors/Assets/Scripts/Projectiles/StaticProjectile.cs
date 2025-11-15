using UnityEngine;

public class StaticProjectile : MonoBehaviour
{
    [SerializeField]
    private ProjectileType projectileType;
    [SerializeField]
    private float lifetime;
    private float lifeStart;
    IWeapon weapon;

    void Start()
    {
    }

    public void Init(IWeapon weapon){
        this.weapon = weapon;
        lifeStart = Time.time;
    }

    public void Init(IWeapon weapon, Vector2 pos, float scale)
    {
        Init(weapon);
        transform.position = pos;
        transform.localScale *= scale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lifeStart > lifetime)
        {
            weapon.Kill(gameObject);
        }

        Collider2D collider = Physics2D.OverlapCircle(transform.position, transform.localScale.x);

        if (collider != null)
        {
            Debug.Log("Projectile hit target!");
        }
    }
}
