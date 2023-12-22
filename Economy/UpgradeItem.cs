using UnityEngine;

[CreateAssetMenu(menuName = "Purchases/Upgrade Item")]
public class UpgradeItem : PurchasableItem
{
    [Header("Upgrade")]
    public StatModifier statMod;
    public StatType statType;
    public StatModSource source;


    public void Init()
    {

    }


    public async override void Purchase()
    {
        base.Purchase();
        Inventory.Instance.AddInventoryItem(inventoryItemId);

        statMod.Source = source;
        var stat = Stats.Instance.GetStat(statType);
        stat.AddModifier(statMod);
    }


    

}
