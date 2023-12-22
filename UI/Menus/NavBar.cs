using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class NavBar : MenuPanel
{
    #region Singleton
    public static NavBar Instance;
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

    public NavBarButton playButton;
    public NavBarButton shopButton, upgradeButton, skillsButton, menuButton;

    [HideInInspector] public MenuPanel ActiveMenuPanel;

    private void Start()
    {
        ActiveMenuPanel = PlayScreen.Instance;
    }

    public IEnumerator Refresh()
    {
        playButton.icon.sprite = GameManager.Instance.gameMode == GameMode.Adventure ? Icons.Instance.adventureModeMini : Icons.Instance.survivalModeMini;
        menuButton.icon.sprite = Icons.Instance.menu;

        yield return StartCoroutine(Utils.WaitFor(Inventory.Instance, 5f));
        yield return StartCoroutine(Utils.WaitFor(Inventory.Instance.localInventory != null, 5f));
        yield return StartCoroutine(Utils.WaitFor(Inventory.Instance.localInventory.Count > 0, 5f));
        
        // TODO...
        
        var isShopUnlocked = true;
        var isUpgradeUnlocked = true;
        var isSkillsUnlocked = true;

        //var isShopUnlocked = Inventory.Instance.HasLocalItem("MENU_SHOP");
        //print($"NavBar: isShopUnlocked: {isShopUnlocked}");
        shopButton.icon.sprite = isShopUnlocked ? Icons.Instance.shop : Icons.Instance.locked;
        //shopButton.button.enabled = isShopUnlocked;

        //var isUpgradeUnlocked = Inventory.Instance.HasLocalItem("MENU_UPGRADE");
        //print($"NavBar: isUpgradeUnlocked: {isUpgradeUnlocked}");
        upgradeButton.icon.sprite = isUpgradeUnlocked ? Icons.Instance.upgrade : Icons.Instance.locked;
        //upgradeButton.button.enabled = isUpgradeUnlocked;

        //var isSkillsUnlocked = Inventory.Instance.HasLocalItem("MENU_SKILLS");
        //print($"NavBar: isSkillsUnlocked: {isSkillsUnlocked}");
        skillsButton.icon.sprite = isSkillsUnlocked ? Icons.Instance.skills : Icons.Instance.locked;
        //skillsButton.button.enabled = isSkillsUnlocked;

        yield return new WaitForSecondsRealtime(0);
    }

    public override void Show(float fadeDuration = 0.2f)
    {
        StartCoroutine(Refresh());
        base.Show();
    }
  


}
