using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;


/// <summary>
/// Singleton.  Mini-panel that sits on top of Adventure and Survival Mode play screens.  Shows and toggles current mode and can open a detailed Mode Selection Panel.
/// </summary>
public class GameModeButton : MenuPanel
{
    #region Singleton
    public static GameModeButton Instance;
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

    public Button toggleButton, fullMenuButton;
    public TextMeshProUGUI gameModeLabel;
    public Image gameModeMiniIcon;

    private void Start()
    {
        toggleButton.onClick.AddListener(ToggleGameMode);
        fullMenuButton.onClick.AddListener(ShowGameModeMenu);
    }

    public override void Show(float fadeDuration = 0.2F)
    {
        base.Show(fadeDuration);
        Refresh();
    }

    public void Refresh()
    {
        gameModeLabel.text = GameManager.Instance.gameMode == GameMode.Survival ? "Survival" : "Adventure";
        gameModeMiniIcon.sprite = GameManager.Instance.gameMode == GameMode.Survival ? Icons.Instance.survivalModeMini : Icons.Instance.adventureModeMini;
    }

    void ShowGameModeMenu()
    {
        GameModeScreen.Instance.Show();
    }

    void ToggleGameMode()
    {
        var newGameMode = GameManager.Instance.gameMode == GameMode.Adventure ? GameMode.Survival : GameMode.Adventure;
        GameModeScreen.Instance.SelectGameMode(newGameMode);
    }
}
