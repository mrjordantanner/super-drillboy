using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using System;
using Unity.Services.Economy.Tools;

/// <summary>
/// Parent class for UI elements representing purchasable items.  UI elements populated from PurchaseData ScriptableObject.
/// </summary>
public class PurchaseButton : MonoBehaviour
{
    public PurchasableItem Item;

    [Header("Options")]
    public bool createDialogBox;

    [Header("UI Elements")]
    public TextMeshProUGUI itemNameLabel;
    public TextMeshProUGUI itemDescriptionLabel;
    public TextMeshProUGUI priceLabel;
    public TextMeshProUGUI limitedQuantityLabel;
    public Image itemImage;
    public Image currencyIcon;


    Action onOkAction, onCancelAction;
    Button button;

    public void InitializeButton()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        itemNameLabel.text = Item.itemName;
        itemDescriptionLabel.text = Item.description;
        priceLabel.text = $"${Item.price}";
        itemImage.sprite = Item.icon;
        limitedQuantityLabel.text = Item.quantityAvailable > 0 ? $"In Stock: {Item.quantityAvailable}" : "";

        DefineActions();
    }

    public void OnClick()
    {
        if (createDialogBox)
        {
            CreateDialogBox();
        }
        else
        {
            onOkAction?.Invoke();
        }
    }

    void DefineActions()
    {
        onOkAction = delegate ()
        {
            Item.Purchase();
            print($"Purchased {Item.itemName} for {Item.price}.");
        };

        onCancelAction = delegate ()
        {
            print("Transaction canceled.");
        };
    }

    void CreateDialogBox()
    {
        Menu.Instance.CreateDialogBox(
            "Confirm Purchase",
            $"Purchase {Item.itemName} for ${Item.price}?",
            Item.itemName.ToString(),
            Item.icon,
            onOkAction,
            onCancelAction);
    }
}
