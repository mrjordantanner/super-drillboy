using UnityEngine;
using UnityEngine.UI;
using TMPro;


[ExecuteInEditMode]
[RequireComponent(typeof(Button))]
public class ButtonTextScaler : MonoBehaviour
{
    //public float scaleRatio = 0.75f;

    //Button button;  
    //TextMeshProUGUI textLabel;
    //RectTransform buttonRect;
    //RectTransform textLabelRect;
    //Vector3 lastScale;

    //void Start()
    //{
    //    button = GetComponent<Button>();
    //    textLabel = GetComponentInChildren<TextMeshProUGUI>();

    //    buttonRect = button.GetComponent<RectTransform>();
    //    textLabelRect = textLabel.GetComponent<RectTransform>();

    //    lastScale = buttonRect.localScale;
    //}

    //void OnEnable()
    //{
    //    button = GetComponent<Button>();
    //    textLabel = GetComponentInChildren<TextMeshProUGUI>();

    //    buttonRect = button.GetComponent<RectTransform>();
    //    textLabelRect = textLabel.GetComponent<RectTransform>();

    //    lastScale = buttonRect.localScale;
    //}

    //void Update()
    //{
    //    if (!Application.isPlaying)
    //    {
    //        CheckForScaleChanges();
    //    }
    //}

    //void CheckForScaleChanges()
    //{
    //    if (buttonRect.localScale != lastScale)
    //    {
    //        lastScale = buttonRect.localScale;
    //        ScaleText();
    //    }
    //}

    //private void ScaleText()
    //{

    //    // Calculate the scaling factor based on the button's size
    //    //float scaleFactor = Mathf.Min(buttonRect.rect.width / textLabelRect.rect.width, buttonRect.rect.height / textLabelRect.rect.height);

    //    //// Apply the scaling factor to the text label's font size
    //    //textLabel.fontSize *= scaleFactor * scaleRatio;
    //    float scaleFactor = Mathf.Min(buttonRect.sizeDelta.x, buttonRect.sizeDelta.y) / Mathf.Min(textLabel.rectTransform.sizeDelta.x, textLabel.rectTransform.sizeDelta.y);
    //    textLabel.fontSize *= scaleFactor;
    //    print($"Scale text: {scaleFactor}");
    //}


}
