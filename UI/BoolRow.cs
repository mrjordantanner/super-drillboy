using UnityEngine;
using UnityEngine.UI;

public class BoolRow : MonoBehaviour
{
    Color trueColor, falseColor;
    //[HideInInspector]
    public Image image;
    [HideInInspector]
    public bool value;

    private void Start()
    {
        trueColor = ColorPalette.Instance.colors[1];
        falseColor = ColorPalette.Instance.colors[7];

        image = GetComponentInChildren<Image>();
    }

    public void SetValue(bool value)
    {
        image.color = value ? trueColor : falseColor;
    }


}
