using UnityEngine;
using DG.Tweening;


public class ScreenFader : MonoBehaviour
{
    public bool isFading;
    public CanvasGroup faderCanvasGroup;

    public void SetAlpha(float alpha)
    {
        faderCanvasGroup.alpha = alpha;
    }

    public void FadeIn(float duration)
    {
        faderCanvasGroup.alpha = 1;
        faderCanvasGroup.DOFade(0, duration).SetEase(Ease.OutSine).SetUpdate(UpdateType.Normal, true);
    }

    public void FadeOut(float duration)
    {
        faderCanvasGroup.alpha = 0;
        faderCanvasGroup.DOFade(1, duration).SetEase(Ease.OutSine).SetUpdate(UpdateType.Normal, true);
    }
}
