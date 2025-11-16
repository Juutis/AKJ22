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

    private bool isPaused = false;

    public bool IsPaused { get { return isPaused; } set { isPaused = value; } }

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
        if (isPaused)
        {
            return;
        }

        MessageBus.Publish(new GameDurationUpdatedEvent(runTimer.GetTime()));

        if (!started || finished)
        {
            return;
        }

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

public struct PlayerKillCountChange : IEvent
{
    public int KillCount;

    public PlayerKillCountChange(int killCount)
    {
        KillCount = killCount;
    }
}
