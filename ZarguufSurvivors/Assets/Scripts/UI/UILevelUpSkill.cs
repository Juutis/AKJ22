using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelUpSkill : MonoBehaviour
{
    private SkillConfig skillConfig;
    public SkillConfig SkillConfig { get { return skillConfig; } }

    [SerializeField]
    private Image imgIcon;

    [SerializeField]
    private TextMeshProUGUI txtTitle;
    [SerializeField]
    private TextMeshProUGUI txtDescription;
    [SerializeField]
    private TextMeshProUGUI txtLevel;
    [SerializeField]
    private TextMeshProUGUI txtKey;

    [SerializeField]
    private Color highlightColor;
    private Color originalColor;
    [SerializeField]
    private Color borderHighlightColor;
    private Color borderOriginalColor;

    [SerializeField]
    private GameObject selectionIndicator;

    [SerializeField]
    private Image imgBg;

    [SerializeField]
    private Image imgBorder;
    
    [SerializeField]
    private Sprite goldIcon;

    private bool isSelected = false;
    public bool IsSelected { get { return isSelected; } }

    private int index;
    public int Index {get {return index;}}

    void Start()
    {

    }

    public void InitializeEmpty()
    {
        originalColor = imgBg.color;
        borderOriginalColor = imgBorder.color;

        txtLevel.text = "";
        txtTitle.text = "Gold";
        txtDescription.text = $"Just gold. What do you do with it? I don't know.";
        txtKey.text = $"{index + 1}";
        imgIcon.sprite = goldIcon;
        index = 0;
        if (index == 0)
        {
            Select();
        }
    }


    public void Initialize(SkillConfig skill, int index)
    {
        originalColor = imgBg.color;
        borderOriginalColor = imgBorder.color;
        skillConfig = skill;
        if (skill.CurrentLevel == 0)
        {
            txtLevel.text = "<Not leveled yet>";
        } else
        {
            txtLevel.text = $"Level {skill.CurrentLevel}";
        }
        txtTitle.text = $"{skill.Title}";
        txtDescription.text = $"{skill.Description}";
        txtKey.text = $"{index + 1}";
        imgIcon.sprite = skill.Icon;
        this.index = index;
        if (index == 0)
        {
            Select();
        }
    }

    public void Highlight()
    {
        imgBg.color = highlightColor;
        imgBorder.color = borderHighlightColor;
        selectionIndicator.SetActive(true);
    }

    public void Unhighlight()
    {
        imgBg.color = originalColor;
        imgBorder.color = borderOriginalColor;
        selectionIndicator.SetActive(false);
    }

    public void Select()
    {
        Highlight();
        isSelected = true;
    }

    public void Deselect()
    {
        Unhighlight();
        isSelected = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
