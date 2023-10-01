using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class Menu : MonoBehaviour
{
    #region Singleton
    public static Menu Instance;
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

    public bool menuActive;

    public GameObject FullscreenMenuBackground;
    public GameObject settings_backToMainMenuButton, settings_backToPauseMenuButton,
        leaderboard_backToMainMenuButton, leaderboard_backToPauseMenuButton, leaderboard_restartGameButton,
        instructions_backToMainMenuButton, instructions_backToPauseMenuButton;

    [Header("Menu Panels")]
    public CanvasGroup MainMenuPanel;
    public CanvasGroup
        SettingsMenuPanel,
        LeaderboardPanel;
    public CanvasGroup nameEntryPanel;
    public CanvasGroup InstructionsPanel;
    public CanvasGroup PostGamePanel;

    [Header("In-Game panels")]
    public CanvasGroup PauseMenuPanel;

    [Header("PostGame")]
    public TextMeshProUGUI checkpointsReached;
    public TextMeshProUGUI depthReached, coinsCollected;

    public bool enteringText;
    public TMP_InputField nameInputField;

    CanvasGroup ActiveMenuPanel;

    public void Init()
    {
        nameInputField.onEndEdit.AddListener(OnInputEndEdit);
        nameInputField.characterLimit = 10;

        FullscreenMenuBackground.SetActive(true);
        OpenMenuPanel(MainMenuPanel);
        HUD.Instance.screenFader.FadeIn(1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !enteringText)
        {
            if (!menuActive)
            {
                GameManager.Instance.Pause();
                ShowPauseMenu();
                PlayClickSound();
            }
            else
            {
                if (ActiveMenuPanel == PauseMenuPanel)
                {
                    BackToGame();
                    PlayClickSound();
                }
                else
                {
                    BackToPauseMenu();
                    PlayClickSound();
                }
            }
        }
    }

    public void PlayClickSound()
    {
        AudioManager.Instance.Play("Click");
    }

    public void RestartGame()
    {
        StartCoroutine(RestartGameFromMenu());
    }

    IEnumerator RestartGameFromMenu()
    {
        AudioManager.Instance.Play("Menu-Confirm");
        HUD.Instance.screenFader.FadeOut(1f);
        AudioManager.Instance.musicAudioSource.DOFade(0, 1f).SetUpdate(UpdateType.Normal, true);
        yield return new WaitForSecondsRealtime(1f);
        GameManager.Instance.RestartGame();
    }


    public void ShowPostGamePanel()
    {
        checkpointsReached.text = PlayerManager.Instance.checkpointsReached.ToString();
        depthReached.text = $"{(int)PlayerManager.Instance.maxDepthReached}m";
        coinsCollected.text = PlayerManager.Instance.coins.ToString();

        OpenMenuPanel(PostGamePanel);
    }

    public void ShowNameEntryPanel()
    {
        enteringText = true;
        if (nameInputField != null)
        {
            nameInputField.Select();
            nameInputField.ActivateInputField();
        }
        StartCoroutine(SwapPanels(nameEntryPanel));
    }

    // Press enter in inputfield callback
    public void OnInputEndEdit(string username)
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            ConfirmNameEntry();
        }
    }

    public void ConfirmNameEntry()
    {
        enteringText = false;
        var username = nameInputField.text;

        var score = (int)PlayerManager.Instance.maxDepthReached;
        print($"Confirmed input: {username} / {score}");

        Dreamlo.Instance.SaveUserAndUpload(username, score);
        CloseMenuPanel(nameEntryPanel);
        OpenLeaderboardPanel(true);
        LeaderboardController.Instance.FocusOnRow();
    }

    public void ShowInstructionsPanel()
    {
        var value = GameManager.Instance.gameRunning;

        instructions_backToMainMenuButton.SetActive(!value);
        instructions_backToPauseMenuButton.SetActive(value);

        StartCoroutine(SwapPanels(InstructionsPanel));
    }

    public void ShowPauseMenu()
    {
        menuActive = true;
        HUD.Instance.canvasGroup.alpha = 0;
        GameManager.Instance.Pause();
        OpenMenuPanel(PauseMenuPanel);
    }

    public void BackToGame()
    {
        menuActive = false;
        HUD.Instance.canvasGroup.alpha = 1;
        CloseMenuPanel(ActiveMenuPanel);
        GameManager.Instance.Unpause();
    }

    public void BackToPauseMenu()
    {
        StartCoroutine(SwapPanels(PauseMenuPanel));
    }

    public void OpenMenuPanel(CanvasGroup panel)
    {
        StartCoroutine(OpenPanel(panel, 0.1f));
    }

    IEnumerator OpenPanel(CanvasGroup panel, float duration)
    {
        menuActive = true;
        ActiveMenuPanel = panel;
        Tween fadeIn = panel.DOFade(1f, duration).SetUpdate(UpdateType.Normal, true);
        yield return new WaitForSecondsRealtime(duration);

        fadeIn.Kill();
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }

    public void CloseMenuPanel(CanvasGroup panel)
    {
        StartCoroutine(ClosePanel(panel, 0.1f));
    }

    IEnumerator ClosePanel(CanvasGroup panel, float duration)
    {
        Tween fadeOut = panel.DOFade(0f, duration).SetUpdate(UpdateType.Normal, true);
        yield return new WaitForSecondsRealtime(duration);

        fadeOut.Kill();
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }

    IEnumerator SwapPanels(CanvasGroup newPanel)
    {
        CloseMenuPanel(ActiveMenuPanel);
        yield return new WaitForSecondsRealtime(0.2f);
        OpenMenuPanel(newPanel);
    }


    #region Button Callbacks
    public void ReturnToMainMenu()
    {
        StartCoroutine(SwapPanels(MainMenuPanel));
    }

    public void OpenSettingsPanel()
    {
        StartCoroutine(SwapPanels(SettingsMenuPanel));

        var value = GameManager.Instance.gameRunning;

        settings_backToMainMenuButton.SetActive(!value);
        settings_backToPauseMenuButton.SetActive(value);

    }

    public void OpenLeaderboardPanel(bool downloadDataOnRefresh = false)
    {
        LeaderboardController.Instance.CreateRows(downloadDataOnRefresh);
        StartCoroutine(SwapPanels(LeaderboardPanel));

        if (GameManager.Instance.gameRunning)
        {
            leaderboard_backToMainMenuButton.SetActive(false);
            leaderboard_backToPauseMenuButton.SetActive(true);
            leaderboard_restartGameButton.SetActive(false);
        }
        else if (!GameManager.Instance.gameOver)
        {
            leaderboard_backToMainMenuButton.SetActive(true);
            leaderboard_backToPauseMenuButton.SetActive(false);
            leaderboard_restartGameButton.SetActive(false);
        }
        else
        {
            leaderboard_backToMainMenuButton.SetActive(false);
            leaderboard_backToPauseMenuButton.SetActive(false);
            leaderboard_restartGameButton.SetActive(true);
        }
    }

    public void StartGameButton()
    {
        StartCoroutine(ClosePanel(MainMenuPanel, 0.2f));
        StartCoroutine(GameManager.Instance.StartGame());
    }

    public void QuitButton()
    {
        GameManager.Instance.Quit();
    }
    #endregion

}

