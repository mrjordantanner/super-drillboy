using UnityEngine;
using UnityEngine.UI;


public class LevelingSystem : MonoBehaviour
{
    #region Singleton
    public static LevelingSystem Instance;
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

    public float
        startingRequiredXP = 1000,
        xpIncreaseFactor = 0.5f,
        requiredXP;

    public Slider levelProgressSlider;

    void Start()
    {
        requiredXP = CalculateRequiredXP();
        UpdateLevelProgressSliders();
        HUD.Instance.UpdatePlayerLevel();
    }

    public void GainXP(int xpAmount)
    {
        PlayerData.Instance.Data.XP += xpAmount;
        PlayerData.Instance.Data.TotalXP += xpAmount;

        if (PlayerData.Instance.Data.XP >= requiredXP)
        {
            LevelUp();
        }

        UpdateLevelProgressSliders();
    }

    void LevelUp()
    {
        PlayerData.Instance.Data.PlayerLevel++;
        var xpRemainder = PlayerData.Instance.Data.XP > requiredXP ? PlayerData.Instance.Data.XP - requiredXP : 0;
        PlayerData.Instance.Data.XP = (int)xpRemainder;
        requiredXP = CalculateRequiredXP();

        HUD.Instance.UpdatePlayerLevel();
        StatsBar.Instance.Refresh();

        HUD.Instance.ShowMessage($"LEVEL UP!  Reached Level {PlayerData.Instance.Data.PlayerLevel}",
            0.2f, 3f, 0.5f, true);

        print($"LEVEL UP!  Reached Level {PlayerData.Instance.Data.PlayerLevel}");
    }

    public float CalculateRequiredXP()
    {
        requiredXP = startingRequiredXP * PlayerData.Instance.Data.PlayerLevel * (1 + xpIncreaseFactor);
        return requiredXP;
    }

    // Update in-game HUD slider and StatsBar slider
    void UpdateLevelProgressSliders()
    {
        var value = Utils.CalculateSliderValue(PlayerData.Instance.Data.XP, requiredXP);

        if (levelProgressSlider != null)
        {
            levelProgressSlider.value = value;
        }

        StatsBar.Instance.playerXPBar.value = value;
    }

}
