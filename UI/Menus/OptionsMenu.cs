using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

public class OptionsMenu : MonoBehaviour
{
    public CanvasGroup Panel;
    public TMP_Dropdown vibrationDropdown;
    public TMP_InputField vibrationCooldownInputField;

    public Toggle autoDrillToggle;
    public Toggle allowHorizDrillingToggle;

    // TODO move these to config
    Setting AllowHorizDrilling, AutoDrill;



    // TODO
    // Make this more generic
    // Make arrays of Toggles, Dropdowns, etc

    private void Start()
    {
        AllowHorizDrilling = Config.Instance.GetSettingByName(nameof(AllowHorizDrilling));
        AutoDrill = Config.Instance.GetSettingByName(nameof(AutoDrill));

        vibrationCooldownInputField.text = GameManager.Instance.vibrationCooldown.ToString();
        PopulateVibrationDropdown();
        ApplySettings();
    }

    public void ToggleOnClick()
    {
        SaveToConfig();
        ApplySettings();
    }

    // BUG:  AllowHorizDrilling is not saving/loading properly but AutoDrillng is

    public void RefreshMenu()
    {
        autoDrillToggle.isOn = AutoDrill.Value == 1;
        allowHorizDrillingToggle.isOn = AllowHorizDrilling.Value == 1;
    }

    float inputFieldValue;
    public void ApplySettings()
    {
        PlayerManager.Instance.autoDrill = AutoDrill.Value == 1;
        PlayerManager.Instance.allowHorizontalDrilling = AllowHorizDrilling.Value == 1;
 
        if (float.TryParse(vibrationCooldownInputField.text, out inputFieldValue))
        {
            GameManager.Instance.vibrationCooldown = inputFieldValue;
        }
        else
        {
            Debug.LogError("Unable to parse vibrationCooldown input field contents to float.");
        }
    }

    public void SaveToConfig()
    {
        AutoDrill.Value = autoDrillToggle.isOn ? 1 : 0;
        AllowHorizDrilling.Value = allowHorizDrillingToggle.isOn ? 1 : 0;

        AutoDrill.SaveToPlayerPrefs();
        AllowHorizDrilling.SaveToPlayerPrefs();
    }

    void PopulateVibrationDropdown()
    {
        vibrationDropdown.ClearOptions();

        var enumValues = System.Enum.GetValues(typeof(GameManager.VibrationStyle));
        var enumNames = System.Enum.GetNames(typeof(GameManager.VibrationStyle));

        var options = new List<TMP_Dropdown.OptionData>();

        for (int i = 0; i < enumValues.Length; i++)
        {
            options.Add(new TMP_Dropdown.OptionData(enumNames[i]));
        }

        vibrationDropdown.AddOptions(options);
    }

}
