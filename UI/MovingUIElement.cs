using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class MovingUIElement : MonoBehaviour
{
    TextMeshProUGUI text;
    Image icon;

    public float duration = 1f;
    public Vector2 moveVector = new (0, 1f);
    [ReadOnly]
    public Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.localPosition;

        text = GetComponentInChildren<TextMeshProUGUI>();
        icon = GetComponentInChildren<Image>();
    }

    public void SetProperties(string textString, Color textColor, bool showIcon = false)
    {
        text.alpha = 1;
        text.color = textColor;
        text.text = textString;
        icon.enabled = showIcon;
    }

    public void TriggerElement()
    {
        text.alpha = 1;
        if (icon.enabled)
        {
            icon.DOFade(1, 0f);
        }

        transform.DOLocalMove(startPosition + (Vector3)moveVector, duration / 2).SetEase(Ease.OutQuint);
        StartCoroutine(Effects());
        StartCoroutine(ResetElement(duration));
    }

    IEnumerator Effects()
    {
        //var oldScale = text.transform.localScale;
        //var newScale = new Vector3(
        //    text.transform.localScale.x,
        //    text.transform.localScale.y,
        //    text.transform.localScale.z) * HUD.Instance.scaleSize;

        //// Scale up
        //text.transform.DOScale(newScale, HUD.Instance.scaleUpDuration).SetEase(Ease.OutElastic);
        //yield return new WaitForSeconds(HUD.Instance.scaleUpDuration);

        //// Scale down
        //text.transform.DOScale(oldScale, HUD.Instance.scaleDownDuration).SetEase(Ease.OutBounce);
        //yield return new WaitForSeconds(HUD.Instance.fadeDelay);

        yield return new WaitForSeconds(duration / 2);

        // Start fade halfway through the duration
        text.DOFade(0, duration / 2).SetEase(Ease.InQuint);
        icon.DOFade(0, duration / 2).SetEase(Ease.InQuint);
    }

    IEnumerator ResetElement(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        text.text = "";
        transform.DOLocalMove(startPosition, 0f);
    }
}

