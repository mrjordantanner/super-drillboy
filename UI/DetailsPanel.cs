using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DetailsPanel : MenuPanel
{
    public enum DetailType { Hazards, Depth, GemMultiplier, PlayCurrency }

    public struct Detail
    {
        public DetailType type;
        public string text;
        public Sprite icon;
        public bool isDifficulty;
    }

    [Header("--")]
    public GameObject DetailsRowPrefab;
    public GameObject DifficultyContainer, BonusContainer;

    [HideInInspector] public List<DetailsRow> DetailsRows = new();
    List<Detail> Details;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        Detail hazardDetail = new() { type = DetailType.Hazards, text = "Extra Hazards", icon = Icons.Instance.hazard, isDifficulty = true };
        Detail levelLengthDetail = new() { type = DetailType.Depth, text = "Depth", icon = Icons.Instance.levelLength, isDifficulty = true };
        Detail gemMultiplierDetail = new() { type = DetailType.GemMultiplier, text = "Gem Multiplier", icon = Icons.Instance.gemMultiplier, isDifficulty = false };
        Detail playCurrencyDetail = new() { type = DetailType.PlayCurrency, text = "Play Currency", icon = Icons.Instance.playCurrencyMini, isDifficulty = false };

        Details = new() { hazardDetail, levelLengthDetail, gemMultiplierDetail, playCurrencyDetail };
    }

    public void CreateAllDetails()
    {
        ClearDetails();
        var DetailsToDisplay = GameManager.Instance.gameMode == GameMode.Adventure ? LevelController.Instance.AdventureDetails : LevelController.Instance.SurvivalDetails;

        foreach (var detail in Details)
        {
            if (DetailsToDisplay.Contains(detail.type))
            {
                var container = detail.isDifficulty ? DifficultyContainer : BonusContainer;
                CreateDetailsRow(detail, container);
            }
        }

        RefreshDetails();
    }

    public DetailsRow CreateDetailsRow(Detail detail, GameObject Container)
    {
        var Details = Instantiate(DetailsRowPrefab, Container.transform.position, Quaternion.identity, Container.transform);
        var detailsRow = Details.GetComponent<DetailsRow>();
        detailsRow.Detail = detail;
        DetailsRows.Add(detailsRow);
        return detailsRow;
    }

    public void ClearDetails()
    {
        foreach (var detailsRow in DetailsRows)
        {
            Destroy(detailsRow.gameObject);
        }
        DetailsRows.Clear();
    }

    public void RefreshDetails()
    {
        foreach (var detailsRow in DetailsRows)
        {
            switch (detailsRow.Detail.type)
            {
                case DetailType.Hazards:
                   // if (hazardDetailsRow != null)
                        detailsRow.Set($"+{LevelController.Instance.ExtraHazardSpawnRate * 100}%");
                    break;

                case DetailType.Depth:
                    //if (levelLengthDetailsRow != null)
                    //{
                        var depthObjectiveValue = LevelController.Instance.difficultyLevel == 1 ? 0 : LevelController.Instance.checkpointsIncreaseFactor * LevelController.Instance.difficultyLevel;
                        detailsRow.Set($"+{depthObjectiveValue * 100}% m");
                    //}
                    break;

                case DetailType.GemMultiplier:
                    // if (gemMultiplierDetailsRow != null)
                    detailsRow.Set($"+{(float)LevelController.Instance.difficultyLevel}x");
                    break;

                case DetailType.PlayCurrency:
                    // if (playCurrencyDetailsRow != null)
                    detailsRow.Set($"{LevelController.Instance.PlayCost * 0.01f} per 100m");
                    break;
            }
        }
    }
}
