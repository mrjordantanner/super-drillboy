using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;


public class StatsBar : MenuPanel
{
    #region Singleton
    public static StatsBar Instance;
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

    [Header("Labels")]
    public TextMeshProUGUI totalGemsOnAccountLabel;
    public TextMeshProUGUI
        playerNameLabel,
        playCurrencyLabel,
        maxPlayCurrencyLabel,
        playerLevelLabel,
        material1Label, material1MaxLabel,
        material2Label;

    public Image playerAvatar;
    public Slider playerXPBar;

    //private void Start()
    //{ 
    //    RefreshStatsBar();   
    //}

    public void Refresh()
    {
        playerNameLabel.text = PlayerData.Instance.Data.PlayerName;
        playerLevelLabel.text = PlayerData.Instance.Data.PlayerLevel.ToString();

        totalGemsOnAccountLabel.text = ((int)Currency.Instance.LocalData.Gems).ToString();
        playCurrencyLabel.text = Currency.Instance.LocalData.PlayCurrency.ToString();
        maxPlayCurrencyLabel.text = Currency.Instance.LocalData.MaxPlayCurrency.ToString();
        material1Label.text = Currency.Instance.LocalData.Material_1.ToString();
        material1MaxLabel.text = Currency.Instance.LocalData.MaxMaterial_1.ToString();
        material2Label.text = Currency.Instance.LocalData.Material_2.ToString();

        playerAvatar.sprite = PlayerData.Instance.GetPlayerAvatar();
        playerXPBar.value = Utils.CalculateSliderValue(PlayerData.Instance.Data.XP, LevelingSystem.Instance.requiredXP);

    }

    public override void Show(float fadeDuration = 0.2f)
    {
        Refresh();
        base.Show();
    }

}
