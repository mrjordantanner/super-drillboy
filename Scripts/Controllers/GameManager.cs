using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;
using Cinemachine;

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

        Init();
    }

    public bool runInDevMode;
    [ReadOnly]
    public float clock;
    [ReadOnly]
    public bool gameRunning;
    [ReadOnly]
    public bool gamePaused;
    [ReadOnly]
    public bool gameOver;
    [ReadOnly]
    public bool inputSuspended;

    [Header("Player")]
    public GameObject PlayerPrefab;
    [HideInInspector]
    public Player player;
    public Transform playerSpawnPoint;
    [ReadOnly]
    public Checkpoint lastCheckpointReached;

    [Header("Misc")]
    public GameObject PPVolume;

    [Header("Dev Options")]
    public bool masterInvulnerability;
    public bool allowDevInput;
    [HideInInspector]
    public bool devModeWasUsed = false;
    bool devModeOn = false;
    bool devInputBufferOn;
    public GameObject devModeControlsText;

    [Header("Test strings")]
    public string testPlayerName;
    public float testScore;

    [HideInInspector]
    public CameraFollow cameraFollow;
    [HideInInspector]
    public int playerScore;

    [Header("Colors")]
    public Color checkpointInactive, checkpointActive;

    [Header("VFX")]
    public GameObject BlockDestroyVFX;
    public GameObject PlayerDeathVFX;

    CinemachineVirtualCamera playerCam;

    public void Init()
    {
        Time.timeScale = 0;
        gameRunning = false;
        gameOver = false;
        inputSuspended = true;

        HUD.Instance.canvasGroup.alpha = 0;


        playerCam = FindObjectOfType<CinemachineVirtualCamera>();
        if (playerCam == null)
        {
            Debug.LogError("Couldn't find playerCam");
        }

        if (runInDevMode)
        {
            StartCoroutine(StartGame());
        }
        else
        {
            Menu.Instance.Init();
            AudioManager.Instance.musicAudioSource.clip = AudioManager.Instance.titleScreenMusic;
            if (AudioManager.Instance.musicEnabled)
            {
                AudioManager.Instance.musicAudioSource.Play();
            }
        }


    }

    private void Update()
    {
        if (gameRunning && !gamePaused)
        {
            UpdateClock();
        }

        if (allowDevInput && !devInputBufferOn)
        {
            HandleDevInput();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePostProcessing();
        }

        // Dev mode
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            devModeWasUsed = true;
            devModeOn = !devModeOn;
            allowDevInput = !allowDevInput;
            devModeControlsText.SetActive(devModeOn);

            StartCoroutine(HUD.Instance.Message($"Developer Mode Enabled: {devModeOn} - Not eligible for leaderboard", 3f, true));

        }

        // Freeze-frame
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (gamePaused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }

            StartDevInputBuffer();
        }
    }

    void TogglePostProcessing()
    {
        if (!PPVolume.activeInHierarchy)
        {
            PPVolume.SetActive(true);
        }
        else
        {
            PPVolume.SetActive(false);
        }
    }

    public void SpawnPlayer(Vector2 spawnPoint, bool initialSpawn = false)
    {
        var existingPlayer = FindObjectOfType<Player>();
        if (existingPlayer != null)
        {
            Destroy(existingPlayer.transform.gameObject);
        }

        var PlayerObject = Instantiate(PlayerPrefab, spawnPoint, Quaternion.identity);
        PlayerObject.name = "Player";
        player = PlayerObject.GetComponent<Player>();
        PlayerManager.Instance.UpdatePlayerRef(player);
        playerCam.Follow = PlayerObject.transform;
        StartCoroutine(PlayerManager.Instance.DamageCooldown());
    }

    public IEnumerator StartGame()
    {
        if (!runInDevMode)
        {
            StartCoroutine(AudioManager.Instance.MusicTransition(AudioManager.Instance.mainMusic));
            HUD.Instance.screenFader.FadeOut(1f);
            yield return new WaitForSecondsRealtime(1f);

            Menu.Instance.FullscreenMenuBackground.SetActive(false);
            HUD.Instance.canvasGroup.alpha = 1;
        }

        SpawnPlayer(playerSpawnPoint.transform.position, true);

        if (!runInDevMode)
        {
            HUD.Instance.screenFader.FadeIn(1f);
            yield return new WaitForSecondsRealtime(1f);
        }

        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;

        gameRunning = true;
        inputSuspended = false;

        if (!runInDevMode)
        {
            StartCoroutine(DisplayInstructionMessages());
        }
    }

    IEnumerator DisplayInstructionMessages()
    {
        yield return new WaitForSeconds(2f);

        StartCoroutine(HUD.Instance.Message($"FALL AS FAR AS POSSIBLE", 5, false));
        yield return new WaitForSeconds(7f);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    void StartDevInputBuffer()
    {
        StartCoroutine(DevInputBuffer());
    }

    IEnumerator DevInputBuffer()
    {
        if (devInputBufferOn) yield return null;
        devInputBufferOn = true;
        yield return new WaitForSecondsRealtime(0.2f);
        devInputBufferOn = false;
    }

    void HandleDevInput()
    {
        // End Game
        if (Input.GetKey(KeyCode.Keypad0))
        {
            StartCoroutine(GameOver());
        }

        // Restart Game
        if (Input.GetKeyDown(KeyCode.KeypadPeriod))
        {
            RestartGame();
            StartDevInputBuffer();
        }

        // Clear Console
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Utils.ClearLog();
        }

        // Save User Data to Dreamlo
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            // Dreamlo.Instance.SaveUserAndUpload(testPlayerName, testScore);
        }

        // Dowload Current User's Data using TestPlayerName
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Dreamlo.Instance.DownloadUser(testPlayerName);
        }

        // Refresh Leaderboard UI
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            LeaderboardController.Instance.CreateRows(true);
        }

        // Trigger SuperDash
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            if (!PlayerManager.Instance.isSuperDashing)
            {
                StartCoroutine(PlayerManager.Instance.StartSuperDash());
            }
        }
    }

    public void Pause()
    {
        //AudioManager.Instance.ReduceMusicVolume();

        gamePaused = true;
        Time.timeScale = 0;
        Physics2D.simulationMode = SimulationMode2D.Script;
        inputSuspended = true;
    }

    public void Unpause()
    {
        //AudioManager.Instance.RestoreMusicVolume();

        gamePaused = false;
        Time.timeScale = 1;
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        inputSuspended = false;
    }

    public IEnumerator GameOver()
    {
        gameRunning = false;
        gameOver = true;

        yield return new WaitForSeconds(1f);

        AudioManager.Instance.Play("Game-Over");

        yield return new WaitForSeconds(2f);

        HUD.Instance.canvasGroup.alpha = 0;
        //CalculateFinalScore(clock);
        Menu.Instance.ShowPostGamePanel();

    }

    void UpdateClock()
    {
        clock += Time.deltaTime;
        HUD.Instance.gameClock.text = Utils.TimeFormat(clock).ToString();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}



