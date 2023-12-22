using UnityEngine.Audio;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections;


[CreateAssetMenu(menuName = "Audio/Sound Effect")]
[ExecuteInEditMode]
[Serializable]
public class SoundEffect : ScriptableObject
{
    //public string soundName;
    public AudioClip clip;

    [Range(0, 1)]
    public float volume = 0.5f;

    [Range(0, 3)]
    public float pitch = 1;
    [Range(0, 0.5f)]
    public float pitchRandomizationAmount = 0;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
    [HideInInspector]
    public bool canPlay;

    public void Play()
    {
        if (Application.isPlaying)
        {
            AudioManager.Instance.soundBank.PlaySound(this);
        }
        else
        {
            var soundBank = FindObjectOfType<SoundBank>();

            soundBank.soundTesterSource.clip = clip;
            soundBank.soundTesterSource.volume = volume;
            soundBank.soundTesterSource.pitch = pitchRandomizationAmount > 0 ? Random.Range(1 - pitchRandomizationAmount, 1 + pitchRandomizationAmount) : pitch;

            soundBank.soundTesterSource.PlayOneShot(clip);
        }
    }
}
