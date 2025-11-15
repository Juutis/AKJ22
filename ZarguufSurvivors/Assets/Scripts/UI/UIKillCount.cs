using TMPro;
using UnityEngine;

public class UIKillCount : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txtKillCount;
    void Start()
    {
        
    }

    private void OnEnable()
    {
        MessageBus.Subscribe<PlayerKillCountChange>(OnPlayerKillCountChange);
    }

    private void OnDisable()
    {
        MessageBus.Unsubscribe<PlayerKillCountChange>(OnPlayerKillCountChange);
    }

    private void OnPlayerKillCountChange(PlayerKillCountChange e)
    {
        txtKillCount.text = $"{e.KillCount}";
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
