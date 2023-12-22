using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ControlsMenu : MonoBehaviour
{
    [Header("Panels")]
    public CanvasGroup ControlsPanel;
    public CanvasGroup
        JoystickPanel,
        JoystickEditModePanel, 
        InputButtonsPanel, 
        ButtonsEditModePanel;

    [Header("Joystick UI Elements")]
    public TextMeshProUGUI EditJoystickButtonText;
    public TextMeshProUGUI EditJoystickInstructionsText;
    public TextMeshProUGUI joystickValueX, joystickValueY;
    public Toggle variableSensitivityToggle;
    public TabGroup settingsTabGroup, controlsTabGroup;

    [Header("Joystick Edit Mode")]
    public float joystickEditModeOpacity = 0.5f;

    [Header("Input Button UI Elements")]
    public TextMeshProUGUI EditInputButtonsButtonText;
    public TextMeshProUGUI EditInputButtonsInstructionsText;

    Vector2 testInput;


    private void Start()
    {
        if (variableSensitivityToggle)
        {
            variableSensitivityToggle.onValueChanged.AddListener(OnVariableSensitivityChanged);
            variableSensitivityToggle.isOn = Config.Instance.VariableSensitivity.Value == 1;
        }
    }

    private void Update()
    {
        if (HUD.Instance.mobileUI.alpha == 0) return;

        testInput = PlayerManager.Instance.playerInput.actions["MoveAndDrill"].ReadValue<Vector2>();
        testInput.y = testInput.y < 0 ? -1 : 0;

        if (!ControlManager.Instance.joystick.isVariableSensitivityOn)
        {
            if (testInput.x > 0) testInput.x = 1;
            else if (testInput.x < 0) testInput.x = -1;
        }

        joystickValueX.text = testInput.x.ToString("F2");
        joystickValueY.text = testInput.y.ToString();
    }

    void OnVariableSensitivityChanged(bool value)
    {
        ControlManager.Instance.joystick.isVariableSensitivityOn = value;
        Config.Instance.VariableSensitivity.Value = value ? 1 : 0;
    }


    // Button callback
    public void RefreshControlsPanel()
    {
        //print("Controls Menu - RefreshControlsPanel");

        if (Config.Instance.UseJoystick.Value == 0)
        {
            controlsTabGroup.SelectTab(controlsTabGroup.tabs[0]);
            ControlManager.Instance.ActivateButtonsUI();
        }
        else
        {
            controlsTabGroup.SelectTab(controlsTabGroup.tabs[1]);
            ControlManager.Instance.ActivateJoystickUI();
            variableSensitivityToggle.isOn = ControlManager.Instance.joystick.isVariableSensitivityOn;
        }

        Menu.Instance.RefreshAllSliders();
        settingsTabGroup.RefreshSelectedPanel();
        controlsTabGroup.RefreshSelectedPanel();
    }

    // Button callback
    public void ToggleJoystickEditMode()
    {
        if (ControlManager.Instance.joystick.locked)
        {
            EnterJoystickEditMode();
        }
        else
        {
            ExitJoystickEditMode();
        }
    }

    public void EnterJoystickEditMode()
    {
        Menu.Instance.SettingsMenuPanel.Hide(0.1f);
        JoystickEditModePanel.ignoreParentGroups = true;

        ControlManager.Instance.UnlockJoystick();
        EditJoystickButtonText.text = "LOCK\r\nJOYSTICK";
        EditJoystickInstructionsText.text = "Drag Joystick to move.\r\nTap LOCK JOYSTICK when finished.";
    }

    public void ExitJoystickEditMode()
    {
        Menu.Instance.SettingsMenuPanel.Show(0.1f);
        JoystickEditModePanel.ignoreParentGroups = false;

        ControlManager.Instance.LockJoystick();
        Config.Instance.SaveAllSettingsToPlayerPrefs();
        EditJoystickButtonText.text = "MOVE\r\nJOYSTICK";
        EditJoystickInstructionsText.text = "Test Joystick behavior\r\nand appearance here.";
    }


    // Button callback
    public void ToggleButtonEditMode()
    {
        if (ControlManager.Instance.buttonsLocked)
        {
            EnterButtonEditMode();
        }
        else
        {
            ExitButtonEditMode();
        }
    }

    public void EnterButtonEditMode()
    {
        Menu.Instance.SettingsMenuPanel.Hide(0.1f);
        ButtonsEditModePanel.ignoreParentGroups = true;

        ControlManager.Instance.ShowButtons();
        ControlManager.Instance.UnlockButtons();
        EditInputButtonsButtonText.text = "LOCK\r\nBUTTONS";
        EditInputButtonsInstructionsText.text = "Drag buttons to move.\r\nTap LOCK BUTTONS when finished.";
    }

    public void ExitButtonEditMode()
    {
        Menu.Instance.SettingsMenuPanel.Show(0.1f);
        ButtonsEditModePanel.ignoreParentGroups = false;

        ControlManager.Instance.LockButtons();
        Config.Instance.SaveAllSettingsToPlayerPrefs();
        EditInputButtonsButtonText.text = "MOVE\r\nBUTTONS";
        EditInputButtonsInstructionsText.text = "Preview button appearance here.";
    }

    public void CloseJoystickPanel()
    {
        if (JoystickPanel.alpha > 0) Menu.Instance.FadeOutCanvasGroup(JoystickPanel);
        if (JoystickEditModePanel.alpha > 0) Menu.Instance.FadeOutCanvasGroup(JoystickEditModePanel);
    }

    public void CloseInputButtonsPanel()
    {
        if (InputButtonsPanel.alpha > 0) Menu.Instance.FadeOutCanvasGroup(InputButtonsPanel);
        if (ButtonsEditModePanel.alpha > 0) Menu.Instance.FadeOutCanvasGroup(ButtonsEditModePanel);
    }
}
