using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Scriptable Objects/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [SerializeField]
    private string Name = "Level";

    [SerializeField]
    private List<SpawnWave> spawnWaves = new();

    [SerializeField]
    public float WaitBeforeStartingNextLevel = 5;

    public SpawnWave GetWave(int index)
    {
        if (spawnWaves.Count <= index)
        {
            return null;
        }
        return spawnWaves[index];
    }

    public List<SpawnWave> AllWaves()
    {
        return new List<SpawnWave>(spawnWaves);
    }

}


[System.Serializable]
public class SpawnWave
{
    public SpawnWaveConfig WaveConfig;

    [SerializeField]
    private QueueBehavior queueBehavior;
    public QueueBehavior QueueBehavior { get { return queueBehavior; } }

    public float WaitSecondsBeforeStarting = 5;
}
