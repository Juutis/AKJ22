using UnityEngine;

public class UIPlayerHealthBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    [SerializeField]
    private UIProgressBar progressBar;

    private void OnEnable()
    {
        MessageBus.Subscribe<PlayerHealthChangeEvent>(OnPlayerHealthChangeEvent);
    }

    private void OnDisable()
    {
        MessageBus.Unsubscribe<PlayerHealthChangeEvent>(OnPlayerHealthChangeEvent);
    }

    private void OnPlayerHealthChangeEvent(PlayerHealthChangeEvent e)
    {
        float percentage = (float)(e.CurrentHealth * 1.0f / e.MaxHealth * 1.0f);
        progressBar.SetPercentage(percentage);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
