using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private UnityEvent onDamageReceived;

    [SerializeField]
    private UnityEvent onKilled;

    private float currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("DebugHurt", Random.Range(1.0f, 3.0f));
    }

    public void DebugHurt()
    {
        Hurt(1);
        Invoke("DebugHurt", Random.Range(1.0f, 3.0f));
    }

    public void Init()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hurt(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            onKilled.Invoke();
        }
        else
        {
            onDamageReceived.Invoke();
        }
    }
}
