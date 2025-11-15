using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Vector2 MoveDir { get { return moveDir; } }
    private Vector2 moveDir;

    InputAction moveAction;

    [SerializeField]
    private bool useRigidbody;
    [SerializeField]
    private SpriteRenderer playerSprite;
    [SerializeField]
    private float playerSpeed;

    private int playerHealth = 100;
    private int playerMaxHealth = 100;


    private int totalPlayerXp = 0;
    private int currentPlayerXp = 0;
    private int currentPlayerLevel = 1;
    private int requiredPlayerXp = 25;

    private Rigidbody2D playerBody;
    private SpriteFlasher flasher;
    private float lastDamaged = 0;
    private float invulnerabilityDuration = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        playerBody = GetComponent<Rigidbody2D>();

        if (!useRigidbody)
        {
            playerBody.simulated = false;
        }

        moveDir = Vector2.up;
        MessageBus.Publish(new PlayerHealthChangeEvent(playerHealth, playerMaxHealth));
        MessageBus.Publish(new XpUpdatedEvent(currentPlayerXp, requiredPlayerXp));
        MessageBus.Publish(new LevelGainedEvent(currentPlayerLevel));

        flasher = GetComponentInChildren<SpriteFlasher>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        
        if (moveValue.sqrMagnitude != 0) {
            moveDir = moveValue.normalized;
        }

        if (moveValue.x < 0)
        {
            playerSprite.flipX = true;
        }
        else if (moveValue.x > 0)
        {
            playerSprite.flipX = false;
        }

        if (useRigidbody)
        {
            playerBody.linearVelocity = moveValue * playerSpeed;
        }
        else
        {
            Vector2 oldPos2 = new Vector2(transform.position.x, transform.position.y);
            Vector2 newPos2 = oldPos2 + moveValue * playerSpeed * Time.deltaTime;

            transform.position = new Vector3(newPos2.x, newPos2.y, transform.position.z);
        }
    }

    public void UpdatePlayerHealth(int healthChange)
    {
        playerHealth += healthChange;
        if (playerHealth <= 0)
        {
            Debug.Log("<color=red>Player died!!!</color>");
        }
        playerHealth = Math.Clamp(playerHealth, 0, playerMaxHealth);
        MessageBus.Publish(new PlayerHealthChangeEvent(playerHealth, playerMaxHealth));
    }

    public void UpdatePlayerXp(int xpGained)
    {
        currentPlayerXp += xpGained;
        totalPlayerXp += xpGained;
        if (currentPlayerXp >= requiredPlayerXp)
        {
            currentPlayerXp = 0;
            currentPlayerLevel += 1;
            MessageBus.Publish(new LevelGainedEvent(currentPlayerLevel));
        }
        MessageBus.Publish(new XpUpdatedEvent(currentPlayerXp, requiredPlayerXp));
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        var spawnableMob = collider2D.GetComponent<SpawnableMob>();
        if (spawnableMob != null && canTakeDamage()) {
            int damage = spawnableMob.GetDamageDoneToPlayer();
            UpdatePlayerHealth(-damage);
            ScreenShake.Instance.Shake(10.0f);
            flasher.Flash();
            lastDamaged = Time.time;
        }

        var xpDrop = collider2D.GetComponent<XpDrop>();
        if (xpDrop != null)
        {
            var amount = xpDrop.XpDropAmount;
            UpdatePlayerXp(amount);
            xpDrop.Kill();
        }
    }

    private bool canTakeDamage()
    {
        return Time.time > lastDamaged + invulnerabilityDuration;
    }
}


public struct PlayerHealthChangeEvent : IEvent
{
    public int CurrentHealth;
    public int MaxHealth;

    public PlayerHealthChangeEvent(int currentHealth, int maxHealth)
    {
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
    }
}

public struct XpUpdatedEvent : IEvent
{
    public int CurrentXp { get; }
    public int RequiredXp { get; }

    public XpUpdatedEvent(int currentXp, int requiredXp)
    {
        CurrentXp = currentXp;
        RequiredXp = requiredXp;
    }
}

public struct LevelGainedEvent : IEvent
{
    public int CurrentLevel;

    public LevelGainedEvent(int currentLevel)
    {
        CurrentLevel = currentLevel;
    }
}