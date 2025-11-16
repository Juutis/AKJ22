using UnityEngine;
using System;
using UnityEditor;
using System.Text.RegularExpressions;

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
    private int groupSize;
    private int indexInGroup;
    private int prevGroupedSide = 1;

    public void Init(EnemyConfig config, int indexInGroup, int groupSize)
    {
        this.config = config;
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponentInChildren<SpriteRenderer>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        speed = UnityEngine.Random.Range(config.MinSpeed, config.MaxSpeed);
        this.indexInGroup = indexInGroup;
        this.groupSize = groupSize;
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
        if (groupSize > 1)
        {
            var dir = target.position - transform.position;
            var offsetDir = -Vector2.Perpendicular(dir).normalized;
            Vector3 offset = 2.0f * prevGroupedSide * offsetDir * (-groupSize / 2 + indexInGroup);
            targetPosition = target.position + dir.normalized * config.RunThroughDistance + offset;
            prevGroupedSide = -prevGroupedSide;
        }
        else
        {
            var dir = target.position - transform.position;
            targetPosition = target.position + dir.normalized * config.RunThroughDistance;
        }
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
