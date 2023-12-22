using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages customization of and saving/loading/applying Settings for custom Input Control properties such as Opacity, Size, and Position.
/// </summary>
public class ControlManager : MonoBehaviour
{
    #region Singleton
    public static ControlManager Instance;
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
    #endregion

        Init();
    }

    [HideInInspector]
    public float unlockedImageOpacity = 0.15f;

    [Header("Joystick")]
    public CustomJoystick joystick;

    [Header("Input Buttons")]
    public bool buttonsLocked;
    public CanvasGroup inputButtonsCanvas;
    public CustomInputButton drillButton;
    public CustomInputButton leftButton, rightButton;

    [HideInInspector]
    public List<CustomInputButton> inputButtons;

    void Init()
    {
        inputButtons.Add(drillButton);
        inputButtons.Add(leftButton);
        inputButtons.Add(rightButton);

        buttonsLocked = true;
    }

    public void SaveToConfig()
    {
        Config.Instance.UseJoystick.Value = Menu.Instance.controlsMenu.controlsTabGroup.tabs[0].isSelected ? 1 : 0;
        Config.Instance.UseJoystick.SaveToPlayerPrefs();
    }


    public void ActivateMobileControls()
    {
        if (Config.Instance.UseJoystick.Value == 0)
        {
            ActivateButtonsUI();
        }
        else
        {
            ActivateJoystickUI();
        }
    }

    public void ActivateButtonsUI()
    {
        ShowButtons();
        ApplyButtonSettings();
        HideJoystick();
    }

    public void ActivateJoystickUI()
    {
        ShowJoystick();
        ApplyJoystickSettings();
        HideButtons();
    }

    // BUTTONS
    public void UnlockButtons()
    {
        foreach (var button in inputButtons)
        {
            button.Unlock();
        }

        buttonsLocked = false;
    }

    public void LockButtons()
    {
        foreach (var button in inputButtons)
        {
            button.Lock();
        }

        buttonsLocked = true;
        Config.Instance.SaveButtonSettings();
    }

    public void ApplyButtonSettings()
    {
        //print("ControlManager - Apply Button Settings");
        foreach (var button in inputButtons)
        {
            button.ApplySettings();
        }
    }

    public void ShowButtons()
    {
        //print("ControlManager - ShowButtons");
        Menu.Instance.FadeInCanvasGroup( inputButtonsCanvas, 0.1f);
        foreach (var button in inputButtons)
        {
            button.ShowControl();
        }
    }

    public void HideButtons()
    {
        //print("ControlManager - HideButtons");
        Menu.Instance.FadeOutCanvasGroup(inputButtonsCanvas, 0.1f);
        foreach (var button in inputButtons)
        {
            button.HideControl();
        }
    }

    // JOYSTICK
    public void UnlockJoystick()
    {
        joystick.Unlock();
    }

    public void LockJoystick()
    {
        joystick.Lock();
        Config.Instance.SaveJoystickSettings();
    }

    public void ApplyJoystickSettings()
    {
        //print("ControlManager - Apply Joystick Settings");
        joystick.ApplySettings();
    }

    public void ShowJoystick()
    {
        //print("ControlManager - Show Joystick");
        joystick.ShowControl();
    }

    public void HideJoystick()
    {
        //print("ControlManager - Hide Joystick");
        joystick.HideControl();
    }
}
