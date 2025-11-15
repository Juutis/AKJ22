using UnityEngine;

public class SpawnableMob : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private MoveTowardsPlayerEnemy mtpEnemy;
    private Damageable damageable;

    void Start()
    {
        mtpEnemy = GetComponent<MoveTowardsPlayerEnemy>();
        damageable = GetComponentInChildren<Damageable>();
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
        if (mtpEnemy != null)
        {
            mtpEnemy.Init();
            damageable.Init();
        }
    }

    public void Hurt()
    {
        
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
