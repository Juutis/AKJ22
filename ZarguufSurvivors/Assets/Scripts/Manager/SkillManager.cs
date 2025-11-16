using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkillManager : MonoBehaviour
{
    public static SkillManager main;

    void Awake()
    {
        main = this;
        PopulateSkills();
    }

    [SerializeField]
    private int maxSkillsPerCategory;

    [SerializeField]
    private List<SkillConfig> skills;

    private List<SkillConfig> skillsThisRun = new();

    private int maxSkillLevel = 10;

    private Dictionary<SkillCategory, List<SkillConfig>> activatedSkills = new();

    void Start()
    {

        //activatedSkills.Add(SkillCategory.Weapon, GetRandomSkillsWithCategory(1, SkillCategory.Weapon));
        /*
        SkillConfig lightning = skillsThisRun.FirstOrDefault(x => x.SkillType == SkillType.FireCurseProjectile);
        lightning.SkillUp();
        activatedSkills.Add(SkillCategory.Weapon, new() { lightning });

        SkillConfig projectileBoost = skillsThisRun.FirstOrDefault(x => x.SkillType == SkillType.PickUpRangeBoost);
        projectileBoost.SkillUp();
        projectileBoost.SkillUp();
        activatedSkills.Add(SkillCategory.Passive, new() { projectileBoost });
        */
    }

    private void PopulateSkills()
    {
        if (skillsThisRun.Count == 0) {
            foreach (var skill in skills)
            {
                skillsThisRun.Add(Instantiate(skill));
            }
        }
    }

    public List<SkillConfig> GetJustWeapons(int number) {
        return skillsThisRun.Where(skill => skill.SkillCategory == SkillCategory.Weapon && skill.CurrentLevel < maxSkillLevel)
            .OrderBy(x => System.Guid.NewGuid())
            .Take(number)
            .ToList();
    }

    public List<SkillConfig> GetRandomSkills(int number)
    {
        return skillsThisRun
            .Where(skill => skill.CurrentLevel < maxSkillLevel)
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

    public bool IsSkillActive(SkillType skillType)
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
        return skillsThisRun.FirstOrDefault(skill => skill.SkillType == skillType)?.CurrentLevel ?? 0;
    }

    public float GetAttackCooldownMultiplier()
    {
        return 1f / (1f + 0.1f * GetSkillLevel(SkillType.AttackSpeedBoost));
    }

    public float GetAttackDamageMultiplier()
    {
        return 1f + 0.1f * GetSkillLevel(SkillType.DamageBoost);
    }

    public float GetXPBoostMultiplier()
    {
        return 1f + 0.1f * GetSkillLevel(SkillType.XPBoost);
    }

    public int GetProjectileCountAddition()
    {
        return 1 * GetSkillLevel(SkillType.ProjectileCountBoost);
    }

    public int GetPlayerMaxHealthAddition()
    {
        return 10 * GetSkillLevel(SkillType.HPBoost);
    }

    public int GetPlayerHealthAddition()
    {
        return 10;
    }

    public int GetMagnetSkillLevel() {
        return GetSkillLevel(SkillType.PickUpRangeBoost);
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

        if (activatedSkills.TryGetValue(category, out List<SkillConfig> skillsInCategory))
        {
            bool hasFullSkills = skillsInCategory.Count >= maxSkillsPerCategory;
            bool isNewSkill = skillsInCategory.All(x => x.SkillType != skill.SkillType);

            if (isNewSkill && hasFullSkills)
            {
                Debug.LogError("Can't levelup another new skill, max reached");
            }
            else if (isNewSkill)
            {
                skillsInCategory.Add(skill);
            }

        }
        else
        {
            activatedSkills.Add(category, new() { skill });
        }

        skill.SkillUp();

        // Delete skills that were not selected if hasFullSkills
        if (activatedSkills.TryGetValue(category, out List<SkillConfig> list))
        {
            bool hasFullSkills = list.Count >= maxSkillsPerCategory;

            if (hasFullSkills)
            {
                List<SkillConfig> toBeRemoved = skillsThisRun
                    .Where(x => x.SkillCategory == skill.SkillCategory)
                    .Where(x => !list.Any(y => y.SkillType == x.SkillType))
                    .ToList();

                toBeRemoved.ForEach(x => skillsThisRun.Remove(x));
            }

            Debug.Log($"Skill leveled! Total skills in {category} is {list.Count}. Is category full? {hasFullSkills}");
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
    FireCurseProjectile,
    DamageBoost,
    XPBoost,
    PickUpRangeBoost,
    MovementSpeedBoost,
    ProjectileCountBoost,
    AttackSpeedBoost,
    HPBoost
}

public enum SkillCategory
{
    Weapon,
    Passive
}
