using UnityEngine;
using TMPro;
using System.Collections;


public class DevTools : MonoBehaviour
{
    #region Singleton
    public static DevTools Instance;
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

    #region Declarations
    [ReadOnly] public bool gameToolsActive, menuToolsActive;
    public float devToolsOpacity = 0.9f;
    public float statsWindowOpacity = 0.75f;

    public bool preventAutoSkillTrigger;

    [Header("Dev Message")]
    public float uiMessageFadeInDuration = 0.25f;
    public float uiMessageDisplayDuration = 2f,
        uiMessageFadeOutDuration = 0.5f;

    [Header("Windows & Labels")]
    [ReadOnly]
    public bool statsWindowActive;
    public MenuPanel statsWindow;
    public MenuPanel menuDevToolsWindow, gameplayDevToolsWindow;
    public TextMeshProUGUI
        label_PlayerVelocity,
        //label_GemsCollected,
        label_CheckpointsReached,
        label_BlocksDestroyed,
        label_SmallGemDropChance,
        label_SmallGemsDropped,
        label_LargeGemDropChance,
        label_LargeGemsDropped,
        label_HealthPickupDropChance,
        label_HealthPickupsDropped,
        label_ExtraLifeDropChance,
        label_ExtraLivesDropped,
        label_RandomDestBlocksSpawned,
        label_RandomDamageBlocksSpawned,
        label_CurrentRandomBlockSpawnChance,
        label_RandomBlockSpawnChanceIncrease,
        label_DifficultyLevel,
        label_MapSectionsCreated,
        label_MapSegmentsCreated,
        label_CreateCheckpointAfterSegments,
        label_CheckpointsPerLevel,
        label_Platform,
        label_DeviceType,
        label_DeviceModel;

    [Header("BoolRows")]
    public BoolRow boolrow_Grounded;
    public BoolRow
        boolrow_Invulnerable,
        boolrow_Drilling,
        boolrow_NitroDrilling,
        boolrow_Dead;

    bool devInputBufferOn;
    [HideInInspector]
    public bool devToolsWereUsed;
    #endregion

