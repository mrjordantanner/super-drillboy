using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PauseMenu : MenuPanel
{
    #region Singleton
    public static PauseMenu Instance;
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

    public CanvasGroup Panel;

    public TextMeshProUGUI gemsCollectedLabel;
    public TextMeshProUGUI 
        depthLabel,
        gameModeLabel,
        difficultyLabel;

    public DetailsPanel detailsPanel;

    public override void Show(float fadeDuration = 0.1f)
    {
        detailsPanel.CreateAllDetails();
        detailsPanel.Show();

        RefreshAll();
        base.Show();
    }

    public void RefreshAll()
    {
        detailsPanel.RefreshDetails();

        gemsCollectedLabel.text = ((int)GemController.Instance.gemsCollectedThisRun).ToString();
        depthLabel.text = $"{(int)PlayerManager.Instance.currentDepth} m";
        difficultyLabel.text = $"Challenge Tier {LevelController.Instance.difficultyLevel}";
        gameModeLabel.text = GameManager.Instance.gameMode == GameMode.Adventure ? "Adventure Mode" : "Survival Mode";
    }

}
