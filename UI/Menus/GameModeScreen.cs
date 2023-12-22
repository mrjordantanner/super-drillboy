using System.Security.Cryptography;
using TMPro;
using UnityEngine.UI;


public class GameModeScreen : MenuPanel
{
    #region Singleton
    public static GameModeScreen Instance;
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

    public Button advModeButton, srvModeButton;
    public TextMeshProUGUI modeDescription, modeDetailsLabel;

    readonly string advModeDescription = "Unlock new levels and collect unique rewards as you progress through the world of Super Drillboy.";
    readonly string srvModeDescription = "Test your endurance as you play through all unlocked levels in a continuous sequence. Fall as far as you can to attain the highest score.";

    readonly string advModeDetails = "Extra Hazards, Gem Multiplier Bonus, and Level Completion Depth vary based on Challenge Tier.";
    readonly string srvModeDetails = "Extra Hazards and Gem Multiplier Bonus vary based on Challenge Tier.  Earn back 1% of the Play Currency spent on the run for every 100m completed.";

    private void Start()
    {
        advModeButton.onClick.AddListener(SelectAdvMode);
        srvModeButton.onClick.AddListener(SelectSrvMode);
    }

    public override void Show(float fadeDuration = 0.2f)
    {
        base.Show(fadeDuration);
        SelectGameMode((GameMode)Config.Instance.GameMode.Value);
    }

    void SelectAdvMode()
    {
        SelectGameMode(GameMode.Adventure);
    }

    void SelectSrvMode()
    {
        SelectGameMode(GameMode.Survival);
    }

    public void SelectGameMode(GameMode gameMode)
    {
        if (gameMode == GameMode.Adventure)
        {
            advModeButton.Select();
            modeDescription.text = advModeDescription;
            modeDetailsLabel.text = advModeDetails;
            PlayScreen.Instance.modeDescriptionLabel.text = advModeDescription;
            Config.Instance.GameMode.Value = 0;

            PlayScreen.Instance.AdventurePanel.Show();
            PlayScreen.Instance.SurvivalPanel.Hide();
        }
        else
        {
            srvModeButton.Select();
            modeDescription.text = srvModeDescription;
            modeDetailsLabel.text = srvModeDetails;
            PlayScreen.Instance.modeDescriptionLabel.text = srvModeDescription;
            Config.Instance.GameMode.Value = 1;

            PlayScreen.Instance.AdventurePanel.Hide();
            PlayScreen.Instance.SurvivalPanel.Show();
        }

        Config.Instance.GameMode.SaveToPlayerPrefs();
        GameManager.Instance.gameMode = gameMode;
        GameModeButton.Instance.Refresh();
    }
}
