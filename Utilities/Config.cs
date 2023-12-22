using UnityEngine;


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
        #endregion

        Init();
    }

    [HideInInspector]
    public Setting MasterVolume, MusicVolume, SoundVolume;

    [HideInInspector]
    public Setting JoystickSize, JoystickOpacity, JoystickPositionX,  JoystickPositionY, JoystickRange, 
        //JoystickDeadZone,
        VariableSensitivity;

    [HideInInspector]
    public Setting
        LeftButtonOpacity, LeftButtonPositionX, LeftButtonPositionY, LeftButtonSize,
        RightButtonOpacity, RightButtonPositionX, RightButtonPositionY, RightButtonSize,
        DrillButtonOpacity, DrillButtonPositionX, DrillButtonPositionY, DrillButtonSize;

    public UserDto currentUserDto = new();

    [HideInInspector] public Setting[] allSettings, audioSettings, joystickSettings, buttonSettings;
    [HideInInspector] public Setting UseJoystick, GameMode, AutoSkill;

    private void Init()
    {
        allSettings = Resources.LoadAll<Setting>("Settings");
        audioSettings = Resources.LoadAll<Setting>("Settings/Audio");
        joystickSettings = Resources.LoadAll<Setting>("Settings/Joystick");
        buttonSettings = Resources.LoadAll<Setting>("Settings/Buttons");

        MasterVolume = GetSettingByName(nameof(MasterVolume));
        MusicVolume = GetSettingByName(nameof(MusicVolume));
        SoundVolume = GetSettingByName(nameof(SoundVolume));

        GameMode = GetSettingByName(nameof(GameMode));

        JoystickSize = GetSettingByName(nameof(JoystickSize));
        JoystickOpacity = GetSettingByName(nameof(JoystickOpacity));
        JoystickPositionX = GetSettingByName(nameof(JoystickPositionX));
        JoystickPositionY = GetSettingByName(nameof(JoystickPositionY));
        JoystickRange = GetSettingByName(nameof(JoystickRange));
        VariableSensitivity = GetSettingByName(nameof(VariableSensitivity));
        //JoystickDeadZone = GetSettingByName(nameof(JoystickDeadZone));

        LeftButtonOpacity = GetSettingByName(nameof(LeftButtonOpacity));
        LeftButtonPositionX = GetSettingByName(nameof(LeftButtonPositionX));
        LeftButtonPositionY = GetSettingByName(nameof(LeftButtonPositionY));
        LeftButtonSize = GetSettingByName(nameof(LeftButtonSize));

        RightButtonOpacity = GetSettingByName(nameof(RightButtonOpacity));
        RightButtonPositionX = GetSettingByName(nameof(RightButtonPositionX));
        RightButtonPositionY = GetSettingByName(nameof(RightButtonPositionY));
        RightButtonSize = GetSettingByName(nameof(RightButtonSize));

        DrillButtonOpacity = GetSettingByName(nameof(DrillButtonOpacity));
        DrillButtonPositionX = GetSettingByName(nameof(DrillButtonPositionX));
        DrillButtonPositionY = GetSettingByName(nameof(DrillButtonPositionY));
        DrillButtonSize = GetSettingByName(nameof(DrillButtonSize));

        UseJoystick = GetSettingByName(nameof(UseJoystick));
        AutoSkill = GetSettingByName(nameof(AutoSkill));

        LoadAllSettingsFromPlayerPrefs();

        ControlManager.Instance.ApplyJoystickSettings();
        ControlManager.Instance.ApplyButtonSettings();
    }

    public Setting GetSettingByName(string settingName)
    {
        foreach (var setting in allSettings)
        {
            if (setting.name == settingName)
            {
                return setting;
            }
        }

        Debug.LogError($"Unable to get Setting by name: {settingName}");
        return null;
    }

    public void LoadAllSettingsFromPlayerPrefs()
    {
        foreach (var setting in allSettings)
        {
            setting.LoadFromPlayerPrefs();
        }
    }

    public void SaveAllSettingsToPlayerPrefs()      
    {
        foreach (var setting in allSettings)
        {
            setting.SaveToPlayerPrefs();
        }
    }

    public void SaveUsernameToPlayerPrefs(string value) 
    {
        PlayerPrefs.SetString("Username", value);
        PlayerPrefs.Save();
        currentUserDto.PlayerName = value;
    }

    public void ResetSettingsToDefault(Setting[] settingsArray)
    { 
        foreach (var setting in settingsArray)
        {
            setting.SetToDefault();
        }
    }

    public static string GetPlayerPrefsString(string key)
    {
        if (PlayerPrefs.HasKey(key)) return PlayerPrefs.GetString(key);
        return string.Empty;
    }

    //public Setting LoadSettingFromPlayerPrefs(Setting setting)
    //{
    //    if (PlayerPrefs.HasKey(setting.name))
    //    {
    //        setting.Value = PlayerPrefs.GetFloat(setting.name);
    //        setting.PlayerPrefsValue = setting.Value;
    //        return setting;
    //    }

    //    setting.Value = setting.Default;
    //    return setting;
    //}

    //public void SaveSettingToPlayerPrefs(Setting setting)
    //{
    //    PlayerPrefs.SetFloat(setting.name, setting.Value);
    //    setting.PlayerPrefsValue = setting.Value;
    //    PlayerPrefs.Save();
    //}

     // Button callback
    public void ResetJoystickSettingsToDefault()
    {
        ResetSettingsToDefault(joystickSettings);
        ControlManager.Instance.ApplyJoystickSettings();
        Menu.Instance.controlsMenu.ExitJoystickEditMode();
        Menu.Instance.controlsMenu.RefreshControlsPanel();
    }

    // Button callback
    public void ResetInputButtonSettingsToDefault()
    {
        ResetSettingsToDefault(buttonSettings);
        ControlManager.Instance.ApplyButtonSettings();
        Menu.Instance.controlsMenu.ExitButtonEditMode();
        Menu.Instance.controlsMenu.RefreshControlsPanel();
    }

    // Button callback
    public void ResetAudioSettingsToDefault()
    {
        ResetSettingsToDefault(audioSettings);
    }

    // Button Callback
    public void SaveAudioSettings()
    {
        foreach (var setting in audioSettings)
        {
            setting.SaveToPlayerPrefs();
        }
    }

    // Button Callback
    public void SaveJoystickSettings()
    {
        foreach (var setting in joystickSettings)
        {
            setting.SaveToPlayerPrefs();
        }

    }

    // Button Callback
    public void SaveButtonSettings()
    {
        foreach (var setting in buttonSettings)
        {
            setting.SaveToPlayerPrefs();
        }

    }
}








