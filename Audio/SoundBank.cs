using UnityEngine.Audio;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEditor.SceneManagement;
using UnityEditor.Rendering;

[ExecuteInEditMode]
public class SoundBank : MonoBehaviour
{
    public SoundEffect[] soundEffects;
    public AudioMixerGroup sfxMixerGroup;
    public AudioSource soundTesterSource;
    public float soundCooldown = 0.12f;

    [HideInInspector]
    public SoundEffect
        // Gameplay SFX
        CollectLargeGem,
        CollectSmallGem,
        CollectArmor,
        CollectExtraLife,
        CheckpointReached,
        DestroyBlock,
        SuperDrill,
        TakeDamage,
        HitArmor,
        Die,

        // Menu SFX
        BuyItem,
        MenuClick;

    void Start()
    {
        LoadSounds();
    }

    public void LoadSounds()
    {
        soundEffects = Resources.LoadAll<SoundEffect>("SoundEffects");
        
        ClearAudioSources();

        // Gameplay Sounds
        CollectLargeGem = GetSoundByName(nameof(CollectLargeGem));
        CollectSmallGem = GetSoundByName(nameof(CollectSmallGem));
        CollectArmor = GetSoundByName(nameof(CollectArmor));
        CollectExtraLife = GetSoundByName(nameof(CollectExtraLife));
        CheckpointReached = GetSoundByName(nameof(CheckpointReached));
        DestroyBlock = GetSoundByName(nameof(DestroyBlock));
        TakeDamage = GetSoundByName(nameof(TakeDamage));
        HitArmor = GetSoundByName(nameof(HitArmor));
        SuperDrill = GetSoundByName(nameof(SuperDrill));
        Die = GetSoundByName(nameof(Die));

        // Menu Sounds
        BuyItem = GetSoundByName(nameof(BuyItem));
        MenuClick = GetSoundByName(nameof(MenuClick));

        soundTesterSource = gameObject.AddComponent<AudioSource>();
        soundTesterSource.playOnAwake = false;
        soundTesterSource.outputAudioMixerGroup = sfxMixerGroup;

        foreach (var s in soundEffects)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;
            s.source.outputAudioMixerGroup = sfxMixerGroup;
            s.canPlay = true;
        }
    }

    public void ClearAudioSources()
    {
        var audioSources = GetComponentsInChildren<AudioSource>();
        foreach (var audioSource in audioSources)
        {
            DestroyImmediate(audioSource);
        }
    }

    public void PlaySound(SoundEffect sound)
    {
        if (AudioManager.Instance.audioMuted) return;

        if (!sound.clip)
        {
            Debug.LogError($"Sound {sound.name} has no audio clip assigned.");
            return;
        }

        if (!sound.canPlay) return;

        if (sound.pitchRandomizationAmount > 0)
        {
            float pitch = Random.Range(1 - sound.pitchRandomizationAmount, 1 + sound.pitchRandomizationAmount);
            sound.source.pitch = pitch;
        }

        sound.source.PlayOneShot(sound.clip);
        StartCoroutine(AudioManager.Instance.SoundCooldown(sound));
        sound.source.pitch = 1.0f;
    }

    SoundEffect GetSoundByName(string soundName)
    {
        return Array.Find(soundEffects, s => s.name == soundName);
    }

}
