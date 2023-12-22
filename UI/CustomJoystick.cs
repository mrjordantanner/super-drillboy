using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;


/// <summary>
/// Customizable Joystick control that is moveable, scalable, and can change opacity.
/// </summary>
public class CustomJoystick : CustomControl
{
    public OnScreenStick joystick;
    public Image joystickImage;
    public bool isVariableSensitivityOn = true;

    [HideInInspector]
    public RectTransform joystickTransform;

    public string
        RangeSettingName,
        VariableSensitivitySettingName;

    [HideInInspector]
    public Setting rangeSetting, variableSensitivitySetting;

    public override void Start()
    {
        base.Start();
        joystickTransform = GetComponent<RectTransform>();
        unlockedPosition = joystickImage.rectTransform.localPosition;
    }

    public override void GetSettingsFromConfig()
    {
        base.GetSettingsFromConfig();
        rangeSetting = Config.Instance.GetSettingByName(RangeSettingName);
        variableSensitivitySetting = Config.Instance.GetSettingByName(VariableSensitivitySettingName);

        settings.Add(rangeSetting);
        settings.Add(variableSensitivitySetting);
    }

    public override void ApplySettings()
    {
        base.ApplySettings();

        ApplySizeSetting();
        ApplyPositionSetting();
        ApplyRangeSetting();
        ApplyVariableSensitivitySetting();
        //ApplyDeadZoneSetting();
    }


    public override void Unlock()
    {
        base.Unlock();
        joystick.enabled = false;
    }

    public override void Lock()
    {
        joystick.enabled = true;

        base.Lock();
        Config.Instance.SaveJoystickSettings();
    }


    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (locked) return;

        dragFactor = CalculateDragFactor(sizeSetting.Value);

        Vector2 delta = (eventData.position - currentDragPosition) * dragFactor;
        joystickImage.rectTransform.localPosition += new Vector3(delta.x, delta.y, 1);
        currentDragPosition = eventData.position;

        unlockedPosition = joystickImage.rectTransform.localPosition;

        posXSetting.Value = unlockedPosition.x;
        posYSetting.Value = unlockedPosition.y;
    }

    public void ApplyPositionSetting()
    {
        joystickImage.rectTransform.localPosition = new Vector2(posXSetting.Value, posYSetting.Value);
        unlockedPosition = joystickImage.rectTransform.localPosition;
    }

    public void ApplySizeSetting()
    {
        joystickTransform.localScale = new Vector3(sizeSetting.Value, sizeSetting.Value, 1);
    }

    public void ApplyRangeSetting()
    {
        joystick.movementRange = Config.Instance.JoystickRange.Value;
    }

    public void ApplyVariableSensitivitySetting()
    {
        isVariableSensitivityOn = variableSensitivitySetting.Value == 1;
    }

    //public void ApplyDeadZoneSetting()
    //{
    //    InputSystem.settings.defaultDeadzoneMin = deadZoneSetting.Value;
    //}

    public float CalculateDragFactor(float size)
    {
        var ratio = (-0.105f * size) + 0.49f;
        ratio = ratio < 0.023f ? 0.023f : ratio;
        ratio = ratio > 0.28f ? 0.28f : ratio;

        return size * ratio;
    }

}
