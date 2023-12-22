using UnityEngine;
using System.Linq;
using Unity.Services.Economy.Tools;
using Unity.Services.Economy;
using System;
using UnityEngine.Rendering;

/// <summary>
/// Base ScriptableObject class for purchasable Items.
/// </summary>
//[CreateAssetMenu(menuName = "Purchases/Purchase Data")]
public class PurchasableItem : ScriptableObject
{
    public string virtualPurchaseId;

    [Header("Item")]
    public string inventoryItemId;
    public string itemName;
    public string description;
    public Sprite icon;

    [Header("Price")]
    public CurrencyType currencyType;
    public float price;
    public int levelRequirement;

    [Header("Availability")]
    public int quantityAvailable = 0;   // 0 = Unlimited Qty

    public virtual void Init()
    {

    }

    public async virtual void Purchase() 
    {
        try
        {
            await EconomyService.Instance.Purchases.MakeVirtualPurchaseAsync(virtualPurchaseId);
            Debug.Log($"Virtual Purchase {virtualPurchaseId} of Item {itemName} ({inventoryItemId}) successful.");
        }
        catch (EconomyValidationException e)
        {
            Debug.LogError(e);
        }
        catch (EconomyRateLimitedException e)
        {
            Debug.LogError(e);
        }
        catch (EconomyException e)
        {
            Debug.LogError(e);
        }
        
    }

}
