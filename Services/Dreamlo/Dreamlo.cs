using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Networking;

public class Dreamlo : MonoBehaviour
{
    #region Singleton
    public static Dreamlo Instance;

    private void Awake()
    {
        if (Application.isEditor)
            Instance = this;
        else
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

    }

    #endregion

    [Header("Enable Dreamlo")]
    public bool DreamloEnabled = true;

    const string webURL = "https://www.dreamlo.com/lb/";
    
    // Compo Leaderboard
    //private readonly string privateCode = "0Y-hzg7HA0Cmp6lo_1zKGgcYecO_xJOkyxv2re4pJepQ";
    //private readonly string publicCode = "5f797ad7eb371809c46342bc";

    // Post-Compo Leaderboard
    private readonly string privateCode = "_n5Ke4Ezdk6T3YVt72cGnwxxu5jMWNWEqcH52p377GSw";
    private readonly string publicCode = "6523390e8f40bc67c8a2b140";

    public List<UserDto> AllUserData = new();

    private void Start()
    {
        DownloadAll();

        // Cache returning player's username if it exists
        var username = Config.GetPlayerPrefsString("Username");
        if (!string.IsNullOrEmpty(username))
        {
            if (!DreamloEnabled) return;
            Menu.Instance.nameInputField.text = username;
            Config.Instance.currentUserDto.PlayerName = username;
        }

        if (!DreamloEnabled)
        {
            print("Dreamlo is disabled!");
        }
    }

    public void SaveUserAndUpload(string username, int score)
    {
        if (!DreamloEnabled) return;
        print($"Saving user: {username} / {score}");

        // If no name given, assign random ID
        var randomInt = Random.Range(100, 999);
        if (string.IsNullOrEmpty(username))
        {
            username = $"Player {randomInt}";

        }
        Menu.Instance.nameInputField.text = username;

        Config.Instance.SaveUsernameToPlayerPrefs(username);

        var dto = CreateUserDto(
                username,
                score,
                9999);

        Config.Instance.currentUserDto = dto;
        //print($"CurrentUserDto: Name: {Config.Instance.currentUserDto.PlayerName} / Score: {Config.Instance.currentUserDto.Score} / Rank: {Config.Instance.currentUserDto.Rank}");
        //print($"Created New UserDto: Name: {dto.PlayerName} / Score: {dto.Score} / Rank: {dto.Rank}");

        UploadUser(dto);
    }

    public void UploadUser(UserDto user)
    {
        if (!DreamloEnabled) return;
        StartCoroutine(UploadUserData(user));
    }

    IEnumerator UploadUserData(UserDto user)
    {
        print($"Uploading user: {user.PlayerName} / Score: {user.Score}");
        var requestString = $"{webURL}{privateCode}/add/{user.PlayerName}/{user.Score}";
        WWW www = new(requestString);
        yield return www;
        //UnityWebRequest request = new(requestString);
        //yield return request;
    }

    public void DownloadAll()
    {
        if (!DreamloEnabled) return;
        //print("Downloading all data...");
        StartCoroutine(DownloadAllData());
    }


    IEnumerator DownloadAllData()
    {
        WWW request = new (webURL + publicCode + "/pipe/");
        // UnityWebRequest request = new(webURL + publicCode + "/pipe/");
        yield return request;

        if (string.IsNullOrEmpty(request.error))
        {
            //print($"Dreamlo Download successful.  Result: {request.text}");

            FormatData(request.text.ToString()); 
            //FormatData(request.result.ToString());
        }
        else
        {
            print("Error Downloading: " + request.error);
        }
    }

    public void DownloadUser(string userName)
    {
        if (!DreamloEnabled) return;
        StartCoroutine(DownloadUserData(userName));
    }

    IEnumerator DownloadUserData(string userName)
    {
        //WWW request = new (webURL + publicCode + "/pipe-get/" + userName);
        UnityWebRequest request = new(webURL + publicCode + "/pipe-get/" + userName);
        yield return request;
 
        if (string.IsNullOrEmpty(request.error))
        {
            //print($"Dreamlo Download successful.  Result: {request.text}");
            //FormatData(request.text.ToString());
            FormatData(request.result.ToString());
        }
        else
        {
            print("Error Downloading: " + request.error);
        }
    }

    void FormatData(string textStream)
    {
        string[] entries = textStream.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        if (entries.Length > 0)
        {
            //print($"Entries in textstream after parsing: {entries.Length}");
            AllUserData.Clear();

            for (int i = 0; i < entries.Length; i++)
            {
                string[] entryInfo = entries[i].Split(new char[] { '|' });

                //foreach (var entry in entryInfo)
                //{
                //    print($"Entry: {entry}");
                //}

                string userName = entryInfo[0];
                int bestScore = int.Parse(entryInfo[1]);
                var rank = i + 1;

                var newUserDto = CreateUserDto(userName, bestScore, rank);
                AllUserData.Add(newUserDto);

                //print($"Dreamlo added User: {newUserDto.Rank} / {newUserDto.PlayerName} / {newUserDto.Score} to AllUsersList");
            }

            //print($"Dreamlo Downloaded - Entries processed: {entries.Length} / Users in cached list: {AllUserData.Count}");
        }
    }


    public UserDto CreateUserDto(string name, int score, int rank)
    {
        return new UserDto()
        {
            PlayerName = name,
            Score = score,
            Rank = rank
        };
    }

}








