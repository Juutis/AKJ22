using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Scriptable Objects/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [SerializeField]
    private string Name = "Level";

    [SerializeField]
    private List<SpawnWave> spawnWaves = new();

    public SpawnWave GetWave(int index)
    {
        if (spawnWaves.Count <= index)
        {
            return null;
        }
        return spawnWaves[index];
    }

}


[System.Serializable]
public class SpawnWave
{
    public SpawnWaveConfig WaveConfig;
    public float WaitSecondsBeforeStarting = 5;
}