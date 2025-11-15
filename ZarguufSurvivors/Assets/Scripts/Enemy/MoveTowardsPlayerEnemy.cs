using UnityEngine;

public class MoveTowardsPlayerEnemy : MonoBehaviour
{
    private float speed;
    private Rigidbody2D rb;
    private Transform target;
    private SpriteRenderer rend;

    public void Init(EnemyConfig config)
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponentInChildren<SpriteRenderer>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        speed = Random.Range(config.MinSpeed, config.MaxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        rb.linearVelocity = (target.position - transform.position).normalized * speed;
        if (rb.linearVelocity.x > 0)
        {
            rend.flipX = false;
        }
        else
        {
            rend.flipX = true;
        }
    }
}
