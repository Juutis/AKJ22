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


    [SerializeField]
    private BoxCollider2D movementBounds;

    private int totalPlayerXp = 0;
    private int currentPlayerXp = 0;
    private int currentPlayerLevel = 1;
    private int requiredPlayerXp = 25;

    private int numberOfMobsKilled = 0;

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
        //MessageBus.Publish(new LevelGainedEvent(currentPlayerLevel));

        flasher = GetComponentInChildren<SpriteFlasher>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        if (moveValue.sqrMagnitude != 0)
        {
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
            MessageBus.Publish(new PlayerDiedEvent(playerHealth));
        }
        playerHealth = Math.Clamp(playerHealth, 0, playerMaxHealth);
        MessageBus.Publish(new PlayerHealthChangeEvent(playerHealth, playerMaxHealth));
        UIManager.main.ShowPoppingText($"{healthChange}", healthChange > 0 ? Color.green : Color.red, transform.position);
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
            UpdateRequiredXP(currentPlayerLevel);
        }
        MessageBus.Publish(new XpUpdatedEvent(currentPlayerXp, requiredPlayerXp));
        UIManager.main.ShowPoppingText($"{xpGained}", Color.yellow, transform.position);
    }

    private void UpdateRequiredXP(int currentPlayerLevel)
    {
        requiredPlayerXp = 25 + (int)Math.Pow(currentPlayerLevel, 1.25f) * 10;
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        handleCollision(collider2D);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        handleCollision(collision);
    }

    public void handleCollision(Collider2D collider)
    {
        if (collider.transform.parent != null)
        {
            var spawnableMob = collider.transform.parent.GetComponent<SpawnableMob>();
            if (spawnableMob != null && canTakeDamage())
            {
                int damage = spawnableMob.GetDamageDoneToPlayer();
                UpdatePlayerHealth(-damage);
                ScreenShake.Instance.Shake(10.0f);
                flasher.Flash();
                lastDamaged = Time.time;
            }
        }

        var xpDrop = collider.GetComponent<XpDrop>();
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


    private void OnEnable()
    {
        MessageBus.Subscribe<MobWasKilledEvent>(OnMobWasKilledEvent);
    }

    private void OnDisable()
    {
        MessageBus.Unsubscribe<MobWasKilledEvent>(OnMobWasKilledEvent);
    }

    private void OnMobWasKilledEvent(MobWasKilledEvent e)
    {
        var mobName = e.Name;
        numberOfMobsKilled += 1;
        MessageBus.Publish(new PlayerKillCountChange(numberOfMobsKilled));
    }

    float xDamping = 0.5f;
    float yDamping = 5f;
    void LateUpdate()
    {
        if (movementBounds != null)
        {
            // Get the boundary limits
            var min = movementBounds.bounds.min;
            var max = movementBounds.bounds.max;

            // Get the Rigidbody's position
            Vector2 currentPos = playerBody.position;

            // Clamp the X and Y coordinates
            float clampedX = Mathf.Clamp(currentPos.x, min.x + xDamping, max.x - xDamping);
            float clampedY = Mathf.Clamp(currentPos.y, min.y + yDamping, max.y - yDamping);

            // Apply the clamped position back to the Rigidbody
            playerBody.position = new Vector2(clampedX, clampedY);
        }
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

public struct PlayerDiedEvent : IEvent
{
    public int CurrentHealth;

    public PlayerDiedEvent(int currentHealth)
    {
        CurrentHealth = currentHealth;
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

public struct PlayerKillCountChange : IEvent
{
    public int KillCount;

    public PlayerKillCountChange(int killCount)
    {
        KillCount = killCount;
    }
}
