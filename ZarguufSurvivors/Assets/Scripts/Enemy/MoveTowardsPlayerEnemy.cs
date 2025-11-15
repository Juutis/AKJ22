using UnityEngine;
using System;
using UnityEditor;

public class MoveTowardsPlayerEnemy : MonoBehaviour
{
    private float speed;
    private Rigidbody2D rb;
    private Transform target;
    private SpriteRenderer rend;
    private EnemyConfig config;

    private Vector3 targetPosition;

    private Action handleFunc;
    private bool runThroughResetting = false;

    public void Init(EnemyConfig config)
    {
        this.config = config;
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponentInChildren<SpriteRenderer>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        speed = UnityEngine.Random.Range(config.MinSpeed, config.MaxSpeed);        
    }

    public void Begin()
    {
        switch(config.MoveStrategy)
        {
            case MoveStrategy.RUN_TOWARDS_PLAYER:
                handleFunc = handleRunTowardsPlayer;
                break;
            case MoveStrategy.RUN_THROUGH:
                handleFunc = handleRunThrough;
                calculateRunThroughTargetPosition();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        handleFunc();
    }

    private void handleRunTowardsPlayer()
    {
        targetPosition = target.position;
    }

    private void handleRunThrough()
    {
        if (Vector3.Distance(transform.position, targetPosition) < 0.15f && !runThroughResetting)
        {
            Invoke("calculateRunThroughTargetPosition", 1.0f);
            runThroughResetting = true;
        }
    }

    private void calculateRunThroughTargetPosition()
    {
        var dir = target.position - transform.position;
        targetPosition = target.position + dir.normalized * config.RunThroughDistance;
        runThroughResetting = false;
    }

    void FixedUpdate()
    {
        var diff = targetPosition - transform.position;
        rb.linearVelocity = diff.normalized * speed;
        if (diff.magnitude > 0.1f)
        {
            if (rb.linearVelocity.x > 0)
            {
                rend.flipX = false;
            }
            else
            {
                rend.flipX = true;
            }
        } else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
