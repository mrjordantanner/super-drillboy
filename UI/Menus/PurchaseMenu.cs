using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Menu Panel
/// </summary>
public class PurchaseMenu : MenuPanel
{
    public GameObject ButtonPrefab;
    public LayoutGroup layoutGroup;
    public string resourcesPath;

    PurchasableItem[] items;
    List<PurchaseButton> buttons = new();

    private void Start()
    {
        items = Resources.LoadAll<PurchasableItem>(resourcesPath);

        CreateButtons();
        RefreshScreen();
    }

    public void RefreshScreen()
    {
        // TODO clear/Create buttons
        // TODO Reinit all buttons?
        // TODO enable/disable buttons if player meets/does not meet purchase requirements
    }


    void CreateButtons()
    {
        ClearAllButtons();

        foreach (var item in items)
        {
            var NewButton = Instantiate(ButtonPrefab, layoutGroup.transform.position, Quaternion.identity, layoutGroup.transform);
            var button = NewButton.GetComponent<PurchaseButton>();
            button.Item = item;
            button.InitializeButton();
            buttons.Add(button);
        }
    }

    void ClearAllButtons()
    {
        foreach (var button in buttons)
        {
            Destroy(button.gameObject);
        }
        buttons.Clear();
    }



    public override void Show(float fadeDuration = 0.2f)
    {
        RefreshScreen();
        base.Show(fadeDuration);
    }

}
