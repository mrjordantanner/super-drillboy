using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;


/// <summary>
/// Singleton.  Saves and loads PlayerData using Unity Cloud Save.
/// </summary>
public class PlayerData : MonoBehaviour
{
    #region Singleton
    public static PlayerData Instance;
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

    public LocalPlayerData Data;
    public Sprite[] playerAvatars;

    private List<ItemKey> itemKeys;

    // Keys
    string XP = nameof(XP);
    string TotalXP = nameof(TotalXP);
    string PlayerLevel = nameof(PlayerLevel);
    string PlayerName = nameof(PlayerName);
    string PlayerAvatar = nameof(PlayerAvatar);

    public async Task Init()
    {
        playerAvatars = Resources.LoadAll<Sprite>("Avatars");
        itemKeys = await CloudSaveService.Instance.Data.Player.ListAllKeysAsync();

        if (itemKeys.Count > 0)
        {
            var loadedData = await LoadAllAsync();
            SetLocalPlayerData(loadedData);
        }
        else
        {
            print($"No existing keys for PlayerData found in Cloud.  Creating new PlayerData keys.");
            await SaveAllAsync();
        }
    }

    public Sprite GetPlayerAvatar()
    {
        return playerAvatars[Data.PlayerAvatarIndex];
    }

    void SetLocalPlayerData(Dictionary<string, Item> data)
    {
        Data.PlayerName = GetStringFromData(PlayerName, data);
        Data.PlayerLevel = GetIntFromData(PlayerLevel, data);
        Data.XP = GetIntFromData(XP, data);
        Data.TotalXP = GetIntFromData(TotalXP, data);
        Data.PlayerAvatarIndex = GetIntFromData(PlayerAvatar, data);

        StatsBar.Instance.Refresh();

        if (GameManager.Instance.cloudLogging) print("PlayerData: Local PlayerData has been set");
    }

    // Load all items from Cloud for the logged-in Player
    public async Task<Dictionary<string, Item>> LoadAllAsync()
    {
        try
        {
            return await CloudSaveService.Instance.Data.Player.LoadAllAsync();
        }
        catch (Exception ex)
        {
            Debug.Log($"PlayerData: Error loading all Player Data from Cloud: {ex}");
            return null;
        }

    }

    // Delete an item in Cloud by Key
    public async Task DeleteAsync(string key)
    {
        await CloudSaveService.Instance.Data.Player.DeleteAsync(key);
    }

    public async Task DeleteAllAsync()
    {
        itemKeys = await CloudSaveService.Instance.Data.Player.ListAllKeysAsync();
        foreach (var key in itemKeys)
        { 
            await DeleteAsync(key.Key);
        }
    }

    public async Task SaveAllAsync()
    {
        try
        {
            var saveData = new Dictionary<string, object>
            {
                { PlayerName, Data.PlayerName },
                { XP, Data.XP },
                { TotalXP, Data.TotalXP },
                { PlayerLevel, Data.PlayerLevel },
                { PlayerAvatar, Data.PlayerAvatarIndex }
            };
            await CloudSaveService.Instance.Data.Player.SaveAsync(saveData);

            if (GameManager.Instance.cloudLogging) print("PlayerData: All Player Data saved to Cloud.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"An exception occurred while saving Player Data: {ex}");
        }
    }

    #region Shortcut Methods

    public async Task SaveLevelAndXPAsync()
    {
        try
        {
            var saveData = new Dictionary<string, object> 
            { 
                { XP, Data.XP }, 
                { TotalXP, Data.TotalXP },
                { PlayerLevel, Data.PlayerLevel }
            };
            await CloudSaveService.Instance.Data.Player.SaveAsync(saveData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"An exception occurred while saving PlayerLevel and XP: {ex}");
        }
    }

    public async Task SavePlayerName()
    {
        try
        {
            var saveData = new Dictionary<string, object> { { PlayerName, Data.PlayerName } };
            await CloudSaveService.Instance.Data.Player.SaveAsync(saveData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"An exception occurred while saving PlayerName: {ex}");
        }
    }


    #endregion

    #region Helper Methods
    float GetFloatFromData(string key, Dictionary<string, Item> data)
    {
        return data[key].Value.GetAs<float>();
    }

    string GetStringFromData(string key, Dictionary<string, Item> data)
    {
        return data[key].Value.GetAsString();
    }

    int GetIntFromData(string key, Dictionary<string, Item> data)
    {
        return data[key].Value.GetAs<int>();
    }
    #endregion
}
