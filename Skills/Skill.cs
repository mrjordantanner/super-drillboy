using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Base class for all Skills.
/// </summary>
public class Skill : MonoBehaviour
{
    public SkillData Data;
    public bool IsInUse;
    [HideInInspector] public float Timer;

    void Update()
    {
        if (IsInUse && Data.duration > 0)
        {
            HandleSkillTimer();
        }

    }

    void HandleSkillTimer()
    {
        Timer -= Time.deltaTime;
        if (Timer <= 0)
        {
            ResetSkill();
        }

    }

    public virtual void Use()
    {
        IsInUse = true;
        Timer = Data.duration;
    }

    public virtual void ResetSkill()
    {
        IsInUse = false;


    }

}
