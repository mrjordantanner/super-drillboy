using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services;
using Unity.Services.Economy.Model;
using Unity.Services.Economy;


public enum CurrencyType { Gems, PlayCurrency, Money }
public enum RewardType { Gems, PlayCurrency, Upgrade }


public class Currency : MonoBehaviour
{
    #region Singleton
    public static Currency Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public LocalCurrencyData LocalData;

    // Currency Definitions
    public CurrencyDefinition GemsDefinition;
    public CurrencyDefinition 
        PlayCurrencyDefinition, 
        Material_1_Definition, 
        Material_2_Definition;

    // Currency Id's
    [HideInInspector] public readonly string GemsId = "GEMS";
    [HideInInspector] public readonly string PlayCurrencyId = "PLAY_CURRENCY";
    [HideInInspector] public readonly string MaterialId_1 = "MATERIAL_1";
    [HideInInspector] public readonly string MaterialId_2 = "MATERIAL_2";

    GetBalancesOptions getBalancesOptions = new() { ItemsPerFetch = 4 };

    public async Task Init()
    {
        await EconomyService.Instance.Configuration.SyncConfigurationAsync();
        GetCurrencyDefinitions();

        Inventory.Instance.Init();
        SyncLocalDataWithCloud();
    }

    void GetCurrencyDefinitions()
    {
        GemsDefinition = EconomyService.Instance.Configuration.GetCurrency(GemsId);
        PlayCurrencyDefinition = EconomyService.Instance.Configuration.GetCurrency(PlayCurrencyId);
        Material_1_Definition = EconomyService.Instance.Configuration.GetCurrency(MaterialId_1);
        Material_2_Definition = EconomyService.Instance.Configuration.GetCurrency(MaterialId_2);
    }

    public async Task SyncLocalDataWithCloud()
    {
        try
        {
            GetBalancesResult getBalancesResult = await EconomyService.Instance.PlayerBalances.GetBalancesAsync(getBalancesOptions); 
            List<PlayerBalance> balances = getBalancesResult.Balances;

            foreach (var balance in balances)
            {
                if (balance.CurrencyId == GemsId) LocalData.Gems = balance.Balance;
                if (balance.CurrencyId == PlayCurrencyId)
                {
                    LocalData.PlayCurrency = (int)balance.Balance;
                    LocalData.MaxPlayCurrency = PlayCurrencyDefinition.Max;
                }
                if (balance.CurrencyId == MaterialId_1)
                {
                    LocalData.Material_1 = (int)balance.Balance;
                    LocalData.MaxMaterial_1 = Material_1_Definition.Max;
                }
                if (balance.CurrencyId == MaterialId_2) LocalData.Material_2 = (int)balance.Balance;
            }

            Menu.Instance.RefreshAllMenuPanels();
            if (GameManager.Instance.cloudLogging) print("Currency: Local Data has been synced to Cloud.");
        }
        catch(Exception ex)
        {
            Debug.LogError($"Currency: Error syncing local Balance data: {ex}");
        }
    }

    public async Task IncrementCurrency(string currencyId, int amount)
    {
        await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(currencyId, amount);
        await SyncLocalDataWithCloud();

        Menu.Instance.RefreshAllMenuPanels();
    }

    public async Task DecrementCurrency(string currencyId, int amount)
    {
        await EconomyService.Instance.PlayerBalances.DecrementBalanceAsync(currencyId, amount);
        await SyncLocalDataWithCloud();

        Menu.Instance.RefreshAllMenuPanels();
    }

    public async Task SetCurrency(string currencyId, int amount)
    {
        await EconomyService.Instance.PlayerBalances.SetBalanceAsync(currencyId, amount);
        await SyncLocalDataWithCloud();

        Menu.Instance.RefreshAllMenuPanels();
    }

    public async Task SetMaxCurrency(string currencyId, int amount)
    {
        // This will involve modifying the CurrencyDefinition
    }

    public async Task IncreaseMaxCurrency(string currencyId, int amount)
    {
        // This will involve modifying the CurrencyDefinition
    }

    public async Task ResetAllCurrenciesToDefault()
    {
        SetCurrency(GemsId, GemsDefinition.Initial);
        SetCurrency(PlayCurrencyId, PlayCurrencyDefinition.Initial);
        SetCurrency(MaterialId_1, Material_1_Definition.Initial);
        SetCurrency(MaterialId_2, Material_2_Definition.Initial);

        SetMaxCurrency(PlayCurrencyId, PlayCurrencyDefinition.Max);
        SetMaxCurrency(MaterialId_1, Material_1_Definition.Max);

        await Currency.Instance.SyncLocalDataWithCloud();
    }


}
