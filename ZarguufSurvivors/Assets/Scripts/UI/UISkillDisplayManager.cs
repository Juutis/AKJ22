using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UISkillDisplayManager : MonoBehaviour
{
    [SerializeField]
    private Transform skillContainer;
    [SerializeField]
    private UISkillDisplay uISkillDisplayPrefab;

    private List<UISkillDisplay> skills = new();

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
        var skillConfig = e.Skill;
        var existingSkill = skills.FirstOrDefault(skill => skill.Config == skillConfig);
        if (existingSkill == null)
        {
            var uiSkill = Instantiate(uISkillDisplayPrefab, skillContainer);
            uiSkill.Initialize(skillConfig);
            skills.Add(uiSkill);
        } else
        {
            existingSkill.UpdateLevel();
        }
    }
}

 