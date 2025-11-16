using System;
using System.Linq;
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
    private float[] playerSpeedLevels;

    private int playerHealth = 100;
    private int playerMaxHealth = 100;
    private const int playerOriginalMaxHealth = 100;


    [SerializeField]
    private BoxCollider2D movementBounds;

    private int totalPlayerXp = 0;
    private int currentPlayerXp = 0;
    private int currentPlayerLevel = 0;
    private int requiredPlayerXp = 25;

    private int numberOfMobsKilled = 0;
    private int numberOfMobsSpawned = 0;

    private Rigidbody2D playerBody;
    private SpriteFlasher flasher;
    private float lastDamaged = 0;
    private float invulnerabilityDuration = 0.5f;
    private float magnetCD = 0.333f;
    private float lastMagnet = 0f;

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

        Invoke("GainFirstLevel", 0.5f); 
        flasher = GetComponentInChildren<SpriteFlasher>();
    }

    public void GainFirstLevel()
    {
        MessageBus.Publish(new LevelGainedEvent(currentPlayerLevel));
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        int currentSpeedLevel = SkillManager.main.GetSkillLevel(SkillType.MovementSpeedBoost);

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
            playerBody.linearVelocity = moveValue * playerSpeedLevels[currentSpeedLevel];
        }
        else
        {
            Vector2 oldPos2 = new Vector2(transform.position.x, transform.position.y);
            Vector2 newPos2 = oldPos2 + moveValue * playerSpeedLevels[currentSpeedLevel] * Time.deltaTime;

            transform.position = new Vector3(newPos2.x, newPos2.y, transform.position.z);
        }

        if (playerMaxHealth < (playerOriginalMaxHealth + SkillManager.main.GetPlayerMaxHealthAddition()))
        {
            playerMaxHealth = playerOriginalMaxHealth + SkillManager.main.GetPlayerMaxHealthAddition();
            playerHealth += SkillManager.main.GetPlayerHealthAddition();
        }

        int magnet = SkillManager.main.GetMagnetSkillLevel();

        if (magnet > 0 && (Time.time - lastMagnet > magnetCD))
        {
            float magnetRange = 2f + 0.3f * magnet;
            Collider2D[] pickups = Physics2D.OverlapCircleAll(transform.position, magnetRange, LayerMask.GetMask("Pickupable"));

            foreach (Collider2D pickup in pickups)
            {
                if (pickup.gameObject.TryGetComponent(out XpDrop xpd))
                {
                    xpd.GoToPlayer();
                }
            }

            lastMagnet = Time.time;
        }
    }

    public void UpdatePlayerHealth(int healthChange)
    {
        playerHealth += healthChange;
        if (playerHealth <= 0)
        {
            MessageBus.Publish(new PlayerDiedEvent(playerHealth));
        }
        playerHealth = Math.Clamp(playerHealth, 0, playerMaxHealth);
        MessageBus.Publish(new PlayerHealthChangeEvent(playerHealth, playerMaxHealth));
        UIManager.main.ShowPoppingText($"{healthChange}", healthChange > 0 ? Color.green : Color.red, transform.position);
    }

    public void UpdatePlayerXp(int xpGained)
    {
        int currentXpGained = Mathf.RoundToInt(xpGained * SkillManager.main.GetXPBoostMultiplier());
        currentPlayerXp += currentXpGained;
        totalPlayerXp += currentXpGained;

        if (currentPlayerXp >= requiredPlayerXp)
        {
            currentPlayerXp = 0;
            currentPlayerLevel += 1;
            MessageBus.Publish(new LevelGainedEvent(currentPlayerLevel));
            UpdateRequiredXP(currentPlayerLevel);
        }
        MessageBus.Publish(new XpUpdatedEvent(currentPlayerXp, requiredPlayerXp));
        UIManager.main.ShowPoppingText($"{currentXpGained}", Color.yellow, transform.position);
    }

    private void UpdateRequiredXP(int currentPlayerLevel)
    {
        requiredPlayerXp = 25 + (int)Math.Pow(currentPlayerLevel, 1.5f) * 10;
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
                SoundManager.main.PlaySound(GameSoundType.Hurt);
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
            SoundManager.main.PlaySound(GameSoundType.PickupXp);
            xpDrop.Kill();
        }

        if (collider.gameObject.tag == "Magnet")
        {
            XpDropManager.Instance.Drops.ForEach(it => it.GoToPlayer());
            Destroy(collider.gameObject);
        }
    }

    private bool canTakeDamage()
    {
        return Time.time > lastDamaged + invulnerabilityDuration;
    }


    private void OnEnable()
    {
        MessageBus.Subscribe<MobWasKilledEvent>(OnMobWasKilledEvent);
        MessageBus.Subscribe<MobWasSpawnedEvent>(OnMobWasSpawnedEvent);
    }

    private void OnDisable()
    {
        MessageBus.Unsubscribe<MobWasKilledEvent>(OnMobWasKilledEvent);
        MessageBus.Unsubscribe<MobWasSpawnedEvent>(OnMobWasSpawnedEvent);
    }

    private void OnMobWasKilledEvent(MobWasKilledEvent e)
    {
        var mobConfig = e.EnemyConfig;
        SoundManager.main.PlaySound(mobConfig.GameSoundType);
        numberOfMobsKilled += 1;
        MessageBus.Publish(new PlayerKillCountChange(numberOfMobsKilled));
        if (LevelManager.main.IsFinished && numberOfMobsKilled >= numberOfMobsSpawned) {
            MessageBus.Publish(new PlayerWinEvent(1));
        }
    }

    private void OnMobWasSpawnedEvent(MobWasSpawnedEvent e)
    {
        numberOfMobsSpawned += 1;
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
            if (clampedX != currentPos.x)
            {
                playerBody.position = new Vector2(clampedX, clampedY);
            }
            if (clampedY != currentPos.y)
            {
                playerBody.position = new Vector2(clampedX, clampedY);
            }
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


public struct PlayerWinEvent : IEvent
{
    public int Number;

    public PlayerWinEvent(int number)
    {
        Number = number;
    }
}