using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SurvivalPanel : MenuPanel
{
    [Header("Level Preview")]
    public GameObject LevelPreviewRowPrefab;
    public GameObject LevelPreviewContainer;
    List<GameObject> LevelPreviewObjects = new();

    public void Init()
    {
        CreateLevelPreviewRows();
    }

    public override void Show(float fadeDuration = 0.2f)
    {
        GameModeButton.Instance.Show();
        CreateLevelPreviewRows();
        PlayScreen.Instance.detailsPanel.CreateAllDetails();
        PlayScreen.Instance.RefreshAll();
        //RefreshAll();

        base.Show(fadeDuration);
    }

    void CreateLevelPreviewRows()
    {
        ClearLevelPreviewRows();

        foreach (var level in LevelController.Instance.LevelThemeBank)
        {
            var NewRow = Instantiate(LevelPreviewRowPrefab, LevelPreviewContainer.transform.position, Quaternion.identity, LevelPreviewContainer.transform);
            NewRow.GetComponent<LevelPreviewRow>().Set(level.previewIcon, level.levelName);
            LevelPreviewObjects.Add(NewRow);
        }
    }

    void ClearLevelPreviewRows()
    {
        foreach (var levelPreview in LevelPreviewObjects)
        {
            Destroy(levelPreview);
        }
        LevelPreviewObjects.Clear();
    }

    //void OnSliderChanged(float sliderValue)
    //{
    //    LevelManager.Instance.SetAndCalculateDifficulty((int)sliderValue);
    //    PlayScreen.Instance.RefreshPlayCost();
    //    RefreshDfficultyLabels();
    //    detailsPanel.RefreshDetails();
    //}

    //public void RefreshAll()
    //{
    //    LevelManager.Instance.SetAndCalculateDifficulty(LevelManager.Instance.difficultyLevel);

    //    PlayScreen.Instance.difficultySlider.maxValue = LevelManager.Instance.GetLowestMaxDifficulty();
    //    PlayScreen.Instance.detailsPanel.RefreshDetails();
    //    PlayScreen.Instance.RefreshDfficultyLabels();
    //    //maxDepthLabel.text = LevelManager.Instance.srvMaxDepthReached.ToString();

    //    PlayScreen.Instance.RefreshPlayCost();
    //}

    //public void RefreshDfficultyLabels()
    //{
    //    PlayScreen.Instance.difficultyText.text = LevelManager.Instance.difficultyLevel.ToString();
    //    PlayScreen.Instance.maxDifficultyText.text = LevelManager.Instance.GetLowestMaxDifficulty().ToString();
    //}

    public override void Hide(float fadeDuration = 0.2F)
    {
        base.Hide(fadeDuration);
    }

}
