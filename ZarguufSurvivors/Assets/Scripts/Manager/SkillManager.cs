using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkillManager : MonoBehaviour
{
    public static SkillManager main;

    void Awake()
    {
        main = this;
    }

    [SerializeField]
    private List<SkillConfig> skills;

    private List<SkillConfig> skillsThisRun = new();

    void Start()
    {
        foreach(var skill in skills)
        {
            skillsThisRun.Add(Instantiate(skill));
        }
    }

    public List<SkillConfig> GetRandomSkills(int number)
    {
        return skillsThisRun
            .OrderBy(x => System.Guid.NewGuid())
            .Take(number)
            .ToList();
    }

    public int GetSkillLevel(SkillType skillType)
    {
        return skillsThisRun.FirstOrDefault(skill => skill.SkillType == skillType).CurrentLevel;
    }

    private void OnEnable()
    {
        MessageBus.Subscribe<SkillLevelUpChosenEvent>(OnSkillLevelUpChosenEvent);
    }

    private void OnDisable()
    {
        MessageBus.Unsubscribe<SkillLevelUpChosenEvent>(OnSkillLevelUpChosenEvent);
    }

    private void OnSkillLevelUpChosenEvent(SkillLevelUpChosenEvent e)
    {
        SkillConfig skill = e.Skill;
        skill.SkillUp();
    }

}


public enum SkillType
{
    StaticProjectile,
    Cauldron,
    ProtectionScroll
}
