using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class IntroSequence : MonoBehaviour
{
    public bool isSequencePlaying;
    public GameObject Prefab;
    public SpriteRenderer backgroundSprite;
    public GameObject tapToStartLabel;

    [Header("Cascade")]
    public Transform startPosition;
    public float
        sequencePreDelay = 1f,
        shotDuration, 
        timeBetweenShots = 0.25f, 
        shotDistance = 20f;
    public int numberToSpawn = 13;
    public float objectLifetime = 8;
    public Ease shotEase = Ease.Linear;
    float distanceBetweenSpawns;

    [Header("Fade")]
    public float preFadeDelay = 1f;
    public float fadeToWhiteDuration = 3f;
    public float fadeInDuration = 0.5f;

    [Header("Logo")]
    public CanvasGroup logoCanvasGroup;
    public float logoSequenceDelay = 0.5f;
    public float logoFadeTime = 1f;
    public Ease logoFadeEase = Ease.InCubic;

    public Vector3 logoScale = new(1, 1, 1);
    public float logoScaleDuration = 1f;
    public Ease logoScaleEase = Ease.InCubic;

    List<GameObject> drillboyList = new();
    Sequence introSequence;

    private void Start()
    {
        tapToStartLabel.SetActive(false);
        logoCanvasGroup.DOFade(0, 0);
    }

    public IEnumerator BeginSequence()
    {
        Time.timeScale = 1;
        isSequencePlaying = true;

        yield return StartCoroutine(CascadeSequence());
        yield return new WaitForSecondsRealtime(preFadeDelay);
        Invoke(nameof(KillDrillboys), fadeToWhiteDuration);
        Invoke(nameof(LogoSequence), fadeToWhiteDuration);

        HUD.Instance.screenFader.canvasGroup.alpha = 0;
        HUD.Instance.screenFader.image.color = Color.white;

        introSequence = DOTween.Sequence()
            .Append(HUD.Instance.screenFader.canvasGroup.DOFade(1, fadeToWhiteDuration).SetEase(Ease.OutSine).SetUpdate(UpdateType.Normal, true))
            .Append(HUD.Instance.screenFader.canvasGroup.DOFade(0, fadeInDuration).SetEase(Ease.OutSine).SetUpdate(UpdateType.Normal, true))
            .OnComplete(() => CompleteSequence());
    }

    void LogoSequence()
    {
        logoCanvasGroup.DOFade(1, logoFadeTime).SetEase(logoFadeEase);
        logoCanvasGroup.transform.DOScale(logoScale, logoScaleDuration).SetEase(logoScaleEase);
    }

    void CompleteSequence()
    {
        introSequence = null;
        tapToStartLabel.SetActive(true);
        isSequencePlaying = false;
    }

    //public IEnumerator BeginSequence()
    //{
    //    Time.timeScale = 1;
    //    isSequencePlaying = true;

    //    yield return new WaitForSecondsRealtime(sequencePreDelay);

    //    yield return StartCoroutine(CascadeSequence());
    //    yield return new WaitForSecondsRealtime(preFadeDelay);
    //    yield return StartCoroutine(LogoSequence());

    //    tapToStartLabel.SetActive(true);

    //    isSequencePlaying = false;
    //}

    // Immediately set everything to its completed state
    public void InterruptSequence()
    {
        StopAllCoroutines();
        CancelInvoke();

        introSequence.Kill();
        KillDrillboys();

        logoCanvasGroup.DOFade(1, 0);
        logoCanvasGroup.transform.DOScale(logoScale, 0);
        HUD.Instance.screenFader.KillFade();
        HUD.Instance.screenFader.SetAlpha(0);

        CompleteSequence(); 
    }

    IEnumerator CascadeSequence()
    {
        distanceBetweenSpawns = numberToSpawn / backgroundSprite.transform.localScale.x;

        for (int i = 0; i < numberToSpawn + 1; i++)
        {
            ShootDrillboy(startPosition.position + (distanceBetweenSpawns * i * Vector3.right));
            yield return new WaitForSecondsRealtime(timeBetweenShots);
        }
    }

    //IEnumerator FadeSequence()
    //{
    //    HUD.Instance.screenFader.FadeToWhite(fadeToWhiteDuration);
    //    yield return new WaitForSecondsRealtime(fadeToWhiteDuration);

    //    KillDrillboys();
    //    yield return new WaitForSecondsRealtime(logoSequenceDelay);
    //    StartCoroutine(LogoSequence());

    //    HUD.Instance.screenFader.FadeIn(fadeInDuration);
    //    yield return new WaitForSecondsRealtime(fadeInDuration);

    //}

    //IEnumerator LogoSequence()
    //{
    //    logo.DOFade(1, logoFadeTime).SetEase(logoFadeEase);
    //    logo.transform.DOScale(logoScale, logoScaleDuration).SetEase(logoScaleEase);

    //    yield return new WaitForSecondsRealtime(Mathf.Max(logoScaleDuration, logoFadeTime));

    //}

    void ShootDrillboy(Vector3 position)
    {
        var Drillboy = Instantiate(Prefab, position, Quaternion.identity, transform);
        var targetPosition = Drillboy.transform.position + (Vector3.down * shotDistance);
        Drillboy.transform.DOMove(targetPosition, shotDuration).SetUpdate(UpdateType.Normal, true).SetEase(shotEase);
        drillboyList.Add(Drillboy);
        Destroy(Drillboy, objectLifetime);
    }

    void KillDrillboys()
    {
        foreach (var drillboy in drillboyList)
        {
            if (drillboy)
            {
                Destroy(drillboy);
            }
        }

        drillboyList.Clear();
    }

}
