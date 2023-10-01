using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using Random = UnityEngine.Random;


public class Config : MonoBehaviour
{
    #region Singleton
    public static Config Instance;

    private void Awake()
    {
        if (Application.isEditor)
            Instance = this;
        else
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

    }

    #endregion

    public UserDto currentUserDto = new();
    private const string UsernameKey = "Username";

    private const string MasterVolumeKey = "MasterVolume";
    private const string SoundVolumeKey = "SoundVolume";
    private const string MusicVolumeKey = "MusicVolume";

    [Header("Default Audio Settings")]
    public float defaultMasterVolume = 1f;
    public float defaultSoundVolume = 0.75f;
    public float defaultMusicVolume = 0.6f;

    public void SaveUsernameToPlayerPrefs(string value)
    {
        PlayerPrefs.SetString(UsernameKey, value);
        PlayerPrefs.Save();
        currentUserDto.PlayerName = value;
    }

    public string LoadUsernameFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(UsernameKey))
        {
            return PlayerPrefs.GetString(UsernameKey);
        }

        return string.Empty;
    }

    public void LoadAudioSettings()
    {
        float masterVolume = defaultMasterVolume;
        float musicVolume = defaultMusicVolume;
        float soundVolume = defaultSoundVolume;

        if (PlayerPrefs.HasKey(MasterVolumeKey))
        {
            masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey);
        }

        if (PlayerPrefs.HasKey(SoundVolumeKey))
        {
            soundVolume = PlayerPrefs.GetFloat(SoundVolumeKey);
        }

        if (PlayerPrefs.HasKey(MusicVolumeKey))
        {
            musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey);
        }

        AudioManager.Instance.masterSlider.slider.value = masterVolume;
        AudioManager.Instance.soundSlider.slider.value = soundVolume;
        AudioManager.Instance.musicSlider.slider.value = musicVolume;
    }


    public void SaveAudioSettings()
    {
        var masterSliderValue = AudioManager.Instance.masterSlider.slider.value;
        PlayerPrefs.SetFloat(MasterVolumeKey, masterSliderValue);

        var soundSliderValue = AudioManager.Instance.soundSlider.slider.value;
        PlayerPrefs.SetFloat(SoundVolumeKey, soundSliderValue);

        var musicSliderValue = AudioManager.Instance.musicSlider.slider.value;
        PlayerPrefs.SetFloat(MusicVolumeKey, musicSliderValue);

        PlayerPrefs.Save();
    }
}








