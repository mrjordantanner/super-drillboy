using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// Parent class for any input control that is moveable, scalable, and can change opacity.
/// </summary>
public class CustomControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public CanvasGroup canvasGroup;
    public Image unlockedImage;
    public bool locked;

    public float dragFactor;

    [ReadOnly]
    public bool isDragging;
    [ReadOnly]
    public Vector2 currentDragPosition;
    [ReadOnly]
    public Vector2 unlockedPosition;

    public string
        OpacitySettingName,
        SizeSettingName,
        PositionXSettingName,
        PositionYSettingName;

    [HideInInspector]
    public Setting posXSetting, posYSetting, opacitySetting, sizeSetting;
    [HideInInspector]
    public List<Setting> settings = new();

    public virtual void Start()
    {
        GetSettingsFromConfig();

        locked = true;
        unlockedPosition = transform.localPosition;
    }

    public virtual void GetSettingsFromConfig()
    {
        posXSetting = Config.Instance.GetSettingByName(PositionXSettingName);
        posYSetting = Config.Instance.GetSettingByName(PositionYSettingName);
        opacitySetting = Config.Instance.GetSettingByName(OpacitySettingName);
        sizeSetting = Config.Instance.GetSettingByName(SizeSettingName);

        settings.Add(posXSetting);
        settings.Add(posYSetting);
        settings.Add(opacitySetting);
        settings.Add(sizeSetting);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (locked) return;
        isDragging = true;
        currentDragPosition = eventData.position;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (locked) return;
        isDragging = false;
    }

    public virtual void OnDrag(PointerEventData eventData) { }

    public virtual void Unlock()
    {
        locked = false;
        unlockedImage.DOFade(ControlManager.Instance.unlockedImageOpacity, 0.3f).SetUpdate(UpdateType.Normal, true);
    }

    public virtual void Lock()
    {
        locked = true;
        unlockedImage.DOFade(0, 0.3f).SetUpdate(UpdateType.Normal, true);

        posXSetting.Value = unlockedPosition.x;
        posYSetting.Value = unlockedPosition.y;
    }

    public virtual void ApplySettings()
    {
        GetSettingsFromConfig();
        ApplyOpacitySetting();
    }

    public void ShowControl()
    {
        Menu.Instance.ToggleCanvasGroup(true, canvasGroup, opacitySetting.Value, 0.1f);
    }

    public void HideControl()
    { 
        Menu.Instance.ToggleCanvasGroup(false, canvasGroup, 0, 0.1f);
    }

    public void ApplyOpacitySetting()
    {
        canvasGroup.alpha = opacitySetting.Value;
    }


}
