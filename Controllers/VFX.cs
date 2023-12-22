using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;
using System.Collections;

public class VFX : MonoBehaviour
{
    #region Singleton
    public static VFX Instance;
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

    [HideInInspector]
    public GameObject VFXContainer;

    public Volume GlobalConstantEffects;

    [Header("Damage Effects")]
    public Volume DamageEffects;
    public float 
        damageEffectsFadeInDuration = 0.1f, 
        damageEffectsFadeOutDuration = 0.5f;

    bool damageEffectsFadeIn, damageEffectsFadeOut;
    float effectsTimer;

    //[Header("Fade Between Volumes")]
    //PostProcessVolume effectsVolume_From, effectsVolume_To;
    //bool fadingBetweenVolumes;
    //float fadeBetweenDuration, fadeBetweenStartTime, fadeBetweenTimer;

    void Init()
    {
        VFXContainer = new GameObject("VFXContainer");
    }

    void Update()
    {
        //if (fadingBetweenVolumes) ProcessFadeBetweenVolumes();
        if (damageEffectsFadeIn) ProcessDamageEffectsFade(DamageEffects, 1);
        if (damageEffectsFadeOut) ProcessDamageEffectsFade(DamageEffects, 0);
    }

    public IEnumerator StartDamageEffects()
    {
        HUD.Instance.ScreenFlash();

        DamageEffectsFadeIn();
        yield return new WaitForSeconds(damageEffectsFadeInDuration);

        DamageEffectsFadeOut();
        yield return new WaitForSeconds(damageEffectsFadeOutDuration);
    }

    public void DamageEffectsFadeIn()
    {
        damageEffectsFadeIn = true;
        damageEffectsFadeOut = false;
        effectsTimer = 0;
    }

    public void DamageEffectsFadeOut()
    {
        damageEffectsFadeIn = false;
        damageEffectsFadeOut = true;
        effectsTimer = 0;
    }

    void ProcessDamageEffectsFade(Volume volume, float targetValue)
    {
        var startingValue = targetValue == 1 ? 0 : 1;
        var duration = targetValue == 1 ? 0.1f : 0.5f;

        effectsTimer += Time.deltaTime;
        var weight = Mathf.SmoothStep(startingValue, targetValue, Mathf.Clamp01(effectsTimer / duration));
        volume.weight = weight;

        if (effectsTimer >= duration)
        {
            effectsTimer = 0;

            damageEffectsFadeIn = false;
            damageEffectsFadeOut = false;
        }
    }

    // Fade between two Post Processing volumes
    //public void FadeBetweenVolumes(PostProcessVolume _effectsVolume_From, PostProcessVolume _effectsVolume_To, float _duration)
    //{
    //    fadingBetweenVolumes = true;
    //    fadeBetweenDuration = Time.time;
    //    effectsVolume_From = _effectsVolume_From;
    //    effectsVolume_To = _effectsVolume_To;
    //    fadeBetweenDuration = _duration;
    //}

    //void ProcessFadeBetweenVolumes()
    //{
    //    float t = (Time.time - startTime) / duration;
    //    effectsVolume_From.weight = Mathf.SmoothStep(1, 0, t);
    //    effectsVolume_To.weight = Mathf.SmoothStep(0, 1, t);

    //    if (effectsVolume_From.weight < 0.01f)
    //    {
    //        effectsVolume_From.weight = 0;
    //        fadingBetweenVolumes = false;
    //    }

    //    if (effectsVolume_To.weight > 0.99f)
    //    {
    //        effectsVolume_To.weight = 1;
    //        fadingBetweenVolumes = false;
    //    }
    //}
}
