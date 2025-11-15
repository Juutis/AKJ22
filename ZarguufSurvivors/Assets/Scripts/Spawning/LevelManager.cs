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
    private LevelConfig currentLevel;

    [SerializeField]
    private WaveSpawner waveSpawnerPrefab;

    [SerializeField]
    private Transform waveContainer;

    [SerializeField]
    private Transform mobContainer;

    private int currentWaveIndex = 0;
    private int nextWaveIndex = 0;
    private WaveSpawner currentWave;
    private SpawnWave currentWaveConfig;

    [SerializeField]
    private Transform playerTransform;

    private bool started = false;
    private bool finished = false;

    private float waveWaitTimer = 0f;

    void Start()
    {
        //playerTransform = Player.main.Transform;
        Begin();
    }

    public void Begin()
    {
        started = true;
    }

    public void Finish()
    {
        Debug.Log("<color=green>All waves have been spawned!</color>");
        finished = true;
    }

    // used for spawning mobs, how to calculate?
    public float GetDistanceOutsideScreen()
    {
        return 5f;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            var mob = GameObject.FindFirstObjectByType<SpawnableMob>();
            if (mob != null) { mob.Kill(); }
        }
#endif

        if (!started || finished)
        {
            return;
        }
        if (currentWave == null || currentWave.Status == WaveStatus.Finished)
        {
            currentWaveConfig = GetNextWave();
            if (currentWaveConfig == null)
            {
                Finish();
                return;
            }
            currentWave = Instantiate(waveSpawnerPrefab, waveContainer);
            currentWave.Initialize(currentWaveIndex, currentWaveConfig.WaveConfig, mobContainer, playerTransform);
        }
        else
        {
            if (currentWave.Status == WaveStatus.None)
            {
                currentWave.StartWaiting(currentWaveConfig.WaitSecondsBeforeStarting);
                waveWaitTimer = 0f;
            }
            if (currentWave.Status == WaveStatus.Waiting)
            {
                waveWaitTimer += Time.deltaTime;
                if (waveWaitTimer >= currentWaveConfig.WaitSecondsBeforeStarting)
                {
                    currentWave.Begin();
                }
            }
        }
    }

    private SpawnWave GetNextWave()
    {
        SpawnWave waveConfig = currentLevel.GetWave(nextWaveIndex);
        currentWaveIndex = nextWaveIndex;
        nextWaveIndex += 1;
        return waveConfig;
    }
}
