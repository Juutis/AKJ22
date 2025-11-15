using UnityEngine;

public class SpawnableMob : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        
    }

    public void Initialize(int mobIndex, int waveIndex, Transform parent)
    {
        name = $"Mob[W{waveIndex} - {mobIndex}]";
        transform.parent = parent;
    }

    public void SetPosition(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    public void Begin()
    {
        // set up mob stats like health, xp drops etc based on some config
    }

    public void Kill()
    {
        // reset mob stats like health etc. here
        SpawnableMobPool.main.Kill(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
