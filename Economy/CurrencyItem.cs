using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using Unity.Services.Core;
using Unity.Services;
using Unity.Services.Economy.Model;
using Unity.Services.Economy;
using UnityEngine.Android;
using System.Threading.Tasks;


[CreateAssetMenu(menuName = "Purchases/Purchasable Currency")]
public class CurrencyItem : PurchasableItem
{
    // TODO: Implement real-money transaction functionality

    public RewardType rewardType;
    public int quantity;

    // TODO this won't handle purchases that require multiple currencies
    // Need to read from the JSON in the Economy files?
    public override void Purchase()
    {
        base.Purchase();
        switch (rewardType)
        {
            case RewardType.Gems:
                BuyGems(quantity);
                break;

            case RewardType.PlayCurrency:
                BuyPlayCurrency(quantity);
                break;
        }
    }

    public async Task BuyGems(int amount = 1000)
    {
        AudioManager.Instance.soundBank.BuyItem.Play();  // TODO make this sound
        await Currency.Instance.IncrementCurrency(Currency.Instance.GemsId, amount);
    }

    public async Task BuyPlayCurrency(int amount = 50)
    {
        AudioManager.Instance.soundBank.BuyItem.Play();  // TODO make this sound
        await Currency.Instance.IncrementCurrency(Currency.Instance.PlayCurrencyId, amount);
    }

}
