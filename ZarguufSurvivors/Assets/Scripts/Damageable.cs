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
        if (currentHealth <= 0)
        {
            onKilled.Invoke();
            MessageBus.Publish(new MobWasKilledEvent(name));
            killedAlready = true;
            ScreenShake.Instance.Shake(2.5f);
        }
        else
        {
            onDamageReceived.Invoke();
            ScreenShake.Instance.Shake(0.5f);
        }
    }
}

public struct MobWasKilledEvent : IEvent
{
    public string Name;

    public MobWasKilledEvent(string name)
    {
        Name = name;
    }
}