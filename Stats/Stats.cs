using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;
using System.Collections;

public class Stats : MonoBehaviour
{
    #region Singleton
    public static Stats Instance;
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
        #endregion

        Init();
    }

    public int health, armor;

    public CharacterStat[] AllCharacterStats;
    public StatModifier[] AllStatMods;

    public CharacterStat
        MoveSpeed,
        //FallSpeed,
        DrillSpeed,
        MaxHealth,
        MaxArmor,
        //ArmorRegenMeters,
        PickupDistance,
        SkillCostReduction,
        SkillChargesBonus;

    //StatModifier
    //    MaxHealthMod,
    //    MaxArmorMod,
    //    MoveSpeedMod,
    //    PickupDistanceMod;

    //float
    //    baseMoveSpeed = 7.5f,
    //    baseDrillSpeed = 0.1f,
    //    //baseArmorRegenMeters = 500,
    //    basePickupDistance = 0.6f,
    //    baseSkillCostReduction = 0;

    //int
    //    baseMaxHealth = 3,
    //    baseMaxArmor = 3,
    //    baseSkillChargesBonus = 0;


    public void Init()
    {
        InitializeCharacterStats();
        InitializeStatModifiers();
    }

    void InitializeCharacterStats()
    {
        AllCharacterStats = Resources.LoadAll<CharacterStat>("CharacterStats");

        MoveSpeed = GetStat(StatType.MoveSpeed);
        DrillSpeed = GetStat(StatType.DrillSpeed);
        MaxHealth = GetStat(StatType.MaxHealth);
        MaxArmor = GetStat(StatType.MaxArmor);
        PickupDistance = GetStat(StatType.PickupDistance);
        SkillCostReduction = GetStat(StatType.SkillCostReduction);
    }
    
    void InitializeStatModifiers()
    {
        AllStatMods = Resources.LoadAll<StatModifier>("StatModifiers");
    }

    public CharacterStat GetStat(StatType type)
    {
        foreach (var stat in AllCharacterStats)
        {
            if (stat.Type == type)
            {
                return stat;
            }
        }

        Debug.LogError($"Unable to get CharacterStat of Type: {type}");
        return null;
    }

    public StatModifier GetStatMod(string statModName)
    {
        foreach (var statMod in AllStatMods)
        {
            if (statMod.name == statModName)
            {
                return statMod;
            }
        }

        Debug.LogError($"Unable to get StatModifier by name: {statModName}");
        return null;
    }

    public void ResetHealth()
    {
        health = (int)MaxHealth.Value;
        HUD.Instance.hitpointsUI.CreateAllIcons();
    }

    public void ResetArmor()
    {
        armor = (int)MaxArmor.Value;
        HUD.Instance.hitpointsUI.CreateAllIcons();
    }
}
