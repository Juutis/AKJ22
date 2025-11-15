using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Scriptable Objects/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [SerializeField]
    private float health;
    public float Health { get { return health; } }
    
    [SerializeField]
    private List<Sprite> sprites;
    public List<Sprite> Sprites { get { return sprites; } }

    [SerializeField]
    private float minSpeed;
    public float MinSpeed { get { return minSpeed; } }

    [SerializeField]
    private float maxSpeed;
    public float MaxSpeed { get { return maxSpeed; } }
}
