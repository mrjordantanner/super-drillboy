using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TestMode { Normal, SkipIntro, SkipAll }
public enum GameMode { Adventure, Survival }


public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;
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

        StartCoroutine(Init());
    }

    #region Declarations
    public TestMode testMode;

    [HideInInspector]
    public int playerScore;

    [Header("Logging")]
    public bool cloudLogging;

    // Adventure Mode:  Play through and unlock individual levels - 5 HP, 1 life per play
    // Survivor Mode:  Play through all unlocked levels in random order - 5 HP, 3 lives per playthrough
    [Header("Game Mode")]
    public GameMode gameMode;
    //public int adventureStartingLives = 1, survivorStartingLives = 1;

    [Header("Mobile Settings")]
    public bool mobileMode;
    public bool mobileVibrationEnabled = true;
    public float vibrationCooldown = 0.03f;
    bool isVibrationOnCooldown;
    public enum VibrationStyle { Light, Medium, Heavy }
    [HideInInspector]
    public VibrationStyle vibrationStyle;

    [Header("Game State")]
    [ReadOnly]
    public bool gameRunning;
    [ReadOnly]
    public bool gamePaused;
    [ReadOnly]
    public bool gameOver;
    [ReadOnly]
    public bool inputSuspended;

    [Header("Misc")]
    [ReadOnly]
    public float clock;

    [Header("VFX")]
    public Material collectGemMaterial;
    public Material hitFlashMaterial;

    //[Header("Frame Counter")]
    //[ReadOnly]
    //public bool frameCounterEnabled;
    //[ReadOnly]
    //public int framesElapsed;
    //[ReadOnly]
    //public int framesToCount;

    [HideInInspector]
    public CinemachineVirtualCamera playerCam;
    [HideInInspector]
    public CinemachineFramingTransposer transposer;

    //public delegate void FrameCountHandler(int frameCount);
    //public event FrameCountHandler OnFrameCount;
    #endregion

    List<object> Dependencies = new();
    public IEnumerator Init()
    {
    //    TODO consider having all dependency Singletons implement interface IInitializable that requires an Init() method
    //     public List<IInitializable> Dependencies = new();
    //      Then, could iterate through all IInitializables and init them after loading
    //     Could do this in the Loader.cs

        Dependencies = new()   
        {
            Menu.Instance,
            HUD.Instance,
            MapGenerator.Instance,
            LevelController.Instance,
            AudioManager.Instance,
            Config.Instance,
            SkillController.Instance,
            Currency.Instance,
            PlayScreen.Instance,
            Inventory.Instance,
            NavBar.Instance
        };

        foreach (var dependency in Dependencies)
        {
            yield return StartCoroutine(Utils.WaitFor(dependency != null, 2));
        }
        print("GM: Dependencies loaded.");

        // Initialize dependencies
        Menu.Instance.Init();
        LevelController.Instance.Init();
        PlayScreen.Instance.Init();
        SkillController.Instance.Init();

        HUD.Instance.Hide();
        Taptic.tapticOn = true;
        
        playerCam = FindObjectOfType<CinemachineVirtualCamera>();
        transposer = playerCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        playerCam.m_Lens.OrthographicSize = mobileMode ? 14 : 11;

        StartCoroutine(AudioManager.Instance.InitializeMusic());
        Menu.Instance.settingsDebugTable.PopulateTable();
        Menu.Instance.FadeInCanvasGroup(HUD.Instance.mobileUI);

        gameRunning = false;
        gameOver = false;
        inputSuspended = true;

        switch (testMode)
        {
            case TestMode.Normal:
                break;

            case TestMode.SkipIntro:
                StartCoroutine(Menu.Instance.MainMenuTransitionFromTitle());
                break;

            case TestMode.SkipAll:
                Menu.Instance.titleScreen.introSequence.backgroundSprite.gameObject.SetActive(false);
                Menu.Instance.titleScreen.introSequence.gameObject.SetActive(false);
                StartCoroutine(StartNewRun());
                break;
        }
    }

    IEnumerator StartVibrationCooldown()
    {
        isVibrationOnCooldown = true;
        yield return new WaitForSecondsRealtime(vibrationCooldown);
        isVibrationOnCooldown = false;
    }

    public void VibrateMobileDevice(VibrationStyle vibrationStyle)
    {
        if (!mobileVibrationEnabled || !mobileMode || isVibrationOnCooldown || gamePaused) return;

        //var vibrationStyle = (VibrationStyle)Menu.Instance.optionsMenu.vibrationDropdown.value;
        StartCoroutine(StartVibrationCooldown());

        switch (vibrationStyle)
        {
            case VibrationStyle.Light:
               Taptic.Light();
                break;

            case VibrationStyle.Medium:
               Taptic.Medium();
                break;

            case VibrationStyle.Heavy:
                Taptic.Heavy();
                break;

            default:
                Taptic.Medium();
                break;

        }
    }

    //private void Update()
    //{
    //    //HandleFrameCounter();

    //    if (gameRunning)
    //    {
    //        playerScore = CalculateScore();
    //    }
    //}

    // Button callback

    public void InitializeNewRun()
    {
        StartCoroutine(InitializeNewRunRoutine());
    }

    IEnumerator InitializeNewRunRoutine()
    {
        GemController.Instance.ResetGemCounters();
        Currency.Instance.DecrementCurrency(Currency.Instance.PlayCurrencyId, (int)LevelController.Instance.PlayCost);

        HUD.Instance.screenFader.FadeOut(1f);
        StartCoroutine(AudioManager.Instance.FadeMusicOut());
        yield return new WaitForSecondsRealtime(1f);

        PauseMenu.Instance.detailsPanel.RefreshDetails();
        
        Menu.Instance.MenuBackgroundsSetActive(false);
        PlayScreen.Instance.Hide(0);
        StatsBar.Instance.Hide(0);
        NavBar.Instance.Hide(0);

        StartCoroutine(StartNewRun());

        DevTools.Instance.menuDevToolsWindow.Hide();
    }

    public void EndRun()
    {
        StartCoroutine(EndRunRoutine());
    }

    IEnumerator EndRunRoutine()
    {
        GemController.Instance.SyncAndResetGemCache();
        PlayerData.Instance.SaveAllAsync();

        // TODO play transitional menu animation here, and also when starting a run - special fade out, etc

        DevTools.Instance.gameplayDevToolsWindow.Hide();
        Menu.Instance.ActiveMenuPanel.Hide();
        HUD.Instance.screenFader.FadeOut(1f);
        StartCoroutine(AudioManager.Instance.FadeMusicOut(1));
        yield return new WaitForSecondsRealtime(1);

        gameRunning = false;
        inputSuspended = true;
        Time.timeScale = 0;
        MapGenerator.Instance.ClearMap();
        if (PlayerManager.Instance.player) Destroy(PlayerManager.Instance.player.gameObject);

        MapGenerator.Instance.ToggleScrollingBackgrounds(false);
        Menu.Instance.MenuBackgroundsSetActive(true);

        PlayScreen.Instance.Show();
        StatsBar.Instance.Show();
        NavBar.Instance.Show();
        HUD.Instance.screenFader.FadeIn(0.5f);
        AudioManager.Instance.FadeMusicIn(AudioManager.Instance.titleScreenMusic);

    }

    public IEnumerator StartNewRun()
    {
        //LevelController.Instance.SetDifficulty(1);  // TODO pass in difficulty from Challenge Tier slider

        print($"GM: NEW RUN -- Difficulty: {LevelController.Instance.difficultyLevel} / Checkpoints: {LevelController.Instance.CheckpointsPerLevel} / Mode: {gameMode}");

        MapGenerator.Instance.InitializeMap();
        MapGenerator.Instance.ApplyLevelThemeBackgrounds();
        MapGenerator.Instance.StartMapGenerator();

        StartCoroutine(Utils.WaitFor(MapGenerator.Instance.isMapReady, 3f));

        MapGenerator.Instance.ToggleScrollingBackgrounds(true);
        MapGenerator.Instance.startingSection.gameObject.SetActive(true);
        GemController.Instance.Init();
        HUD.Instance.Show();

        if (mobileMode) ControlManager.Instance.ActivateMobileControls();

        PlayerManager.Instance.SpawnPlayer(PlayerManager.Instance.playerSpawnPoint.transform.position);

        gameRunning = true;
        inputSuspended = false;

        HUD.Instance.screenFader.FadeIn(1f);
        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }

    public void RestartGame()
    {
        StartCoroutine(Restart());
    }

    IEnumerator Restart()
    {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        AudioManager.Instance.ReduceMusicVolume();

        gamePaused = true;
        Time.timeScale = 0;
        Physics2D.simulationMode = SimulationMode2D.Script;
        inputSuspended = true;
        HUD.Instance.screenFlash.SetActive(false);
        StatsBar.Instance.Show();
    }

    public void Unpause()
    {
        AudioManager.Instance.RestoreMusicVolume();

        gamePaused = false;
        Time.timeScale = 1;
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        inputSuspended = false;
        HUD.Instance.screenFlash.SetActive(true);
        StatsBar.Instance.Hide();
    }

    public IEnumerator GameOver()
    {
        gameRunning = false;
        gameOver = true;

        yield return new WaitForSeconds(1f);

        //AudioManager.Instance.Play("Game-Over");

        yield return new WaitForSeconds(2f);

        HUD.Instance.Hide();
        Menu.Instance.FadeOutCanvasGroup(HUD.Instance.mobileUI);
        Menu.Instance.scorePanel.ShowScorePanel();
    }

    public void Quit()
    {
        GemController.Instance.SyncAndResetGemCache();
        PlayerData.Instance.SaveAllAsync();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    //public int CalculateScore()
    //{
    //    return (int)(PlayerManager.Instance.currentDepth + GemController.Instance.gemsCollectedThisRun);
    //}

    //void ShowNewBestScoreMessage()
    //{
    //    HUD.Instance.ShowMessage($"New High Score!",
    //        uiMessageFadeInDuration, uiMessageDisplayDuration, uiMessageFadeOutDuration, true);
    //}

    //public void EnableFrameCounter(int targetFrames)
    //{
    //    framesToCount = targetFrames;
    //    framesElapsed = 0;
    //    frameCounterEnabled = true;
    //}

    //void HandleFrameCounter()
    //{
    //    if (frameCounterEnabled && framesElapsed < framesToCount)
    //    {
    //        framesElapsed++;
    //        if (framesElapsed == framesToCount && OnFrameCount != null)
    //        {
    //            OnFrameCount(framesElapsed);
    //            frameCounterEnabled = false;
    //            framesElapsed = 0;
    //        }
    //    }
    //}


}
