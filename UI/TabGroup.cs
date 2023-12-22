using TMPro;
using UnityEngine;


public class TabGroup : MonoBehaviour
{
    public bool selectFirstTabOnStartup = true;

    [Header("Unselected Colors")]
    public Color textColor;
    public Color outlineColor, fillColor;

    [Header("Selected Colors")]
    public Color selectedTextColor;
    public Color selectedOutlineColor, selectedFillColor;

    public Sprite outlineSprite, outlineSelectedSprite, fillSprite;
    //[HideInInspector]
    public Tab[] tabs;

    private void Start()
    {
        tabs = GetComponentsInChildren<Tab>();

        if (selectFirstTabOnStartup)
        {
            for (int i = 0; i < tabs.Length; i++)
            {
                if (i == 0)
                {
                    SelectTab(tabs[i]);
                }
                else
                {
                    DeselectTab(tabs[i]);
                }
            }
        }

    }

    // Button callback
    public void RefreshSelectedPanel()
    {
        foreach (var tab in tabs)
        {
            if (tab.isSelected)
            {
                SelectTab(tab);
            }
        }
    }

    // Button callback
    public void SelectTab(Tab tab)
    {
        tab.isSelected = true;
        tab.outline.sprite = outlineSelectedSprite;
        tab.outline.color = selectedOutlineColor;
        tab.fill.color = selectedFillColor;
        tab.label.color = selectedTextColor;
        tab.label.fontStyle = FontStyles.Bold;

        if (tab.panelsToShow.Length > 0)
        {
            foreach (var panel in tab.panelsToShow)
            {
                ActivatePanel(panel);
            }
        }

        DeselectOtherTabs(tab);
    }

    void DeselectTab(Tab tab)
    {
        tab.isSelected = false;
        tab.outline.sprite = outlineSprite;
        tab.outline.color = outlineColor;
        tab.fill.color = fillColor;
        tab.label.color = textColor;
        tab.label.fontStyle = FontStyles.Normal;

        if (tab.panelsToShow.Length > 0)
        {
            foreach (var panel in tab.panelsToShow)
            {
                DeactivatePanel(panel);
            }
        }
    }

    void DeselectOtherTabs(Tab selectedTab)
    {
        foreach (var tab in tabs)
        {
            if (tab != selectedTab)
            {
                DeselectTab(tab);
            }
        }
    }

    void ActivatePanel(CanvasGroup panel)
    {
        Menu.Instance.FadeInCanvasGroup(panel, 0.1f);
    }

    void DeactivatePanel(CanvasGroup panel)
    {
        Menu.Instance.FadeOutCanvasGroup(panel, 0.1f);
    }

}
