using TMPro;
using UnityEngine;

public class UIRunTimer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txtTimer;
    private bool isStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Initialize()
    {
        
    }

    public void Begin()
    {
        isStarted = true;
    }

    private void OnEnable()
    {
        MessageBus.Subscribe<GameDurationUpdatedEvent>(OnRunTimeUpdated);
    }

    private void OnDisable()
    {
        MessageBus.Unsubscribe<GameDurationUpdatedEvent>(OnRunTimeUpdated);
    }

    private void OnRunTimeUpdated(GameDurationUpdatedEvent e)
    {
        // Format the time and update the UI text
        int minutes = (int)(e.CurrentRunTime / 1000 / 60);
        int seconds = (int)(e.CurrentRunTime / 1000 % 60);
        txtTimer.text = $"{minutes:00}:{seconds:00}";
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted)
        {
            
        }
    }
}
