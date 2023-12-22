using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CollapsibleSectionButton : MonoBehaviour
{
    public bool isSectionExpanded;
    Button toggleButton;
    public GameObject toggleImage;
    public GameObject collapsibleSection;

    void Start()
    {
        isSectionExpanded = false;
        toggleButton = GetComponent<Button>();
        toggleButton.onClick.AddListener(ToggleSection);

        ToggleSection();
    }

    void ToggleSection()
    {
        isSectionExpanded = !isSectionExpanded;
        ToggleSection(isSectionExpanded);
    }

    void ToggleSection(bool enable)
    {
        collapsibleSection.SetActive(enable);
        LayoutRebuilder.ForceRebuildLayoutImmediate(collapsibleSection.GetComponent<RectTransform>());
        var rotation = enable ? 0 : 90;
        toggleImage.transform.DORotate(new Vector3(0, 0, rotation), 0.05f).SetUpdate(UpdateType.Normal, true);
    }

}
