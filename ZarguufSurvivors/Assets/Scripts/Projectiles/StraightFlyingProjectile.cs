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

        Collider2D collider = Physics2D.OverlapCircle(transform.position, transform.localScale.x);

        if (collider != null)
        {
            Debug.Log("Projectile hit target!");
        }
    }
}
