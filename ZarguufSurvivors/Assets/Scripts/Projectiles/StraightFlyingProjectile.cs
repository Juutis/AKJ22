using UnityEngine;

public class StraightFlyingProjectile : MonoBehaviour
{
    [SerializeField]
    private bool randomDirection;

    private Vector2 dir;
    private float speed;

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
    }

    // Update is called once per frame
    void Update()
    {
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
