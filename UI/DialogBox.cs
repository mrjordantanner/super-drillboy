using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Generic DialogBox class that sits on a prefab to be instantiated and destroyed at runtime.
/// </summary>
public class DialogBox : MonoBehaviour
{
    public TextMeshProUGUI header;
    public TextMeshProUGUI message;
    public TextMeshProUGUI description;
    public RectTransform panel;
    public Image modalBackground;
    public Image dialogImage;
    public Button okButton;
    public Button cancelButton;

    private Action onOkAction;
    private Action onCancelAction;

    public void Initialize(
        string headerText, 
        string messageText, 
        string descriptionText,
        Sprite sprite, 
        Action onOk, 
        Action onCancel)
    {
        header.text = headerText;
        message.text = messageText;
        description.text = descriptionText;
        dialogImage.sprite = sprite;
        onOkAction = onOk;
        onCancelAction = onCancel;

        okButton.onClick.AddListener(OnOkButton);
        cancelButton.onClick.AddListener(OnCancelButton);
    }

    public void SetOkAction(Action action)
    {
        onOkAction = action;
    }

    public void SetCancelAction(Action action)
    {
        onCancelAction = action;
    }

    void OnOkButton()
    {
        onOkAction?.Invoke();
        CloseDialog();
    }

    void OnCancelButton()
    {
        onCancelAction?.Invoke();
        CloseDialog();
    }

    void CloseDialog()
    {
        // TODO Additional cleanup or animation can be added here

        Destroy(gameObject);
    }
}
