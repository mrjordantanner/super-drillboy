using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;


public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI message;
    public float messageScaleSize = 1.25f;
    public float messageScaleUpDuration = 0.1f,
        messageScaleDownDuration = 0.2f,
        messageFadeOutDuration = 1;

    public Vector3 normalTextScale;
    Color defaultColor;
    private Sequence currentMessageSequence;

    public bool teleportMessageDisplaying;

    Button teleportButton;

    private void Start()
    {
        normalTextScale = message.transform.localScale;
        defaultColor = ColorPalette.Instance.colors[5];

        teleportButton = GetComponentInChildren<Button>();
        if (teleportButton != null)
        {
            teleportButton.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        PlayerManager.Instance.Teleport();
    }

    public void ShowMessage(string text, float messageFadeDelay = 0.3f)
    {
        message.text = "";
        message.color = defaultColor;

        if (currentMessageSequence != null)
        {
            currentMessageSequence.Kill();
            message.transform.DOScale(normalTextScale, 0f);
            message.DOFade(1, 0f).OnComplete(() => DisplayNewMessage(text, messageFadeDelay));
        }
        else
        {
            DisplayNewMessage(text, messageFadeDelay);
        }
    }

    private void DisplayNewMessage(string text, float messageFadeDelay)
    {
        message.text = text;
        currentMessageSequence = DOTween.Sequence()
            .Append(message.DOFade(1, 0f))
            .Append(message.transform.DOScale(normalTextScale * messageScaleSize, messageScaleUpDuration).SetEase(Ease.OutElastic))
            .Append(message.transform.DOScale(normalTextScale, messageScaleDownDuration).SetEase(Ease.OutBounce))
            .AppendInterval(messageFadeDelay)
            .Append(message.DOFade(0, messageFadeOutDuration).SetEase(Ease.OutQuint))
            .OnComplete(() => currentMessageSequence = null);
    }

    public void ClearMessage()
    {
        currentMessageSequence?.Kill();
        currentMessageSequence = DOTween.Sequence()
            .Append(message.transform.DOScale(normalTextScale, 0f))
            .Append(message.DOFade(1, 0.05f)
            .OnComplete(() => currentMessageSequence = null));
        message.text = "";
    }

    // TODO
    public void CreateNotification()
    {
        throw new System.NotImplementedException();
        // e.g. Show Armor +1 featuring a small Armor Icon, etc
    }

}