    private void Start()
    {
        //SetStatWindowStaticStats();
        //statsWindow.Hide();
        menuDevToolsWindow.Hide();
        gameplayDevToolsWindow.Hide();

        //if (GameManager.Instance.mobileMode)
        //{
        //    label_Platform.text = Application.platform.ToString();
        //    label_DeviceType.text = SystemInfo.deviceType.ToString();
        //    label_DeviceModel.text = SystemInfo.deviceModel.ToString();
        //}
    
    }

 

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    ToggleStatsWindow();
        //}

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            ToggleDevWindow();
        }

        if (!devInputBufferOn && (gameToolsActive || menuToolsActive))
        {
            if (!GameManager.Instance.gameRunning && menuToolsActive)
            {
                HandleMenuDevInput();
            }
            else if (GameManager.Instance.gameRunning && gameToolsActive)
            {
                HandleGameplayDevInput();
            }
            
        }

        //if (statsWindowActive)
        //{
        //    UpdateStatsWindow();
        //    UpdateBoolRows();
        //}
    }

    void HandleGameplayDevInput()
    {
        if (Menu.Instance.enteringText) return;

        // 1: Gain Health
        if (Input.GetKeyDown(KeyCode.Keypad1))// || Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerManager.Instance.CollectArmor();
            StartDevInputBuffer();
        }

        // 2:
        if (Input.GetKeyDown(KeyCode.Keypad2))// || Input.GetKeyDown(KeyCode.Alpha2))
        {
            
            StartDevInputBuffer();
        }

        // 3: Toggle No Special
        if (Input.GetKeyDown(KeyCode.Keypad3))// || Input.GetKeyDown(KeyCode.Alpha3))
        {
            TogglePreventBoost();
            StartDevInputBuffer();
        }

        // 4: Toggle Invulnerability
        if (Input.GetKeyDown(KeyCode.Keypad4))// || Input.GetKeyDown(KeyCode.Alpha4))
        {
            ToggleInvulnerability();
            StartDevInputBuffer();
        }

        // 5: Use Current Skill
        if (Input.GetKeyDown(KeyCode.Keypad5))// || Input.GetKeyDown(KeyCode.Alpha5))
        {
            SkillController.Instance.CurrentSkill.Use();
            StartDevInputBuffer();
        }

        // 6: Cue Level Change
        if (Input.GetKeyDown(KeyCode.Keypad6))// || Input.GetKeyDown(KeyCode.Alpha6))
        {
            CueLevelChange();
            StartDevInputBuffer();
        }

        // 7: Die
        if (Input.GetKeyDown(KeyCode.Keypad7))// || Input.GetKeyDown(KeyCode.Alpha7))
        {
            PlayerDie();
            StartDevInputBuffer();
        }

        // 8: Restart Game
        if (Input.GetKeyDown(KeyCode.Keypad8))// || Input.GetKeyDown(KeyCode.Alpha8))
        {
            GameManager.Instance.RestartGame();
            StartDevInputBuffer();
        }

        // 9: Toggle Freeze-frame
        if (Input.GetKeyDown(KeyCode.Keypad9))// || Input.GetKeyDown(KeyCode.Alpha9))
        {
           ToggleFreezeframe();
        }

        // 0: 
        if (Input.GetKeyDown(KeyCode.Keypad0))// || Input.GetKeyDown(KeyCode.Alpha0))
        {
            
            StartDevInputBuffer();
        }

        // Period: Test Damage VFX
        if (Input.GetKeyDown(KeyCode.KeypadPeriod))
        {
            StartCoroutine(VFX.Instance.StartDamageEffects());
            StartDevInputBuffer();
        }


        // ALPHA KEYS
        // Alpha 1: Increase DifficultyLevel
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            IncreaseDifficultyLevel();
        }

        // Alpha 2: Decrease difficultyLvel
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DecreaseDifficultyLevel();
        }

        // Alpha 3: Increase GemDepthFactor
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            IncreaseGemDepthFactor();
        }

        // Alpha 4: Decrease GemDepthFactor
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            DecreaseGemDepthFactor();
        }

        // Alpha 5: Reset GemDepthMultiplier
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ResetGemDepthMultiplier();
        }

        // Alpha 6: Collect Small Gem
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            GemController.Instance.CollectGem(GemType.Small);
            StartDevInputBuffer();
        }

        // Alpha 7: Collect Large Gem
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            GemController.Instance.CollectGem(GemType.Large);
            StartDevInputBuffer();
        }
    }

    void HandleMenuDevInput()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1)) GainPlayCurrency();
        if (Input.GetKeyDown(KeyCode.Keypad2)) GainGems();
        if (Input.GetKeyDown(KeyCode.Keypad3)) GainXP();

        if (Input.GetKeyDown(KeyCode.Keypad8)) ResetPlayerData();
        if (Input.GetKeyDown(KeyCode.Keypad9)) ResetEconomyData();
    }

    #region Gameplay DevTools Methods

    public void ResetGemDepthMultiplier()
    {
        GemController.Instance.ResetGemDepthMultiplier();
        StartDevInputBuffer();

        HUD.Instance.ShowMessage($"Gem Depth Multiplier Reset: {GemController.Instance.gemDepthMultiplier}",
            uiMessageFadeInDuration, uiMessageDisplayDuration, uiMessageFadeOutDuration, true);
    }

    public void DecreaseGemDepthFactor()
    {
        GemController.Instance.gemDepthFactor--;
        StartDevInputBuffer();

        HUD.Instance.ShowMessage($"Gem Depth Factor: {GemController.Instance.gemDepthFactor}",
            uiMessageFadeInDuration, uiMessageDisplayDuration, uiMessageFadeOutDuration, true);
    }

    public void IncreaseGemDepthFactor()
    {
        GemController.Instance.gemDepthFactor++;
        StartDevInputBuffer();

        HUD.Instance.ShowMessage($"Gem Depth Factor: {GemController.Instance.gemDepthFactor}",
            uiMessageFadeInDuration, uiMessageDisplayDuration, uiMessageFadeOutDuration, true);
    }

    public void IncreaseDifficultyLevel()
    {
        LevelController.Instance.SetDifficulty(LevelController.Instance.difficultyLevel + 1);
        StartDevInputBuffer();

        HUD.Instance.ShowMessage($"Difficulty Level: {LevelController.Instance.difficultyLevel}",
            uiMessageFadeInDuration, uiMessageDisplayDuration, uiMessageFadeOutDuration, true);
    }

    public void DecreaseDifficultyLevel()
    {
        var newLevel = LevelController.Instance.difficultyLevel - 1;
        newLevel = newLevel < 1 ? 1 : newLevel;
        LevelController.Instance.SetDifficulty(newLevel);

        StartDevInputBuffer();

        HUD.Instance.ShowMessage($"Difficulty Level: {LevelController.Instance.difficultyLevel}",
            uiMessageFadeInDuration, uiMessageDisplayDuration, uiMessageFadeOutDuration, true);
    }

    public void ToggleFreezeframe()
    {
        if (GameManager.Instance.gamePaused)
        {
            GameManager.Instance.Unpause();
        }
        else
        {
            GameManager.Instance.Pause();
        }

        StartDevInputBuffer();
    }

    public void ToggleMobileVibration()
    {
        GameManager.Instance.mobileVibrationEnabled = !GameManager.Instance.mobileVibrationEnabled;
        HUD.Instance.ShowMessage($"Vibration Enabled: {GameManager.Instance.mobileVibrationEnabled}",
            uiMessageFadeInDuration, uiMessageDisplayDuration, uiMessageFadeOutDuration, true);
    }

    public void TogglePreventBoost()
    {
        PlayerManager.Instance.preventBoost = !PlayerManager.Instance.preventBoost;
        HUD.Instance.ShowMessage($"Prevent Boost: {PlayerManager.Instance.preventBoost}",
            uiMessageFadeInDuration, uiMessageDisplayDuration, uiMessageFadeOutDuration, true);
    }

    public void ToggleInvulnerability()
    {
        devToolsWereUsed = true;
        PlayerManager.Instance.masterInvulnerability = !PlayerManager.Instance.masterInvulnerability;
        HUD.Instance.ShowMessage($"Master Invulnerability: {PlayerManager.Instance.masterInvulnerability}",
            uiMessageFadeInDuration, uiMessageDisplayDuration, uiMessageFadeOutDuration, true);
    }

    public void CueLevelChange()
    {
        devToolsWereUsed = true;
        MapGenerator.Instance.IncreaseLevel();
        HUD.Instance.ShowMessage($"Difficulty: {LevelController.Instance.difficultyLevel} - Map Change Pending",
            uiMessageFadeInDuration, uiMessageDisplayDuration, uiMessageFadeOutDuration, true);
    }

    public void PlayerDie()
    {
        StartCoroutine(PlayerManager.Instance.PlayerDeath());
    }

    //public void ToggleMusic()
    //{
    //    AudioManager.Instance.musicMuted = !AudioManager.Instance.musicMuted;
    //    AudioManager.Instance.musicAudioSource.mute = AudioManager.Instance.musicMuted;

    //    if (AudioManager.Instance.musicAudioSource.mute)
    //    {
    //        HUD.Instance.ShowMessage($"Music Muted",
    //            uiMessageFadeInDuration, uiMessageDisplayDuration, uiMessageFadeOutDuration, true);
    //        AudioManager.Instance.musicAudioSource.volume = 0;
    //    }
    //    else
    //    {
    //        HUD.Instance.ShowMessage($"Music Unmuted",
    //            uiMessageFadeInDuration, uiMessageDisplayDuration, uiMessageFadeOutDuration, true);
    //        AudioManager.Instance.musicAudioSource.volume = 1;
    //    }
    //}
    #endregion

    #region Menu DevTools Methods
    public void GainGems(int amount = 1000)
    {
        Currency.Instance.IncrementCurrency(Currency.Instance.GemsId, amount);
        StartDevInputBuffer();
    }

    // these can probably go away since we can buy from the Shop now
    public void GainPlayCurrency(int amount = 100)
    {
        Currency.Instance.IncrementCurrency(Currency.Instance.PlayCurrencyId, amount);
        StartDevInputBuffer();
    }

    public void IncreaseMaxPlayCurrency(int amount = 100)
    {
        Currency.Instance.IncreaseMaxCurrency(Currency.Instance.PlayCurrencyId, amount);
        StartDevInputBuffer();
    }

    public void GainXP(int amount = 1000)
    {
        LevelingSystem.Instance.GainXP(amount);
        PlayerData.Instance.SaveLevelAndXPAsync();
        print($"DevTools - Gained {amount} XP");
        StartDevInputBuffer();
    }

    public void ResetPlayerData()
    {
        PlayerData.Instance.Data.ResetToDefaults();
        LevelingSystem.Instance.CalculateRequiredXP();

        PlayScreen.Instance.RefreshAll();    
        StatsBar.Instance.Refresh(); 
        HUD.Instance.UpdatePlayerLevel();

        print("DevTools - Player Level and XP reset!");
        StartDevInputBuffer();

        PlayerData.Instance.SaveAllAsync();
    }

    public void ResetEconomyData()
    {
        Currency.Instance.ResetAllCurrenciesToDefault();
    }
    #endregion

    void StartDevInputBuffer()
    {
        StartCoroutine(DevInputBuffer());
    }

    IEnumerator DevInputBuffer()
    {
        devToolsWereUsed = true;

        if (devInputBufferOn) yield return null;
        devInputBufferOn = true;
        yield return new WaitForSecondsRealtime(0.2f);
        devInputBufferOn = false;
    }

    // Button Callbacks for DevKeys
    public void FlagDevMode()
    {
        devToolsWereUsed = true;
    }

    //public void ToggleStatsWindow()
    //{
    //    var targetOpacity = statsWindowActive ? 0 : statsWindowOpacity;
    //    Menu.Instance.ToggleCanvasGroup(!statsWindowActive, statsWindow, targetOpacity);
    //    statsWindowActive = !statsWindowActive;
    //}

    public void ToggleDevWindow()
    {
        if (!GameManager.Instance.gameRunning)
        {
            if (menuToolsActive)
            {
                menuDevToolsWindow.Hide();
               
            }
            else
            {
                menuDevToolsWindow.Show();
            }
            menuToolsActive = !menuToolsActive;
        }
        else
        {
            if (gameToolsActive)
            {
                gameplayDevToolsWindow.Hide();
            }
            else
            {
                gameplayDevToolsWindow.Show();
             }
            gameToolsActive = !gameToolsActive;
        }
        
    }
 
    //void UpdateStatsWindow()
    //{
    //    label_PlayerVelocity.text = FormatPlayerVelocity();
    //    //label_GemsCollected.text = ((int)GemController.Instance.gemsCollectedThisRun).ToString();
    //    label_CheckpointsReached.text = PlayerManager.Instance.checkpointsReached.ToString();
    //    label_BlocksDestroyed.text = MapController.Instance.blocksDestroyed.ToString();
    //    label_SmallGemsDropped.text = MapController.Instance.smallGemsDropped.ToString();
    //    label_LargeGemsDropped.text = MapController.Instance.largeGemsDropped.ToString();
    //    label_HealthPickupsDropped.text = MapController.Instance.healthPickupsDropped.ToString();
    //    label_ExtraLivesDropped.text = MapController.Instance.extraLifePickupsDropped.ToString();
    //    label_RandomDestBlocksSpawned.text = MapController.Instance.destBlocksSpawned.ToString();
    //    label_RandomDamageBlocksSpawned.text = MapController.Instance.blockGroupsSpawned.ToString();
    //    label_CurrentRandomBlockSpawnChance.text = Utils.FormatPercent(MapController.Instance.damageSpawnChance, 1);
    //    label_DifficultyLevel.text = MapController.Instance.difficultyLevel.ToString();
    //    label_MapSectionsCreated.text = MapController.Instance.totalMapSectionsCreated.ToString();
    //    label_MapSegmentsCreated.text = MapController.Instance.totalSegmentsCreated.ToString();
    //}

    //void UpdateBoolRows()
    //{
    //    boolrow_Grounded.SetValue(PlayerManager.Instance.isGrounded);
    //    boolrow_Invulnerable.SetValue(PlayerManager.Instance.invulnerable || PlayerManager.Instance.masterInvulnerability);
    //    boolrow_Drilling.SetValue(PlayerManager.Instance.isDashing);
    //    boolrow_NitroDrilling.SetValue(PlayerManager.Instance.isBoosting);
    //    boolrow_Dead.SetValue(PlayerManager.Instance.dead);
    //}

    //void SetStatWindowStaticStats()
    //{
    //    label_SmallGemDropChance.text = Utils.FormatPercent(MapController.Instance.smallGemDropChance);
    //    label_LargeGemDropChance.text = Utils.FormatPercent(MapController.Instance.largeGemDropChance);
    //    label_HealthPickupDropChance.text = $"{MapController.Instance.healthDropChance * 100f}%";
    //    label_ExtraLifeDropChance.text = $"{MapController.Instance.extraLifeDropChance * 100f}%";
    //    label_RandomBlockSpawnChanceIncrease.text = $"{MapController.Instance.damageSpawnChanceIncrease * 100f}%";
    //    label_CreateCheckpointAfterSegments.text = MapController.Instance.createCheckPointAfterNumberofSegments.ToString();
    //    label_CheckpointsPerLevel.text = MapController.Instance.checkpointsPerLevel.ToString();
    //}

    //string FormatPlayerVelocity()
    //{
    //    var absVel = Mathf.Abs(PlayerManager.Instance.velocity.y);
    //    float roundedVel = Mathf.Round(absVel);
    //    return roundedVel.ToString();
    //}

}
