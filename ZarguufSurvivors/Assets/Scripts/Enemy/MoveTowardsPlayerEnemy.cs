using UnityEngine;

public class MoveTowardsPlayerEnemy : MonoBehaviour
{
    [SerializeField]
    private float minSpeed = 3.0f;

    [SerializeField]
    private float maxSpeed = 5.0f;

    private float speed;
    private Rigidbody2D rb;
    private Transform target;
    private Animator anim;
    private SpriteRenderer rend;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponentInChildren<SpriteRenderer>();
    }

    public void Init()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        speed = Random.Range(minSpeed, maxSpeed);
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
