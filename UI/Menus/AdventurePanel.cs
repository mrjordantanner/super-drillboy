using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;


public class AdventurePanel : MenuPanel
{
    public TextMeshProUGUI selectedLevelNameLabel;
    public TextMeshProUGUI
        selectedLevelMaxDepthLabel;

    public Image selectedLevelImage, levelRewardImage;

    public void Init()
    {
        //PlayScreen.Instance.RefreshAll();
        
    }

    public override void Show(float fadeDuration = 0.2f)
    {
        GameModeButton.Instance.Show();
        PlayScreen.Instance.detailsPanel.CreateAllDetails();
        PlayScreen.Instance.RefreshAll();

        base.Show(fadeDuration);
    }


    //void OnSliderChanged(float sliderValue)
    //{
    //    LevelManager.Instance.SetAndCalculateDifficulty((int)sliderValue);
    //    LevelManager.Instance.selectedLevel.currentDifficulty = LevelManager.Instance.difficultyLevel;
    //    PlayScreen.Instance.RefreshPlayCost();
    //    RefreshDfficultyLabels();
    //    detailsPanel.RefreshDetails();
    //}

    //public void RefreshAll()
    //{
    //    LevelManager.Instance.SetAndCalculateDifficulty(LevelManager.Instance.selectedLevel.currentDifficulty);

    //   // detailsPanel.RefreshDetails();
    //    RefreshDfficultyLabels();
    //    RefreshSelectedLevelInfo();
    //}

    public void RefreshSelectedLevelInfo()
     {
        PlayScreen.Instance.difficultySlider.value = LevelController.Instance.selectedLevel.currentDifficulty;
        PlayScreen.Instance.difficultySlider.maxValue = LevelController.Instance.selectedLevel.maxDifficulty;
        PlayScreen.Instance.difficultyText.text = LevelController.Instance.difficultyLevel.ToString();
        PlayScreen.Instance.maxDifficultyText.text = LevelController.Instance.selectedLevel.maxDifficulty.ToString();

        selectedLevelNameLabel.text = LevelController.Instance.selectedLevel.levelName;
        selectedLevelImage.sprite = LevelController.Instance.selectedLevel.levelPreviewSprite;
        
        PlayScreen.Instance.RefreshPlayCost();
    }

    //void RefreshDfficultyLabels()
    //{
    //    PlayScreen.Instance.difficultyText.text = LevelManager.Instance.selectedLevel.currentDifficulty.ToString();
    //    PlayScreen.Instance.maxDifficultyText.text = LevelManager.Instance.selectedLevel.maxDifficulty.ToString();
    //}

    public override void Hide(float fadeDuration = 0.2F)
    {
        base.Hide(fadeDuration);
    }

}
