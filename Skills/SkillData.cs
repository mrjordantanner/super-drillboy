using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Holds data for Skills.
/// </summary>
[CreateAssetMenu(menuName = "Skills/Skill Data")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public int levelRequirement;
    public bool hasBeenAcquired;

    public int baseSkillCost;
    public int maxSkillCharges;

    public float duration;

    public SoundEffect triggerSound;
    public Sprite icon;

    public int Cost
    {
        get
        {
            var cost = baseSkillCost * (1 - Stats.Instance.SkillCostReduction.Value);
            return (int)cost;
        }
    }

}
