using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System;


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

    #region Declarations

    [ReadOnly]
    public bool enteringText;

    [Header("Dialog Box")]
    public GameObject DialogBoxPrefab;
    public GameObject DialogBoxContainer;

    [Header("Buttons & BGs")]
    public GameObject TitleScrollingBackground;
    public GameObject FullscreenMenuBackground;
    public GameObject settings_backToMainMenuButton, settings_backToPauseMenuButton,
        leaderboard_backToMainMenuButton, leaderboard_backToPauseMenuButton, leaderboard_restartGameButton,
        instructions_backToMainMenuButton, instructions_backToPauseMenuButton;

    [Header("Non-Singleton Menus")]
    public MenuPanel SettingsMenuPanel;
    public MenuPanel LeaderboardMenuPanel,
        NameEntryMenuPanel,
        InstructionsMenuPanel;

    [Header("Purchase Menus")]
    public PurchaseMenu ShopMenu;
    public PurchaseMenu UpgradeMenu;

    [Header("Misc")]
    public TMP_InputField nameInputField;

    [HideInInspector] public MenuPanel ActiveMenuPanel;
    [HideInInspector] public SettingsSlider[] settingsSliders;

    public TextMeshProUGUI versionNumberText;
    public TextMeshProUGUI companyNameText;

    [Header("Menu Panels")]
    public TitleScreen titleScreen;
    public PauseMenu pauseMenu;
    public ControlsMenu controlsMenu;
    public OptionsMenu optionsMenu;
    public ScorePanel scorePanel;
    public SettingsDebugTable settingsDebugTable;

    [Header("Misc")]
    public Image toggleSoundIconImage;   // TODO probably move this

    #endregion

    public void Init()
    {
        versionNumberText.text = $"v{Application.version}";
        companyNameText.text = Application.companyName;

        settingsSliders = FindObjectsOfType<SettingsSlider>();
        RefreshAllSliders();
        SetAudioToggleIcon();

        nameInputField.onEndEdit.AddListener(OnInputEndEdit);
        nameInputField.characterLimit = 10;

        if (GameManager.Instance.testMode == TestMode.Normal)
        {
            titleScreen.Show();
            HUD.Instance.screenFader.FadeIn(1f);
        }

        MenuBackgroundsSetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            GameManager.Instance.RestartGame();
        }

        if (Input.GetKeyDown(KeyCode.M) && GameManager.Instance.gameRunning && !enteringText)
        {
            if (!GameManager.Instance.gamePaused)
            {
                ShowPauseMenu();
                PlayClickSound();
            }
            else
            {
                if (ActiveMenuPanel == PauseMenu.Instance.Panel)
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

    public void RefreshAllSliders()
    {
        foreach (var slider in settingsSliders)
        {
            slider.Refresh();
        }
    }

    public void RefreshAllMenuPanels()
    {
        UpgradeMenu.RefreshScreen();
        StatsBar.Instance.Refresh();
        PlayScreen.Instance.RefreshAll();
    }

    public void OnInputEndEdit(string username)
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            ConfirmNameEntry();
        }
    }

    public void MenuBackgroundsSetActive(bool value)
    {
        FullscreenMenuBackground.SetActive(value);
        TitleScrollingBackground.SetActive(value);
    }


    public void OnToggleAudioButtonClick()
    {
        // TODO save/load this from PlayerPrefs and set on app start

        AudioManager.Instance.ToggleAudio();
        SetAudioToggleIcon();
    }

    void SetAudioToggleIcon()
    {
        toggleSoundIconImage.sprite = AudioManager.Instance.audioMuted ? Icons.Instance.audioDisabled : Icons.Instance.audioEnabled;
    }

    public void CreateDialogBox(
        string headerText,
        string messageText,
        string descriptionText,
        Sprite sprite,
        Action onOk,
        Action onCancel)
    {
        var NewDialogBox = Instantiate(DialogBoxPrefab, DialogBoxContainer.transform.position, Quaternion.identity, DialogBoxContainer.transform);
        var dialogBox = NewDialogBox.GetComponent<DialogBox>();
        dialogBox.Initialize(headerText, messageText, descriptionText, sprite, onOk, onCancel);
    }


    #region Panel Methods
    public IEnumerator MainMenuTransitionFromTitle()
    {
        TitleScrollingBackground.SetActive(true);

        titleScreen.introSequence.backgroundSprite.DOFade(0, 1.5f);

        PlayScreen.Instance.Show(1);
        StatsBar.Instance.Show(1);
        NavBar.Instance.Show(1);
        
        yield return new WaitForSecondsRealtime(1);

        titleScreen.introSequence.gameObject.SetActive(false);
        titleScreen.introSequence.logoCanvasGroup.gameObject.SetActive(false);
        titleScreen.Hide();

        //TitleScrollingBackground.SetActive(false);
        AudioManager.Instance.FadeMusicIn(AudioManager.Instance.titleScreenMusic);

        ActiveMenuPanel = PlayScreen.Instance;
    }

    public void ShowPauseMenu()
    {
        GameManager.Instance.Pause();
        FullscreenMenuBackground.SetActive(true);
        FadeOutCanvasGroup(HUD.Instance.mobileUI);
        HUD.Instance.Hide();
        PauseMenu.Instance.Show();
    }

    public void ToggleCanvasGroup(
        bool enabled,
        CanvasGroup canvasGroup,
        float targetOpacity,
        float fadeDuration = 0.3f)
    {
        canvasGroup.DOFade(targetOpacity, fadeDuration).SetUpdate(UpdateType.Normal, true);
        canvasGroup.interactable = enabled;
        canvasGroup.blocksRaycasts = enabled;
    }

    public void FadeInCanvasGroup(CanvasGroup canvasGroup, float fadeDuration = 0.2f)
    {
        canvasGroup.DOFade(1f, fadeDuration).SetUpdate(UpdateType.Normal, true);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void FadeOutCanvasGroup(CanvasGroup canvasGroup, float fadeDuration = 0.2f)
    {
        canvasGroup.DOFade(0f, fadeDuration).SetUpdate(UpdateType.Normal, true);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    public IEnumerator SwapMenus(MenuPanel newPanel)
    {
        ActiveMenuPanel.Hide();
        yield return new WaitForSecondsRealtime(0.2f);
        newPanel.Show();
    }
    #endregion


    #region Button Callbacks
    public void Swap(MenuPanel newPanel)
    {
        StartCoroutine(SwapMenus(newPanel));
    }

    public void BackToGame()
    {
        //menuActive = false;
        FullscreenMenuBackground.SetActive(false);
        HUD.Instance.Show();
        ActiveMenuPanel.Hide();
        FadeInCanvasGroup(HUD.Instance.mobileUI);
        ControlManager.Instance.ActivateMobileControls();
        GameManager.Instance.Unpause();
    }

    public void BackToPauseMenu()
    {
        if (HUD.Instance.mobileUI.alpha > 0) FadeOutCanvasGroup(HUD.Instance.mobileUI);
        controlsMenu.CloseJoystickPanel();
        controlsMenu.CloseInputButtonsPanel();
        StartCoroutine(SwapMenus(pauseMenu));
    }

    public void ReturnToMainMenu()
    {
        if (HUD.Instance.mobileUI.alpha > 0) FadeOutCanvasGroup(HUD.Instance.mobileUI);
        controlsMenu.CloseJoystickPanel();
        controlsMenu.CloseInputButtonsPanel();
        StartCoroutine(SwapMenus(PlayScreen.Instance));
    }

    public void PlayClickSound()
    {
        AudioManager.Instance.soundBank.MenuClick.Play();
    }

    public void PlayStartSound()
    {
        AudioManager.Instance.soundBank.SuperDrill.Play();
    }

    // Button callback
    public void RestartGame()
    {
        StartCoroutine(RestartGameFromMenu());
    }

    IEnumerator RestartGameFromMenu()
    {
        HUD.Instance.screenFader.FadeOut(1f);
        AudioManager.Instance.musicAudioSource.DOFade(0, 0.5f).SetUpdate(UpdateType.Normal, true);
        yield return new WaitForSecondsRealtime(0.5f);
        GameManager.Instance.RestartGame();
    }

    public void ShowNameEntryPanel()
    {
        if (DevTools.Instance.devToolsWereUsed)
        {
            GameManager.Instance.EndRun();
        }

        enteringText = true;
        if (nameInputField != null)
        {
            nameInputField.Select();
            nameInputField.ActivateInputField();
        }
        StartCoroutine(SwapMenus(NameEntryMenuPanel));
    }

    public void ConfirmNameEntry()
    {
        enteringText = false;
        var username = nameInputField.text;

        Dreamlo.Instance.SaveUserAndUpload(username, GameManager.Instance.playerScore);
        NameEntryMenuPanel.Hide();
        OpenLeaderboardPanel(true);
        //LeaderboardController.Instance.FocusOnRow();
        LeaderboardController.Instance.ScrollToRow(LeaderboardController.Instance.currentUserIndex);
    }

    public void ShowInstructionsPanel()
    {
        var value = GameManager.Instance.gameRunning;

        instructions_backToMainMenuButton.SetActive(!value);
        instructions_backToPauseMenuButton.SetActive(value);

        StartCoroutine(SwapMenus(InstructionsMenuPanel));
    }

    public void OpenSettingsPanel()
    {
        RefreshAllSliders();
        controlsMenu.RefreshControlsPanel();
        StartCoroutine(SwapMenus(SettingsMenuPanel));

        var value = GameManager.Instance.gameRunning;

        settings_backToMainMenuButton.SetActive(!value);
        settings_backToPauseMenuButton.SetActive(value);
    }

    public void OpenLeaderboardPanel(bool downloadDataOnRefresh = false)
    {
        LeaderboardController.Instance.CreateRows(downloadDataOnRefresh);
        StartCoroutine(SwapMenus(LeaderboardMenuPanel));

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

    public void QuitButton()
    {
        GameManager.Instance.Quit();
    }
   
    #endregion

}

