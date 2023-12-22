using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MasterSettingsSlider : MonoBehaviour, IPointerUpHandler
{ 
    public SettingsSlider[] childSliders;
    public Slider slider;
    public TextMeshProUGUI valueText;

    [Header("Value Style")]
    [Range(0, 2)]
    public int decimalPlaces;
    public bool displayAsPercentage;

    private void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        SetValueText();

        //foreach (var childSlider in childSliders)
        //{
        //    childSlider.slider.interactable = false;
        //}
    }

    public virtual void OnSliderValueChanged(float newSliderValue)
    {
        foreach (var childSlider in childSliders)
        {
            //childSlider.setting.Value = slider.value;
            childSlider.slider.value = slider.value;
        }

        SetValueText();
    }

    void SetValueText()
    {
        valueText.text = displayAsPercentage ? $"{(slider.value * 100).ToString("F" + decimalPlaces)}%" : slider.value.ToString("F" + decimalPlaces);
    }

    public void Refresh()
    {
        foreach (var childSlider in childSliders)
        {
            var settingVal = childSlider.setting.Value;
            childSlider.slider.value = settingVal;
        }

        SetValueText();
    }

    // Update value if user clicks on slider instead of sliding handle
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        //print("On pointer up - parent");

        slider.value = slider.minValue + (slider.maxValue - slider.minValue) * eventData.position.x / slider.GetComponent<RectTransform>().rect.width;
        OnSliderValueChanged(slider.value);

    }
}
