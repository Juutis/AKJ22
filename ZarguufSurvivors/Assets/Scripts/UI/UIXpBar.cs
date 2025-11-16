using TMPro;
using UnityEngine;

public class UIXpBar : MonoBehaviour
{
    [SerializeField]
    private UIProgressBar progressBar;

    [SerializeField]
    private TextMeshProUGUI txtLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progressBar.SetPercentage(0);
    }


    private void OnEnable()
    {
        MessageBus.Subscribe<XpUpdatedEvent>(OnXpUpdatedEvent);
        MessageBus.Subscribe<LevelGainedEvent>(OnLevelGainedEvent);
    }

    private void OnDisable()
    {
        MessageBus.Unsubscribe<XpUpdatedEvent>(OnXpUpdatedEvent);
        MessageBus.Unsubscribe<LevelGainedEvent>(OnLevelGainedEvent);
    }

    private void OnXpUpdatedEvent(XpUpdatedEvent e)
    {
        float percentage = (float)(e.CurrentXp*1.0f / e.RequiredXp*1.0f);
        progressBar.SetPercentage(percentage);
    }



    private void OnLevelGainedEvent(LevelGainedEvent e)
    {
        txtLevel.text = $"level {e.CurrentLevel + 1}";
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
