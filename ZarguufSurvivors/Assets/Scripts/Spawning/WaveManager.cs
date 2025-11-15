using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    private WaveSpawner waveSpawnerPrefab;

    private Transform mobContainer;

    private List<WaveSpawner> waves = new();

    private Transform playerTransform;

    private bool started = false;
    private bool finished = false;

    private int levelIndex = 0;
    private LevelConfig currentLevelConfig;
    private UnityAction finishedCallback;

    void Start()
    {
        Begin();
    }

    public void Initialize(int index, LevelConfig levelConfig, Transform parent, Transform mobContainer, Transform playerTransform, UnityAction finishedCallback)
    {
        levelIndex = index;
        transform.parent = parent;
        currentLevelConfig = levelConfig;
        this.mobContainer = mobContainer;
        this.playerTransform =playerTransform;
        this.finishedCallback = finishedCallback;
    }

    public void Begin()
    {
        started = true;
    }

    public void Finish()
    {
        Debug.Log($"<color=green>Level #{levelIndex}: All waves have been spawned!</color>");
        finishedCallback();
        finished = true;
    }

    public float GetDistanceOutsideScreen()
    {
        var camHeight = Camera.main.orthographicSize;
        var camWidth = camHeight * Camera.main.aspect;
        return Mathf.Max(camWidth, camHeight) * 1.41f + 1.0f;
    }

    void Update()
    {
        if (!started || finished)
        {
            return;
        }

        if (waves.Count == 0)
        {
            PickupWaves();
        }

        var allWavesFinished = waves.All(wave => wave.Status == WaveStatus.Finished);
        if (allWavesFinished)
        {
            Finish();
            return;
        }

        //var pendingWaves = waves.FindAll(wave => wave.Status == WaveStatus.None);

        WaveSpawner previousWave = null;
        foreach(var wave in waves)
        {
            if (wave.Status == WaveStatus.None) {
                if (previousWave == null || previousWave.Status == WaveStatus.Finished)
                {
                    wave.StartWaiting();
                }
                else if (wave.QueueBehavior == QueueBehavior.StartsWhenLevelStarts)
                {
                    wave.StartWaiting();
                }
                else if (wave.QueueBehavior == QueueBehavior.StartsWhenPreviousStarts &&
                     (previousWave.Status == WaveStatus.Spawning || previousWave.Status == WaveStatus.Finished))
                {
                    wave.StartWaiting();
                }
            }
            previousWave = wave;
        }

    }

    private void PickupWaves()
    {
        var waveConfigs = currentLevelConfig.AllWaves();
        var index = 0;
        foreach(var waveConfig in waveConfigs)
        {
            var waveSpawner = Instantiate(waveSpawnerPrefab, transform.parent);
            waveSpawner.Initialize(index, waveConfig, mobContainer, playerTransform);
            waves.Add(waveSpawner);
            index += 1;
        }
    }

}