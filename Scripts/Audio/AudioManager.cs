using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public enum EnemySoundType { Hit, Attack, Spawn, Die }

public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public Sound[] sounds;
    // public Dictionary<EnemySoundType, string> defaultEnemySounds = new();

    public float soundCooldown;
    public float pitchRandomizationAmount = 0.2f;

    [Header("Music")]
    public bool musicEnabled;
    public AudioClip titleScreenMusic;
    public AudioClip mainMusic;
    public AudioSource musicAudioSource;

    [Header("Mixer Groups")]
    public AudioMixerGroup sfxMixerGroup;
    public AudioMixerGroup musicMixerGroup;

    [Header("Audio Sliders")]
    public AudioSlider masterSlider;
    public AudioSlider musicSlider;
    public AudioSlider soundSlider;



    private void Start()
    {
        //defaultEnemySounds.Add(EnemySoundType.Hit, "Enemy-Hit");
        //defaultEnemySounds.Add(EnemySoundType.Die, "Enemy-Death");

        musicAudioSource.loop = true;
        musicAudioSource.outputAudioMixerGroup = musicMixerGroup;

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = sfxMixerGroup;
            s.canPlay = true;
        }

        Config.Instance.LoadAudioSettings();

        masterSlider.OnSliderValueChanged(masterSlider.slider.value);
        soundSlider.OnSliderValueChanged(soundSlider.slider.value);
        musicSlider.OnSliderValueChanged(musicSlider.slider.value);

        if (!musicEnabled)
        {
            print("MUSIC IS DISABLED");
        }
    }

    public void Play(string name, bool randomizePitch = false)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null || !s.canPlay)
        {
            return;
        }

        if (randomizePitch)
        {
            float pitch = Random.Range(1 - pitchRandomizationAmount, 1 + pitchRandomizationAmount);
            s.source.pitch = pitch;
        }

        s.source.PlayOneShot(s.clip);
        StartCoroutine(SoundCooldown(s));

        s.source.pitch = 1.0f;
    }

    IEnumerator SoundCooldown(Sound sound)
    {
        soundCooldown = Time.deltaTime;

        sound.canPlay = false;
        yield return new WaitForSecondsRealtime(soundCooldown);
        sound.canPlay = true;
    }

    public IEnumerator MusicTransition(AudioClip newTrack)
    {
        if (!musicEnabled) yield break;

        var tempVol = musicAudioSource.volume;

        musicAudioSource.DOFade(0, 1f).SetUpdate(UpdateType.Normal, true);
        yield return new WaitForSecondsRealtime(1f);

        musicAudioSource.Stop();
        yield return new WaitForSecondsRealtime(0.5f);

        musicAudioSource.clip = newTrack;
        musicAudioSource.loop = true;

        musicAudioSource.Play();
        musicAudioSource.DOFade(tempVol, 1f).SetUpdate(UpdateType.Normal, true);
    }

    public void PauseAllAudio(bool value)
    {
        AudioListener.pause = value;
    }
}

