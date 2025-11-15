using System.Threading;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private WaveStatus status = WaveStatus.None;
    public WaveStatus Status {get {return status;}}


    private SpawnWave spawnWaveConfig;
    public QueueBehavior QueueBehavior { get { return spawnWaveConfig.QueueBehavior; } }
    private SpawnWaveConfig wave;

    private Transform spawnContainer;

    private Transform playerTransform;

    private int amountSpawned = 0;

    private float spawnTimer = 0f;

    private bool isSpawning = false;
    private int waveIndex = 0;

    private float waitTime = 0f;

    private float waitBeforeStartTimer = 0f;

    public void Initialize(int index, SpawnWave spawnWaveConfig, Transform mobContainer, Transform playerTransform)
    {
        this.spawnWaveConfig = spawnWaveConfig;
        var waveConfig = spawnWaveConfig.WaveConfig;
        if (waveConfig.EnemyConfig == null)
        {
            Debug.LogError($"Wave {waveConfig} doesn't have EnemyConfig assigned!");
        }
        if (waveConfig.Amount == 0)
        {
            Debug.LogError($"Wave {waveConfig} will spawn 0 mobs (Amount is 0)!");
        }
        if (waveConfig.Timing == SpawnTiming.Interval && waveConfig.AmountPerInterval == 0)
        {
            Debug.LogError($"Wave {waveConfig} will spawn 0 mobs (AmountInterval is 0)!");
        }
        if (waveConfig.Timing == SpawnTiming.Interval && waveConfig.Interval > 10)
        {
            Debug.LogWarning($"Wave {waveConfig} will spawn mobs very slow (Interval is over 10s)!");
        }
        waveIndex = index;
        wave = waveConfig;
        spawnContainer = mobContainer;
        this.playerTransform = playerTransform;
        SetName();
    }

    public void StartWaiting()
    {
        status = WaveStatus.Waiting;
        waitTime = spawnWaveConfig.WaitSecondsBeforeStarting;
        SetName();
    }

    private void Begin()
    {
        isSpawning = true;
        status = WaveStatus.Spawning;
        spawnTimer = 0f;
        SetName();
    }

    private void SpawnMob()
    {
        if (amountSpawned >= wave.Amount)
        {
            return;
        }
        var mob = SpawnableMobPool.main.Get();
        mob.Initialize(wave.EnemyConfig, amountSpawned, waveIndex, spawnContainer);

        if (wave.Formation == SpawnFormation.CircleAroundPlayer)
        {
            mob.SetPosition(GetRandomPositionAroundCircle(playerTransform.position));
        }

        mob.Begin();
        amountSpawned += 1;
        if (amountSpawned >= wave.Amount)
        {
            isSpawning = false;
            status = WaveStatus.Finished;
        }
        SetName();
    }

    private Vector2 GetRandomPositionAroundCircle(Vector2 position)
    {
        float radius = LevelManager.main.GetDistanceOutsideScreen(); // should be outside the screen
        return position + UnityEngine.Random.insideUnitCircle.normalized * radius;
    }

    private void SetName()
    {
        var waiting = Status == WaveStatus.Waiting ? $" {waitTime}s" : "";
        var spawnWait = (Status == WaveStatus.Spawning && wave.Timing == SpawnTiming.Interval) ? $" {wave.Interval}s" : "";
        name = $"Wave #{waveIndex} [{Status}{waiting}{spawnWait}] [{amountSpawned}/{wave.Amount}] ";
    }

    // Update is called once per frame
    void Update()
    {
        if (status == WaveStatus.Waiting)
        {
            waitBeforeStartTimer += Time.deltaTime;
            if (waitBeforeStartTimer >= waitTime)
            {
                Begin();
            }
        }
        if (!isSpawning)
        {
            return;
        }
        if (wave.Timing == SpawnTiming.Instant)
        {
            //Debug.Log($"Spawing {wave.Amount} mobs instantly...");
            for (int i = 0; i < wave.Amount; i += 1)
            {
                SpawnMob();
            }
        }
        if (wave.Timing == SpawnTiming.Interval)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= wave.Interval)
            {
                spawnTimer = 0f;
                for (int i = 0; i < wave.AmountPerInterval; i += 1)
                {
                    SpawnMob();
                }
            }
        }
    }
}


public enum WaveStatus
{
    None,
    Waiting,
    Spawning,
    Finished
}