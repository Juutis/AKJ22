using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkillDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txtLevel;
    [SerializeField]
    private Image imgIcon;

    private SkillConfig skillConfig;
    public SkillConfig Config {get {return skillConfig;}}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize(SkillConfig config)
    {
        skillConfig = config;

        imgIcon.sprite = config.Icon;
        UpdateLevel();
    }

    public void UpdateLevel()
    {
        txtLevel.text = $"{skillConfig.CurrentLevel}";
    }
}
