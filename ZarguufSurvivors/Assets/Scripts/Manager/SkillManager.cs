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
    private int maxSkillsPerCategory;

    [SerializeField]
    private List<SkillConfig> skills;

    private List<SkillConfig> skillsThisRun = new();

    private Dictionary<SkillCategory, List<SkillConfig>> activatedSkills = new();

    void Start()
    {
        foreach (var skill in skills)
        {
            skillsThisRun.Add(Instantiate(skill));
        }

        //activatedSkills.Add(SkillCategory.Weapon, GetRandomSkillsWithCategory(1, SkillCategory.Weapon));
        activatedSkills.Add(SkillCategory.Weapon, skills.Where(x => x.SkillType == SkillType.ChainLightningProjectile).ToList());
    }

    public List<SkillConfig> GetRandomSkills(int number)
    {
        return skillsThisRun
            .OrderBy(x => System.Guid.NewGuid())
            .Take(number)
            .ToList();
    }

    public List<SkillConfig> GetRandomSkillsWithCategory(int number, SkillCategory cat)
    {
        return skillsThisRun
            .Where(x => x.SkillCategory == cat)
            .OrderBy(x => System.Guid.NewGuid())
            .Take(number)
            .ToList();
    }

    public bool isSkillActive(SkillType skillType)
    {
        SkillConfig conf = skills.FirstOrDefault(x => x.SkillType == skillType);

        if (conf == null)
        {
            Debug.LogError($"Missing config {skillType}");
        }

        if (activatedSkills.TryGetValue(conf.SkillCategory, out List<SkillConfig> list))
        {
            return list.Any(x => x.SkillType == skillType);
        }

        return false;
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
        SkillCategory category = skill.SkillCategory;

        if (activatedSkills.TryGetValue(category, out List<SkillConfig> skills))
        {
            bool hasFullSkills = skills.Count == maxSkillsPerCategory;
            bool isNewSkill = skills.All(x => x.name != skill.name);

            if (isNewSkill && hasFullSkills)
            {
                Debug.LogError("Can't levelup another new skill, max reached");
            }
            else if (isNewSkill)
            {
                skills.Add(skill);
            }
            
        }
        else
        {
            activatedSkills.Add(category, new() { skill });
        }

        skill.SkillUp();

            // TODO: Delete skills that were not selected if hasFullSkills
        if (activatedSkills.TryGetValue(category, out List<SkillConfig> list))
        {
            bool hasFullSkills = list.Count == maxSkillsPerCategory;
            
            if (hasFullSkills) {
                List<SkillConfig> toBeRemoved = skillsThisRun
                    .Where(x => x.SkillCategory == skill.SkillCategory)
                    .Where(x => list.Any(y => y.SkillType != x.SkillType))
                    .ToList();

                toBeRemoved.ForEach(x => skillsThisRun.Remove(x));
            }
        }
    }

}


public enum SkillType
{
    MagicMissile,
    Fireball,
    ProtectionScroll,
    PoisonCauldron,
    ChainLightningProjectile,
    FireCurseProjectile
}

public enum SkillCategory
{
    Weapon,
    Passive
}
