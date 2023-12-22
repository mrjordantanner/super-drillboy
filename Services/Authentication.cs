using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;


public class Authentication : MonoBehaviour
{
    #region Singleton
    public static Authentication Instance;
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
        #endregion

        Init();
    }

    [HideInInspector]
    public string playerId;
    [HideInInspector]
    public string accessToken;

    async Task Init()
    {
        await Startup();

        Currency.Instance.Init();
        PlayerData.Instance.Init();
    }

    public async Task Startup()
    {
        await UnityServices.InitializeAsync();
        await SignInAnonymously();
    }

    private async Task SignInAnonymously()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            playerId = AuthenticationService.Instance.PlayerId;
            accessToken = AuthenticationService.Instance.AccessToken;

            Debug.Log($"Signed in as PlayerId: {playerId}");

            AuthenticationService.Instance.SignInFailed += (err) => {
                Debug.LogError(err);
            };

            AuthenticationService.Instance.SignedOut += () => {
                Debug.Log("Player signed out.");
            };

            AuthenticationService.Instance.Expired += () =>
            {
                Debug.Log("Player session could not be refreshed and expired.");
            };
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

}
