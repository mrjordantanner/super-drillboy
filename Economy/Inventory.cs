using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using Unity.Services.Economy.Model;
using Unity.Services.Economy;

/// <summary>
/// Singleton. Stores local data of purchased items and interacts with Economy Cloud service.
/// </summary>
[ExecuteInEditMode]
public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    [Header("Test Add")]
    public string testAddItemId;

    [Header("Test Delete")]
    public string testDeletePlayersInventoryItemId;

    // INVENTORY ITEM IDs
    // Unlockable Menus
    [HideInInspector] public readonly string Menu_Shop = "MENU_SHOP";
    [HideInInspector] public readonly string Menu_Upgrade = "MENU_UPGRADE";
    [HideInInspector] public readonly string Menu_Skills = "MENU_SKILLS";

    // Levels
    [HideInInspector] public readonly string Level_TheUndergarden = "LEVEL_THE_UNDERGARDEN";

    // Skills
    [HideInInspector] public readonly string Skill_SuperDrill = "SKILL_SUPER_DRILL";

    // Suits
    [HideInInspector] public readonly string Suit_Default = "SUIT_DEFAULT";

    public List<PlayersInventoryItem> localInventory = new();
    List<string> initialInventoryItems = new();

    //public struct LocalItemRecord
    //{
    //    public string Id;
    //    public bool Unlocked;
    //}

    public async Task Init()
    {
        if (!Application.isPlaying) return;

        initialInventoryItems = new()
        {
            Level_TheUndergarden,
            Skill_SuperDrill,
            Suit_Default
        };

        await SyncLocalDataWithCloud();
    }

    public async Task<List<PlayersInventoryItem>> SyncLocalDataWithCloud()
    {
        // Optional, defaults to 20
        var options = new GetInventoryOptions
        {
            ItemsPerFetch = 50
        };

        var inventoryResult = await EconomyService.Instance.PlayerInventory.GetInventoryAsync(options);
        var items = inventoryResult.PlayersInventoryItems;

        if (items.Count < initialInventoryItems.Count)
        {
            print($"Inventory: No inventory items found in Cloud. Adding {initialInventoryItems.Count} initial Inventory items.");
            await AddInitialInventoryItems();
            return items;
        }

        localInventory = new();
        foreach (var item in items)
        {
            localInventory.Add(item);
        }

        print($"Inventory: Fetched {items.Count} Inventory Items from Cloud.");

        return items;

        //if (inventoryResult.HasNext)
        //{
        //    inventoryResult = await inventoryResult.GetNextAsync(5);
        //    List<PlayersInventoryItem> nextFiveItems = inventoryResult.PlayersInventoryItems;
        //    // do something with your items
        //}
    }

    async Task AddInitialInventoryItems()
    {
        foreach (var item in initialInventoryItems)
        {
            await AddInventoryItem(item);
        }

        await SyncLocalDataWithCloud();
    }

    public async Task<PlayersInventoryItem> FetchInventoryItemFromCloud(string inventoryItemId)
    {
        var options = new GetInventoryOptions
        {
            InventoryItemIds = new List<string>() { inventoryItemId }
        };

        try
        {
            var inventoryResult = await EconomyService.Instance.PlayerInventory.GetInventoryAsync(options);
            List<PlayersInventoryItem> listOfItems = inventoryResult.PlayersInventoryItems;

            if (listOfItems.Count > 1)
            {
                print($"Inventory: Warning: Expected single inventory item for id {inventoryItemId} but found {listOfItems.Count}");
            }

            return listOfItems[0];
        }
        catch(Exception ex)
        {
            Debug.LogError($"Inventory: Error retrieving Inventory item {inventoryItemId}: {ex}");
            return null;
        }
    }

    public async Task<PlayersInventoryItem> AddInventoryItem(string inventoryItemId, Dictionary<string, object> optionalInstanceData = null)
    {
        //optionalInstanceData = new Dictionary<string, object>
        //{
        //    { "rarity", "purple" }
        //};
        //

        try
        {
            //if (optionalInstanceData.Count > 0)
            //{
            //    var options = new AddInventoryItemOptions { InstanceData = optionalInstanceData };
            //    print($"Inventory: Successfully added Item {inventoryItemId} to Inventory");

            //    return await EconomyService.Instance.PlayerInventory.AddInventoryItemAsync(inventoryItemId, options);
            //}

            var options = new AddInventoryItemOptions
            {
                PlayersInventoryItemId = inventoryItemId,
            };

            var item = await EconomyService.Instance.PlayerInventory.AddInventoryItemAsync(inventoryItemId, options);
            print($"Inventory: Successfully added item {inventoryItemId} to Inventory.");
            return item;
        }
        catch(Exception ex)
        {
            Debug.LogError($"Inventory: Error adding Inventory item {inventoryItemId}: {ex}");
            return null;
        }

    }

    public async Task DeleteInventoryItem(string playersInventoryItemId)
    {
        try
        {
            var item = GetLocalItem(playersInventoryItemId);
            localInventory.Remove(item);
            await EconomyService.Instance.PlayerInventory.DeletePlayersInventoryItemAsync(playersInventoryItemId);
            
            print($"Inventory: Deleted item {playersInventoryItemId} from Cloud.");

        }
        catch(Exception ex)
        {
            Debug.LogError($"Inventory: Error deleting Inventory item {playersInventoryItemId}: {ex}");
        }
    }

    public bool HasLocalItem(string inventoryItemId)
    {
        if (localInventory.Count == 0)
        {
            Debug.LogError("Inventory: HasLocalItem() Local Inventory count 0.  Returning false.");
            return false;
        }
        foreach (var item in localInventory)
        {
            if (item.InventoryItemId == inventoryItemId) return true;
        }

        return false;
    }

    public PlayersInventoryItem GetLocalItem(string inventoryItemId)
    {
        foreach (var item in localInventory)
        {
            if (item.InventoryItemId == inventoryItemId) return item;

        }

        print($"Inventory:  Local item {inventoryItemId} not found.");
        return null;
    }
}
