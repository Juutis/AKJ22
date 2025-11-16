using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UILevelUpMenu : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform skillContainer;

    [SerializeField]
    private UILevelUpSkill uiLevelUpSkillPrefab;

    private List<UILevelUpSkill> uiSkills = new();

    private List<SkillConfig> skillConfigs = new();

    [SerializeField]
    private TextMeshProUGUI txtCurrentLevel;

    private bool isOpen = false;
    private int currentLevel;

    private int maxSkills = 3;
    private int numberOfSkills;

    private void OnEnable()
    {
        MessageBus.Subscribe<LevelGainedEvent>(OnLevelGainedEvent);
    }

    private void OnDisable()
    {
        MessageBus.Unsubscribe<LevelGainedEvent>(OnLevelGainedEvent);
    }

    private void OnLevelGainedEvent(LevelGainedEvent e)
    {
        currentLevel = e.CurrentLevel;
        txtCurrentLevel.text = $"Level up! Level {currentLevel}";
        Show();
    }

    public void Show()
    {
        PopulateSkills();
        isOpen = true;
        animator.Play("uiLevelUpMenuShow");
        Time.timeScale = 0f;
        LevelManager.main.Pause();
    }

    public void AfterHide()
    {
        foreach(var uiSkill in uiSkills)
        {
            Destroy(uiSkill.gameObject);
        }
        uiSkills = new();
        skillConfigs = new();
        Time.timeScale = 1f;
        LevelManager.main.Unpause();
        isOpen = false;
    }

    public void Hide()
    {
        animator.Play("uiLevelUpMenuHide");
        isOpen = false;
    }

    private void PopulateSkills()
    {
        skillConfigs = SkillManager.main.GetRandomSkills(maxSkills);
        numberOfSkills = skillConfigs.Count;
        var index = 0;
        foreach(var skillConfig in skillConfigs)
        {
            var uiLevelUpSkill = Instantiate(uiLevelUpSkillPrefab, skillContainer);
            uiLevelUpSkill.Initialize(skillConfig, index);
            uiSkills.Add(uiLevelUpSkill);
            index += 1;
        }
    }

    private void ChooseSelectedSkill()
    {
        var skill = uiSkills.FirstOrDefault(uiSkill => uiSkill.IsSelected).SkillConfig;
        MessageBus.Publish(new SkillLevelUpChosenEvent(skill));
        Hide();
    }

    private void MoveSelectionDown()
    {
        var selectedSkill = uiSkills.FirstOrDefault(skill => skill.IsSelected);
        var nextIndex = selectedSkill.Index + 1;
        if (nextIndex < numberOfSkills)
        {
            selectedSkill.Deselect();
            uiSkills[nextIndex].Select();
        }
    }
    private void MoveSelectionUp()
    {
        var selectedSkill = uiSkills.FirstOrDefault(skill => skill.IsSelected);
        var nextIndex = selectedSkill.Index - 1;
        if (nextIndex >= 0)
        {
            selectedSkill.Deselect();
            uiSkills[nextIndex].Select();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            if (Keyboard.current == null)
            {
                return;
            }
            if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame)
            {
                MoveSelectionUp();
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame)
            {
                MoveSelectionDown();
            }
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                ChooseSelectedSkill();
            }
            /*if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                ChooseSkill(1);
            } else if (Keyboard.current.digit2Key.wasPressedThisFrame) {
                
                ChooseSkill(2);
            } else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                ChooseSkill(3);
            }*/
        }
    }

}


public struct SkillLevelUpChosenEvent : IEvent
{
    public SkillConfig Skill { get; }

    public SkillLevelUpChosenEvent(SkillConfig skill)
    {
        Skill = skill;
    }
}
