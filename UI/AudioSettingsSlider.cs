using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class AudioSettingsSlider : SettingsSlider
{
    public AudioMixer mixer;
    public string mixerParameter;

    void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public override void OnSliderValueChanged(float value)
    {
        base.OnSliderValueChanged(value);
        AudioManager.Instance.SetMixerValue(mixer, mixerParameter, slider.value);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        print("on pointer up -- child");
        if (mixerParameter == "SoundVolume")
        {
            AudioManager.Instance.soundBank.CollectSmallGem.Play();
        }

    }


}
