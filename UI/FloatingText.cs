using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class FloatingText : MonoBehaviour
{
    TextMeshProUGUI text;
    Image icon;

    private void Awake()
    {
        icon = GetComponentInChildren<Image>();
    }

    public void SetProperties(string textString, Color textColor, bool showIcon = false)
    {
        Destroy(gameObject, 2f);

        text = GetComponent<TextMeshProUGUI>();
        text.color = textColor;
        text.text = textString;

        icon.enabled = showIcon;
        StartCoroutine(Effects());
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

        yield return new WaitForSeconds(0.5f);

        // Fade
        text.DOFade(0, 0.5f).SetEase(Ease.InQuint);
        icon.DOFade(0, 0.5f).SetEase(Ease.InQuint);



    }
}

