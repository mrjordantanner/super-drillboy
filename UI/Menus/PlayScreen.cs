using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayScreen : MenuPanel
{
    #region Singleton
    public static PlayScreen Instance;
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

    [Header("Mode Panels")]
    public AdventurePanel AdventurePanel;
    public SurvivalPanel SurvivalPanel;

    [Header("Details")]
    public DetailsPanel detailsPanel;
    public TextMeshProUGUI modeDescriptionLabel;
    public Color diffDetailColor, diffValueColor, rewardDetailColor, rewardValueColor;

    [Header("Start Button")]
    public Button startButton;
    public TextMeshProUGUI playCostLabel;
    public Color canAffordColor, cannotAffordColor;

    [Header("Challenge Tier")]
    public Slider difficultySlider;
    public TextMeshProUGUI difficultyText, maxDifficultyText;

    public void Init()
    {
        MapGenerator.Instance.ToggleScrollingBackgrounds(false);
        Menu.Instance.TitleScrollingBackground.SetActive(true);
        Menu.Instance.FullscreenMenuBackground.SetActive(true);

        detailsPanel.CreateAllDetails();
        AdventurePanel.Init();
        SurvivalPanel.Init();
        //RefreshAll();  
        //

        difficultySlider.onValueChanged.AddListener(OnSliderChanged);
    }

    public override void Show(float fadeDuration = 0.2f)
    {
        SetSliderMax();
        RefreshAll();
        GameModeButton.Instance.Show();

        if (GameManager.Instance.gameMode == GameMode.Adventure)
        {
            AdventurePanel.Show();
            SurvivalPanel.Hide();
        }
        else
        {
            AdventurePanel.Hide();
            SurvivalPanel.Show();
        }

        base.Show(fadeDuration);
    }

    //public void OnToggleGameMode(GameMode newGameMode)
    //{

    //}

    void SetSliderMax()
    {
        difficultySlider.maxValue = LevelController.Instance.DifficultySliderMax;
    }

    void OnSliderChanged(float sliderValue)
    {
        LevelController.Instance.SetDifficulty((int)sliderValue);

        if (GameManager.Instance.gameMode == GameMode.Adventure)
        {
            LevelController.Instance.selectedLevel.currentDifficulty = LevelController.Instance.difficultyLevel;

        }
        else
        {

        }

        RefreshPlayCost();
        RefreshDfficultyLabels();
        detailsPanel.RefreshDetails();
    }

    public void RefreshAll()
    {
        if (GameManager.Instance.gameMode == GameMode.Adventure)
        {
            AdventurePanel.RefreshSelectedLevelInfo();
            //maxDepthLabel.text = LevelManager.Instance.srvMaxDepthReached.ToString();
        }
        else
        {
            LevelController.Instance.SetDifficulty(LevelController.Instance.selectedLevel.currentDifficulty);
        }

        detailsPanel.RefreshDetails();
        RefreshDfficultyLabels();
        RefreshPlayCost();
    }

    public void RefreshPlayCost()
    {
        playCostLabel.text = LevelController.Instance.PlayCost.ToString();
        playCostLabel.color = Currency.Instance.LocalData.PlayCurrency >= LevelController.Instance.PlayCost ? canAffordColor : cannotAffordColor;
    }

    public void RefreshDfficultyLabels()
    {
        if (GameManager.Instance.gameMode == GameMode.Adventure)
        {
            difficultyText.text = LevelController.Instance.selectedLevel.currentDifficulty.ToString();
            maxDifficultyText.text = LevelController.Instance.selectedLevel.maxDifficulty.ToString();
        }
        else
        {
            difficultyText.text = LevelController.Instance.difficultyLevel.ToString();
            maxDifficultyText.text = LevelController.Instance.GetLowestMaxDifficulty().ToString();
        }

    }

    // Button callback
    public void OnTapStartButton()
    {
        if (Currency.Instance.LocalData.PlayCurrency >= LevelController.Instance.PlayCost)
        {
            Menu.Instance.PlayStartSound();
            GameManager.Instance.InitializeNewRun();
        }
        else
        {
            // TODO Not enough Play Currency - buy more now?  Can either watch an Ad or pay 5000 gems, etc
            // TODO make a single-button warning dialog box for messages like these
            print("You do not have enough Play Currency to play this level.  Buy more?");
        }
    }

    bool IsAdventureMode()
    {
        return GameManager.Instance.gameMode == GameMode.Adventure;
    }

    public override void Hide(float fadeDuration = 0.2F)
    {
        base.Hide(fadeDuration);
        GameModeButton.Instance.Hide();
    }

}
