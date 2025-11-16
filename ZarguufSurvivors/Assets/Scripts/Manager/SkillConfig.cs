using UnityEngine;

[CreateAssetMenu(fileName = "SkillConfig", menuName = "Scriptable Objects/SkillConfig")]
public class SkillConfig : ScriptableObject
{
    [SerializeField]
    private SkillCategory skillCategory;
    public SkillCategory SkillCategory { get { return skillCategory; } }

    [SerializeField]
    private SkillType skillType;
    public SkillType SkillType { get { return skillType; } }

    [SerializeField]
    private Sprite icon;
    public Sprite Icon { get { return icon; } }

    [SerializeField]
    private int currentLevel = 0;
    public int CurrentLevel { get { return currentLevel; } }

    [SerializeField]
    private string title;

    public string Title { get { return title; } }

    [SerializeField]
    [TextArea]
    private string description;
    public string Description { get { return description; } }

    public void SkillUp()
    {
        currentLevel += 1;
    }

}
