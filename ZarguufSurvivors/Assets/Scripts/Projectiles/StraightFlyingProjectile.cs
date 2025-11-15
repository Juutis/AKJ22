using UnityEngine;

public class StraightFlyingProjectile : MonoBehaviour
{
    [SerializeField]
    private bool randomDirection;
    [SerializeField]
    private ProjectileType projectileType;

    private Vector2 dir;
    private float speed;
    private float lifetime = 5;
    private float lifeStart;
    private float radiusCoef = 0.65f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Init(Vector2 dir, float speed)
    {
        if (randomDirection)
        {
            this.dir = Random.insideUnitCircle.normalized;
        }
        else
        {
            this.dir = dir;
        }

        this.speed = speed;
        lifeStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lifeStart >= lifetime) {
            ProjectilePoolManager.main.GetPool(projectileType).Kill(gameObject);
        }

        Vector2 oldPos2 = new Vector2(transform.position.x, transform.position.y);
        Vector2 newPos2 = oldPos2 + dir * speed * Time.deltaTime;
        transform.position = new Vector3(newPos2.x, newPos2.y, transform.position.z);


        Debug.DrawLine(transform.position, transform.position + Vector3.up * transform.localScale.x * radiusCoef, Color.red);
        Debug.DrawLine(transform.position, transform.position + Vector3.down * transform.localScale.x * radiusCoef, Color.red);
        Debug.DrawLine(transform.position, transform.position + Vector3.left * transform.localScale.x * radiusCoef, Color.red);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * transform.localScale.x * radiusCoef, Color.red);
        Collider2D collider = Physics2D.OverlapCircle(transform.position, transform.localScale.x * radiusCoef, LayerMask.GetMask("Enemy Damage Receiver"));

        if (collider != null && collider.TryGetComponent<Damageable>(out Damageable dmg))
        {
            dmg.Hurt(1);
        }
    }
}
