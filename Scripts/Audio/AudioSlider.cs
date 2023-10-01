using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


[RequireComponent(typeof(Slider))]
public class AudioSlider : MonoBehaviour
{
    public AudioMixer mixer;
    public string mixerParameter;
    [HideInInspector]
    public Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }
    public void OnSliderValueChanged(float value)
    {
        var newValue = slider.value < 0.01 ? -80f : Mathf.Log10(value) * 20;
        mixer.SetFloat(mixerParameter, newValue);
    }
}
