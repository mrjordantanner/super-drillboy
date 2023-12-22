using System;
using System.Collections.Generic;
using UnityEngine;


// from https://forum.unity.com/threads/tutorial-character-stats-aka-attributes-system.504095/

public enum StatType
{
    MoveSpeed,
    FallSpeed,
    MaxArmor,
    //ArmorRegenMeters,           // +1 per X meters traveled
    MaxHealth,
    DrillSpeed,                 // Drill cooldown reduction
    PickupDistance,
    SkillCostReduction,         // -X%
    SkillChargesBonus           // +X on top of all Skill's baseMaxCharges
}

[Serializable]
[CreateAssetMenu(menuName = "Stats/Character Stat")]
public class CharacterStat : ScriptableObject//, IComparable<CharacterStat>
{
    public StatType Type;
    public float BaseValue;
    public float _value;
    float lastBaseValue;

    [HideInInspector] public bool isDirty = true;

    public List<StatModifier> statModifiers;

    public float Value
    {
        get
        {
            if (isDirty || lastBaseValue != BaseValue)
            {
                lastBaseValue = BaseValue;
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        }
    }

    //public CharacterStat(StatType type, float baseValue)
    //{
    //    statModifiers = new List<StatModifier>();
    //    Type = type;
    //    BaseValue = baseValue;
    //}

    public void AddModifier(StatModifier mod)
    {
        statModifiers.Add(mod);
        _value = CalculateFinalValue();
        isDirty = true;
    }

    public void RemoveModifier(StatModifier mod)
    {
        statModifiers.Remove(mod);
        _value = CalculateFinalValue();
        isDirty = true;
    }

    public void RemoveAllModifiersFromSource(StatModSource source)
    {
        foreach (var statMod in statModifiers)
            if (statMod.Source == source)
            {
                RemoveModifier(statMod);
                isDirty = true;
            }
    }

    float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0;

        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];

            switch (mod.ModType)
            {
                case StatModType.Flat:
                    finalValue += mod.Value;
                    break;

                case StatModType.PercentAdd:
                    sumPercentAdd += mod.Value;

                    // If we're at the end of the list OR the next modifer isn't of this type
                    if (i + 1 >= statModifiers.Count || statModifiers[i + 1].ModType != StatModType.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                    break;

                case StatModType.PercentMult:
                    finalValue *= 1 + mod.Value;
                    break;

                case StatModType.Multiply:
                    finalValue *= mod.Value;
                    break;
            }
        }

        return (float)Math.Round(finalValue, 4);
    }
}

