using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Misc Menu Screen accessed from Bottom Nav Bar.
/// </summary>
public class MiscMenuScreen : MenuPanel
{
    #region Singleton
    public static MiscMenuScreen Instance;
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

    public TextMeshProUGUI versionNumberText, companyNameText;

    public override void Show(float fadeDuration = 0.2F)
    {
        base.Show(fadeDuration);
        versionNumberText.text = $"v{Application.version}";
        companyNameText.text = Application.companyName;
    }


}
