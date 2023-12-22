using UnityEngine;
using UnityEngine.UI;

public class Icons : MonoBehaviour
{
    #region Singleton
    public static Icons Instance;
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

    public Sprite locked;

    [Header("Game Mode")]
    public Sprite adventureModeMini;
    public Sprite 
        adventureModeFull, 
        survivalModeMini, 
        survivalModeFull;

    [Header("Nav Bar")]
    public Sprite shop;
    public Sprite upgrade, skills, menu;

    [Header("Stats Bar")]
    public Sprite gemsMini;
    public Sprite playCurrencyMini, material1Mini, material2Mini;

    //[Header("Player Avatars")]
    //public Sprite playerAvatarDefault;
    //public Sprite playerAvatar1, playerAvatar2;

    [Header("Shop")]
    public Sprite gemPurchaseSmall;
    public Sprite gemPurchaseMedium, gemPurchaseLarge;

    [Header("Details")]
    public Sprite hazard;
    public Sprite gemMultiplier, playCurrency, levelLength;

    [Header("Audio")]
    public Sprite audioDisabled;
    public Sprite audioEnabled;

    //[Header("Upgrades")]
    //public Sprite

}
