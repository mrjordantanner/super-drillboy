using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;


public class SuperDrill : Skill
{

    public override void Use()
    {
        base.Use();
        var pm = PlayerManager.Instance;
        var player = pm.player;

        Data.triggerSound.Play();

        pm.invulnerable = true;
        pm.isBoosting = true;
        pm.isDashing = false;

        player.trails.enabled = true;
        player.trails.repeatRate = pm.boostTrailRepeatRate;
        player.trails.duration = pm.boostTrailDuration;
        player.trailRenderer.emitting = true;
        player.anim.SetBool("isSuperDashing", true);  // TODO add 'string animationClipName' to PlayerData

        GameManager.Instance.transposer.m_ScreenY = pm.boostingCamOffset;
        HUD.Instance.EnableGemIconEffects();
    }

    public override void ResetSkill()
    {
        base.ResetSkill();
        var pm = PlayerManager.Instance;
        var player = pm.player;

        pm.isBoosting = false;
        pm.invulnerable = false;

        player.trails.enabled = false;
        player.trails.repeatRate = pm.defaultSpriteTrailRepeatRate;
        player.trails.duration = pm.defaultSpriteTrailDuration;
        player.trailRenderer.emitting = false;
        player.anim.SetBool("isSuperDashing", false);

        GameManager.Instance.transposer.m_ScreenY = pm.normalCamOffset;
        HUD.Instance.DisableGemIconEffects();

        StartCoroutine(pm.DamageCooldown());
    }


}
