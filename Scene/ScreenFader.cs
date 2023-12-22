using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class ScreenFader : MonoBehaviour
{
    [HideInInspector]
    public CanvasGroup canvasGroup;
    [HideInInspector]
    public Image image;
    Tween fadeTween;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();  
        image = GetComponent<Image>();
    }

    public void KillFade()
    {
        fadeTween?.Kill();
    }

    public void SetAlpha(float value)
    {
        canvasGroup.alpha = value;
    }

    public void FadeIn(float duration)
    {
        canvasGroup.alpha = 1;
        fadeTween = canvasGroup.DOFade(0, duration).SetEase(Ease.OutQuint).SetUpdate(UpdateType.Normal, true)
            .OnComplete(() => fadeTween = null);
    }

    public void FadeOut(float duration)
    {
        canvasGroup.alpha = 0;
        image.color = Color.black;
        fadeTween = canvasGroup.DOFade(1, duration).SetEase(Ease.InQuint).SetUpdate(UpdateType.Normal, true)
            .OnComplete(() => fadeTween = null);
    }

    public void FadeToWhite(float duration)
    {
        canvasGroup.alpha = 0;
        image.color = Color.white;
        fadeTween = canvasGroup.DOFade(1, duration).SetEase(Ease.OutSine).SetUpdate(UpdateType.Normal, true)
            .OnComplete(() => fadeTween = null);

    }
}
