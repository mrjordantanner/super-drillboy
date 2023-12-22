using UnityEngine;
using System.Linq;

/// <summary>
/// Singleton.  Holds Level references and handles Level Select.
/// </summary>
public class LevelController : MonoBehaviour
{
    #region Singleton
    public static LevelController Instance;
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

        //Init();
    }

    [HideInInspector] public LevelTheme[] LevelThemeBank;
    [ReadOnly] public int selectedLevelIndex, levelLoopNumber;
    [ReadOnly] public LevelTheme selectedLevel;

    [Header("Difficulty")]
    [ReadOnly] public int difficultyLevel;  // TODO start saving this to cloud or at least playerPrefs
    [ReadOnly][SerializeField] private float currentExtraHazardSpawnRate;
    public float baseDamageSpawnChance = 0.05f;
    public float damageSpawnChanceFactor = 0.025f;

    [Header("Level Length")]
    [ReadOnly][SerializeField] private int currentCheckpointsPerLevel;
    public int baseCheckpointsPerLevel = 7;
    public float checkpointsIncreaseFactor = 1.25f;   // SRV mode only

    [Header("Play Cost")]
    public int survivalBasePlayCost = 100;
    public float advPlayCostFactor = 1.25f, srvPlayCostFactor = 1.50f;
    //[ReadOnly][SerializeField] private float currentPlayCost;



    int difficultySliderMax;
    public int DifficultySliderMax
    {
        get
        {
            return GameManager.Instance.gameMode switch
            {
                GameMode.Adventure => selectedLevel.maxDifficulty,
                GameMode.Survival => GetLowestMaxDifficulty(),
                _ => difficultySliderMax,
            };
        }
        set
        {
            difficultySliderMax = value;
        }
    }

    public int PlayCost
    {
        get
        {
            return GameManager.Instance.gameMode switch
            {
                GameMode.Adventure => difficultyLevel == 1 ? selectedLevel.basePlayCost : (int)(selectedLevel.basePlayCost * advPlayCostFactor * difficultyLevel),
                GameMode.Survival => difficultyLevel == 1 ? survivalBasePlayCost : (int)(survivalBasePlayCost * srvPlayCostFactor * difficultyLevel),
                _ => survivalBasePlayCost,
            };
        }
    }

    public int CheckpointsPerLevel
    {
        get
        {
            switch (GameManager.Instance.gameMode)
            {
                case GameMode.Adventure:
                    currentCheckpointsPerLevel = difficultyLevel == 1 ? baseCheckpointsPerLevel : (int)(baseCheckpointsPerLevel + (checkpointsIncreaseFactor * difficultyLevel));
                    return currentCheckpointsPerLevel;

                case GameMode.Survival:
                    currentCheckpointsPerLevel = baseCheckpointsPerLevel;
                    return baseCheckpointsPerLevel;

                default: return baseCheckpointsPerLevel;
            }
        }
    }

    public float ExtraHazardSpawnRate
    {
        get
        {
            currentExtraHazardSpawnRate = difficultyLevel == 1 ? baseDamageSpawnChance : baseDamageSpawnChance + (damageSpawnChanceFactor * difficultyLevel);
            return currentExtraHazardSpawnRate;
        }
    }

    [Header("Game Mode Details")]
    public DetailsPanel.DetailType[] AdventureDetails;
    public DetailsPanel.DetailType[] SurvivalDetails;

    [HideInInspector]
    public LevelTheme TheUndergarden, AbyssalGlacier, FathomlessCrypt, LostWellspring;

    public void Init()
    {
        LevelThemeBank = Resources.LoadAll<LevelTheme>("LevelThemes");
        InitializeLevels();

        TheUndergarden = GetLevelByName(nameof(TheUndergarden));
        AbyssalGlacier = GetLevelByName(nameof(AbyssalGlacier));
        FathomlessCrypt = GetLevelByName(nameof(FathomlessCrypt));
        LostWellspring = GetLevelByName(nameof(LostWellspring));

        UpdateLevelTheme();
        SetDifficulty(1);   // Todo load this from cloudsave or playerprefs
    }

    public int SetDifficulty(int newDifficulty)
    {
        difficultyLevel = newDifficulty;
        return difficultyLevel;
    }

    //float CalculateDamageSpawnChance()
    //{
    //    currentDamageSpawnChance = difficultyLevel == 1 ? baseDamageSpawnChance : baseDamageSpawnChance + (damageSpawnChanceFactor * difficultyLevel);
    //    return currentDamageSpawnChance;
    //}

    //int CalculateNumberOfCheckpoints()
    //{
    //    // Level length doesn't change in Survival mode
    //    if (GameManager.Instance.gameMode == GameMode.Survival) return baseCheckpointsPerLevel;

    //    currentCheckpointsPerLevel = difficultyLevel == 1 ? 
    //        baseCheckpointsPerLevel : (int)(baseCheckpointsPerLevel + (checkpointsIncreaseFactor * difficultyLevel));
    //    return currentCheckpointsPerLevel;
    //}

    //public float CalculatePlayCost()
    //{
    //    var increaseFactor = GameManager.Instance.gameMode == GameMode.Adventure ? advPlayCostFactor : srvPlayCostFactor;
    //    var baseCost = GameManager.Instance.gameMode == GameMode.Adventure ? selectedLevel.basePlayCost : survivalBasePlayCost;

    //    currentPlayCost = difficultyLevel == 1 ? baseCost : (int)(baseCost * increaseFactor * difficultyLevel);

    //    var adv = difficultyLevel == 1 ? selectedLevel.basePlayCost : (int)(selectedLevel.basePlayCost * advPlayCostFactor * difficultyLevel);
    //    var srv = difficultyLevel == 1 ? survivalBasePlayCost : (int)(survivalBasePlayCost * srvPlayCostFactor * difficultyLevel);

    //    return currentPlayCost;
    //}

    public void InitializeLevels()
    {
        foreach (var levelTheme in LevelThemeBank)
        {
            levelTheme.Init();
        }
    }

    public LevelTheme GetLevelByName(string levelName)
    {
        foreach (var level in LevelThemeBank)
        {
            if (level.name == levelName)
            {
                return level;
            }
        }

        Debug.LogError($"Unable to get LevelTheme by name: {levelName}");
        return null;
    }

    public void UpdateLevelTheme()
    {
        if (selectedLevelIndex > LevelThemeBank.Length - 1)
        {
            selectedLevelIndex = 0;
            levelLoopNumber++;
        }
        selectedLevel = LevelThemeBank[selectedLevelIndex];
    }

    public void CycleSelectedLevel(int direction)
    {
        selectedLevelIndex += direction;

        if (selectedLevelIndex < 0)
        {
            selectedLevelIndex = LevelThemeBank.Length - 1;
        }
        else if (selectedLevelIndex >= LevelThemeBank.Length)
        {
            selectedLevelIndex = 0;
        }

        selectedLevel = LevelThemeBank[selectedLevelIndex];
        PlayScreen.Instance.AdventurePanel.RefreshSelectedLevelInfo();

    }

    public int GetLowestMaxDifficulty()
    {
        var maxDiffs = LevelThemeBank.Select(x => x.maxDifficulty);
        return maxDiffs.Min();
    }
}
