using UnityEngine;

[CreateAssetMenu(fileName = "SpawnWaveConfig", menuName = "Scriptable Objects/SpawnWaveConfig")]
public class SpawnWaveConfig : ScriptableObject
{
    [SerializeField]
    private EnemyConfig enemyConfig;

    public EnemyConfig EnemyConfig { get { return enemyConfig; } }

    [SerializeField]
    private SpawnFormation formation;
    public SpawnFormation Formation {get {return formation;}}

    [SerializeField]
    private int amount = 5;
    public int Amount {get {return amount;}}

    [SerializeField]
    private SpawnTiming timing;
    public SpawnTiming Timing { get { return timing; } }

    [SerializeField]
    [Tooltip("Only if timing is Interval")]
    private float interval;
    public float Interval { get { return interval; } }

    [SerializeField]
    [Tooltip("Only if timing is Interval")]
    private int amountPerInterval = 1;
    public int AmountPerInterval { get { return amountPerInterval; } }

}

public enum SpawnFormation
{
    CircleAroundPlayer,
    Grouped,
}

public enum SpawnTiming
{
    Instant,
    Interval
}
public enum QueueBehavior
{
    WaitsForPrevious,
    StartsWhenPreviousStarts,
    StartsWhenLevelStarts
}