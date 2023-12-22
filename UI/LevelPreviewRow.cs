using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPreviewRow : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI levelName;

    public void Set(Sprite iconSprite, string name)
    {
        icon.sprite = iconSprite;
        levelName.text = name;
    }
}
