using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using System;


/// <summary>
/// Handles references to, gaining, and switching Skills.
/// </summary>
public class SkillController : MonoBehaviour
{
    #region Singleton
    public static SkillController Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

    }
    #endregion

    public bool autoSkillUse;  // TODO set this up in Config and UI
  // move to DevTools

    [Header("State")]
    [ReadOnly] public Skill CurrentSkill;
    [ReadOnly] public int skillCharges;
    [ReadOnly] public int resource;

    [Header("UI")]
    public Slider resourceBar;
    public TextMeshProUGUI resourceLabel, resourceCostLabel;
    public Toggle autoSkillToggle;
    public SkillChargesUI skillChargesUI;

    [HideInInspector] public Skill[] Skills;
    [HideInInspector] public SkillData[] SkillDatas;
    Skill SuperDrill;
    SkillData SuperDrillData;

    public void Init()
    {
        Skills = GetComponents<Skill>();
        SkillDatas = Resources.LoadAll<SkillData>("Skills");

        SuperDrill = GetComponent<SuperDrill>();
        SuperDrillData = GetSkillDataByName("Super Drill");
        SuperDrill.Data = SuperDrillData;
        
        SetSkill(SuperDrill);
        ResetResource();

        var autoSkillValue = Config.Instance.AutoSkill.Value == 1;
        SetAutoSkill(autoSkillValue);
        autoSkillToggle.isOn = autoSkillValue;
    }

    public void OnAutoSkillToggleChanged()
    {
        var value = autoSkillToggle.isOn;
        SetAutoSkill(value);
    }

    void SetAutoSkill(bool value)
    {
        autoSkillUse = value;

        Config.Instance.AutoSkill.Value = value ? 1 : 0;
        Config.Instance.AutoSkill.SaveToPlayerPrefs();

        skillChargesUI.enabled = !value;
        skillChargesUI.Container.SetActive(!value);
    }

    private void Update()
    {
        if (GameManager.Instance.gameRunning && !GameManager.Instance.inputSuspended && !GameManager.Instance.gamePaused)
        {
            if (Input.GetKey(KeyCode.UpArrow) && !autoSkillUse && skillCharges > 0 && !CurrentSkill.IsInUse)
            {
                UpdateSkillCharges(-1);
                ResetResource();
                CurrentSkill.Use();
            }
        }
    }

    public void ResetResource()
    {
        resource = 0;
        UpdateResource();
    }

    public void UpdateResource()
    {
        resourceBar.value = resource;
        resourceLabel.text = resource.ToString();
    }

    public void ResetSkillCharges()
    {
        skillCharges = 0;
        skillChargesUI.CreateIcons();
    }

    public void UpdateSkillCharges(int updateAmount)
    {
        skillChargesUI.RefreshWithPop(skillCharges);
        skillCharges += updateAmount;
    }

    public void UpdateSkillCost()
    {
        resourceCostLabel.text = CurrentSkill.Data.Cost.ToString();
        resourceBar.maxValue = CurrentSkill.Data.Cost;
    }

    public SkillData GetSkillDataByName(string skillName)
    {
        //return Array.Find(Skills, skill => skill.Data.skillName == skillName);
        foreach (var skillData in SkillDatas)
        {
            if (skillData.skillName == skillName)
            {
                return skillData;
            }
        }

        Debug.LogError($"SkillData for {skillName} not found in SkillDatas Array");
        return null;
    }

    public void SetSkill(Skill newSkill)
    {
        CurrentSkill = newSkill;
        UpdateSkillCost();

        // TODO set selected skill icon in HUD
    }

    public void SetResourceToMax()
    {
        var valueNeededForMax = CurrentSkill.Data.Cost - resource;
        GainResource(valueNeededForMax);
    }

    public void GainResource(int amount)
    {
        // Short circuit if we're already at Max charges
        if (!autoSkillUse && skillCharges == CurrentSkill.Data.maxSkillCharges) return;

        resource += amount;
        UpdateResource();

        if (resource >= CurrentSkill.Data.Cost)
        {
            // Use Skill automatically right away
            if (autoSkillUse)
            {
                resource -= CurrentSkill.Data.Cost;
                UpdateResource();

                CurrentSkill.Use();
            }

            // No AutoSkill, gain a Skill Charge
            else if (!autoSkillUse
                && skillCharges < CurrentSkill.Data.maxSkillCharges)
            {
                UpdateSkillCharges(1);

                // If we've reached Max charges, don't update progress bar or label
                if (skillCharges == CurrentSkill.Data.maxSkillCharges)
                {
                    resource = 0;
                    resourceBar.value = resourceBar.maxValue;
                }
                // Otherwise, update them
                else
                {
                    resource -= CurrentSkill.Data.Cost;
                    UpdateResource();
                }
            }

            // Already at Max, don't update progress bar or label
            else if (!autoSkillUse
                && skillCharges == CurrentSkill.Data.maxSkillCharges)
            {
                resource = 0;
                resourceBar.value = resourceBar.maxValue;
            }
        }
        
    }

}
