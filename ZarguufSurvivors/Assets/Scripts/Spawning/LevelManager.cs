using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    void Awake()
    {
        main = this;
    }

    [SerializeField]
    private List<LevelConfig> levels = new();

    private int currentLevelIndex = 0;
    private int nextLevelIndex = 0;

    private LevelConfig currentLevelConfig;

    [SerializeField]
    private WaveManager waveManagerPrefab;

    [SerializeField]
    private Transform waveContainer;

    [SerializeField]
    private Transform parentPrefab;

    [SerializeField]
    private Transform mobContainer;


    private Transform currentLevelParent;

    [SerializeField]
    private Transform playerTransform;

    private bool started = false;
    private bool finished = false;

    private bool waitingForLevel = false;

    private float levelWaitTimer = 0f;

    private Timer runTimer;

    private int totalPlayerXp = 0;
    private int currentPlayerXp = 0;
    private int currentPlayerLevel = 1;
    private int requiredPlayerXp = 25;

    private int playerHealth = 100;
    private int playerMaxHealth = 100;
    private int playerKillCount = 0;

    void Start()
    {
        //playerTransform = Player.main.Transform;
        Begin();
    }

    public void Begin()
    {
        if (levels.Count == 0)
        {
            Debug.LogWarning("No levels set in LevelManager!");
            return;
        }
        MessageBus.Publish(new PlayerHealthChangeEvent(playerHealth, playerMaxHealth));
        MessageBus.Publish(new XpUpdatedEvent(currentPlayerXp, requiredPlayerXp));
        MessageBus.Publish(new LevelGainedEvent(currentPlayerLevel));
        started = true;
        runTimer = new();
    }

    public void Finish()
    {
        Debug.Log("<color=green>All levels and waves have been spawned!</color>");
        finished = true;
    }

    public float GetDistanceOutsideScreen()
    {
        var camHeight = Camera.main.orthographicSize;
        var camWidth = camHeight * Camera.main.aspect;
        return Mathf.Max(camWidth, camHeight) * 1.41f + 1.0f;
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

    public void UpdatePlayerKillCount(int killCount)
    {
        playerKillCount += killCount;
        MessageBus.Publish(new PlayerKillCountChange(playerKillCount));
    }

    void Update()
    {
#if UNITY_EDITOR
        DebugStuffz();
#endif

        if (!started || finished)
        {
            return;
        }

        MessageBus.Publish(new GameDurationUpdatedEvent(runTimer.GetTime()));

        if (currentLevelConfig == null)
        {
            currentLevelConfig = GetNextLevel();
            if (currentLevelConfig == null)
            {
                Finish();
                return;
            }
            else
            {
                currentLevelParent = Instantiate(parentPrefab, waveContainer);
                var waitForSecondsInfo = $"{currentLevelConfig.WaitBeforeStartingNextLevel}s";
                currentLevelParent.name = $"Level #{currentLevelIndex} [WAITING {waitForSecondsInfo}]";
                waitingForLevel = true;
                levelWaitTimer = 0f;
            }
        }
        else if (waitingForLevel)
        {
            levelWaitTimer += Time.deltaTime;
            if (levelWaitTimer >= currentLevelConfig.WaitBeforeStartingNextLevel)
            {
                waitingForLevel = false;
                var waveManager = Instantiate(waveManagerPrefab);
                currentLevelParent.name = $"Level #{currentLevelIndex} [SPAWNING]";
                waveManager.Initialize(currentLevelIndex, currentLevelConfig, currentLevelParent, mobContainer, playerTransform, delegate
                {
                    currentLevelParent.name = $"Level #{currentLevelIndex} [FINISHED]";
                    currentLevelConfig = null;
                });
            }
        }
    }

    private void DebugStuffz()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            var mob = GameObject.FindFirstObjectByType<SpawnableMob>();
            if (mob != null) { mob.Kill(); }
        }
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            UpdatePlayerXp(1);
        }
        if (Keyboard.current != null && Keyboard.current.oKey.wasPressedThisFrame)
        {
            UpdatePlayerHealth(-5);
        }
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            UpdatePlayerHealth(2);
        }
        if (Keyboard.current != null && Keyboard.current.uKey.wasPressedThisFrame)
        {
            UpdatePlayerKillCount(2);
        }
    }

    private LevelConfig GetNextLevel()
    {
        if (levels.Count <= nextLevelIndex)
        {
            return null;
        }
        var level = levels[nextLevelIndex];
        currentLevelIndex = nextLevelIndex;
        nextLevelIndex += 1;
        return level;
    }
}


public struct GameDurationUpdatedEvent : IEvent
{
    public double CurrentRunTime { get; }

    public GameDurationUpdatedEvent(double currentTime)
    {
        CurrentRunTime = currentTime;
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

public struct PlayerKillCountChange : IEvent
{
    public int KillCount;

    public PlayerKillCountChange(int killCount)
    {
        KillCount = killCount;
    }
}
