using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailsRow : MonoBehaviour
{
    public DetailsPanel.Detail Detail;
    public TextMeshProUGUI detailLabel;
    public TextMeshProUGUI valueLabel;
    public Image icon;

    public void Set(string value)
    {
        valueLabel.text = value;

        icon.sprite = Detail.icon;
        detailLabel.text = Detail.text;

        detailLabel.color = Detail.isDifficulty ? PlayScreen.Instance.diffDetailColor : PlayScreen.Instance.rewardDetailColor;
        valueLabel.color = Detail.isDifficulty ? PlayScreen.Instance.diffValueColor : PlayScreen.Instance.rewardValueColor;
    }
}
