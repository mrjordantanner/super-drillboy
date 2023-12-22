using System.Drawing;
using System.Threading.Tasks;
using UnityEngine;


public enum GemType { Small, Large }

/// <summary>
/// Singleton class responsible for Gem-related values and calculations.
/// </summary>
public class GemController : MonoBehaviour
{
    #region Singleton
    public static GemController Instance;
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

    [Header("Gems")]
    [ReadOnly]
    public int smallGemValue = 1;
    public int largeGemValue = 2,
        smallGemValueTowardSkill = 2,
        largeGemValueTowardSkill = 5;

    [Header("Multiplier")]
    [ReadOnly]
    public float totalGemMultiplier;
    public float gemMultiplierBonus;  // Additive bonus from increased Difficulty
    public float 
        gemLevelMultiplier,
        gemLevelMultiplierIncrement = 1,
        gemDepthMultiplier,
        gemDepthMultiplierIncrement = 0.25f;

    [Header("Multiplier Depth")]
    public int gemDepthMultiplierDistanceQuota = 500;
    [ReadOnly]
    public int gemDepthFactor;
    [ReadOnly]
    public float distanceFromLastSpawnPoint;

    //[Header("Scoring")]
    //public int gemPointValue = 3;

    [Header("Gem Stats")]
    [ReadOnly]
    public float gemsCollectedThisRun;
    [ReadOnly]
    public float gemsCollectedSinceLastSync;

    public void Init()
    {
        ResetGemCounters();
        ResetGemLevelMultiplier();
        ResetGemDepthMultiplier();

        CalculateGemDepthMultiplier();
    }

    public void ResetGemCounters()
    {
        SkillController.Instance.ResetResource();
        gemsCollectedThisRun = 0;
        gemsCollectedSinceLastSync = 0;
    }

    void ResetGemLevelMultiplier()
    {
        gemLevelMultiplier = 1;
    }

    public void ResetGemDepthMultiplier()
    {
        gemDepthMultiplier = 0;
        gemDepthFactor = 0;
    }

    private void Update()
    {
        if (GameManager.Instance.gameRunning && PlayerManager.Instance.player)
        {
            CalculateGemMultiplier();
        }
    }

    float CalculateGemDepthMultiplier()
    {
        if (PlayerManager.Instance.player)
        {
            // Whenever player is spawned/respawned, set LastRespawnPoint to that point and ResetGemDepthMultiplier()
            // Every frame/frequently, see if the player has traveled distanceQuota * depthInterval meters from LastRespawnPoint
            // If yes, increment the depthInterval
            distanceFromLastSpawnPoint = Utils.GetDistance(PlayerManager.Instance.player.transform.position, PlayerManager.Instance.lastSpawnPoint);
            if (distanceFromLastSpawnPoint >= gemDepthMultiplierDistanceQuota * (gemDepthFactor + 1))
            {
                gemDepthFactor++;
            }

            gemDepthMultiplier = gemDepthMultiplierIncrement * gemDepthFactor;
            return gemDepthMultiplier;
        }

        return 1;
    }

    public float CalculateGemMultiplier()
    {
        totalGemMultiplier = LevelController.Instance.difficultyLevel + CalculateGemDepthMultiplier() + gemMultiplierBonus;
        if (HUD.Instance) HUD.Instance.UpdateGemMultiplier();

        return totalGemMultiplier;
    }

    public void CollectGem(GemType gemType)
    {
        float multipliedValue = 0;
        int valueTowardSkill = 0;
        switch (gemType)
        {
            case GemType.Small:
                multipliedValue = smallGemValue * totalGemMultiplier;
                valueTowardSkill = smallGemValueTowardSkill;
                break;

            case GemType.Large:
                multipliedValue = largeGemValue * totalGemMultiplier;
                valueTowardSkill = largeGemValueTowardSkill;
                break;
        }


        SkillController.Instance.GainResource(valueTowardSkill);
        gemsCollectedThisRun += multipliedValue;
        gemsCollectedSinceLastSync += multipliedValue;

        var sound = gemType == GemType.Large ? AudioManager.Instance.soundBank.CollectLargeGem : AudioManager.Instance.soundBank.CollectSmallGem;
        sound.Play();

        PlayerManager.Instance.player.CollectGemFlash();
        StartCoroutine(HUD.Instance.GemIconFlashOnce());

        HUD.Instance.gemsCollectedThisRunLabel.text = ((int)gemsCollectedThisRun).ToString();
        HUD.Instance.totalGemsOnAccountLabel.text = ((int)Currency.Instance.LocalData.Gems).ToString();
        HUD.Instance.PopGemText();
    }

    public async Task SyncAndResetGemCache()
    {
        Currency.Instance.IncrementCurrency(Currency.Instance.GemsId, (int)gemsCollectedSinceLastSync);
        gemsCollectedSinceLastSync = 0;
    }


}
