using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Customizable InputButton control that is moveable, scalable, and can change opacity.
/// </summary>
public class CustomInputButton : CustomControl
{
    public Button button;
    public bool isButtonPressed;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        isButtonPressed = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        isButtonPressed = false;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (locked) return;

        Vector2 delta = (eventData.position - currentDragPosition) * dragFactor;
        transform.localPosition += new Vector3(delta.x, delta.y, 1);
        currentDragPosition = eventData.position;

        unlockedPosition = transform.localPosition;

        posXSetting.Value = unlockedPosition.x;
        posYSetting.Value = unlockedPosition.y;
    }


    public float CalculateDragFactor(float size)
    {
        var ratio = (-0.105f * size) + 0.49f;
        ratio = ratio < 0.023f ? 0.023f : ratio;
        ratio = ratio > 0.28f ? 0.28f : ratio;

        return size * ratio;
    }

    public override void Unlock()
    {
        button.enabled = false;
        base.Unlock();
    }

    public override void Lock()
    {
        button.enabled = true;
        transform.localPosition = unlockedPosition;

        base.Lock();
        Config.Instance.SaveButtonSettings();
    }

    public override void ApplySettings()
    {
        base.ApplySettings();
        ApplySizeSetting();
        ApplyPositionSetting();
    }

    public void ApplyPositionSetting()
    {
        transform.localPosition = new Vector2(posXSetting.Value, posYSetting.Value);
        unlockedPosition = transform.localPosition;
    }

    public void ApplySizeSetting()
    {
        transform.localScale = new Vector3(sizeSetting.Value, sizeSetting.Value, 1);
    }
}
