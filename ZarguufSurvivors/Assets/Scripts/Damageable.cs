using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onDamageReceived;

    [SerializeField]
    private UnityEvent onKilled;

    private float currentHealth;
    private bool killedAlready;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void DebugHurt()
    {
        Hurt(1);
        Invoke("DebugHurt", Random.Range(1.0f, 3.0f));
    }

    public void Init(float maxHealth)
    {
        currentHealth = maxHealth;
        killedAlready = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Hurt(float damage)
    {
        if (killedAlready)
        {
            return;
        }
        currentHealth -= damage;
        SoundManager.main.PlaySound(GameSoundType.EnemyHit);
        if (currentHealth <= 0)
        {
            var spawnableMob = GetComponentInParent<SpawnableMob>();
            Debug.Log($"found spawnablemob: {spawnableMob}");
            if (spawnableMob != null)
            {
                MessageBus.Publish(new MobWasKilledEvent(spawnableMob.Config));
            }
            onKilled.Invoke();
            killedAlready = true;
            ScreenShake.Instance.Shake(2.5f);
        }
        else
        {
            onDamageReceived.Invoke();
            ScreenShake.Instance.Shake(0.5f);
        }
    }

    public bool IsKilled()
    {
        return killedAlready;
    }
}

public struct MobWasKilledEvent : IEvent
{
    public EnemyConfig EnemyConfig;

    public MobWasKilledEvent(EnemyConfig enemyConfig)
    {
        EnemyConfig = enemyConfig;
    }
}