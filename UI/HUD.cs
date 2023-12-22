using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;


public class HUD : MenuPanel
{
    #region Singleton
    public static HUD Instance;
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

    [Header("Input")]
    public CanvasGroup mobileUI;

    [Header("Labels")]
    public TextMeshProUGUI depthLabel;
    public TextMeshProUGUI livesLabel,
        scoreLabel,
        playerLevelLabel;

    public CanvasGroup HUDButtonsGroup;
    public CanvasGroup worldUI;
    public ScreenFader screenFader;
    public GameObject screenFlash;
    CanvasGroup screenFlashCanvas;

    public TextMeshProUGUI levelNameText;
    public MovingUIElement livesChangedUIElement;

    [Header("Health & Armor")]
    public HitpointsUI hitpointsUI;

    [Header("Gems")]
    public Image gemIcon;
    public Image gemIconFill;
    public Sprite gemFlashSprite;
    public MovingUIElement gemsChangedUIElement;
    public Animator gemAnim;
    public Sprite gemNormalSprite;
    public TextMeshProUGUI gemMultiplierIntegerLabel, gemMultiplierRemainderLabel;
    public TextMeshProUGUI gemsCollectedThisRunLabel,
        totalGemsOnAccountLabel;

    [Header("Message Text")]
    public CanvasGroup messageCanvasGroup;
    public TextMeshProUGUI messageText;
    public BlinkingText messageBlinkingText;

    //[Header("Floating Text")]
    //public GameObject FloatingTextPrefab;
    //public float floatingTextOffsetX = -0.5f;
    //public float floatingTextOffsetY = 2f;
    //public float floatingTextMoveAmount = 1;

    private void Update()
    {
        if (GameManager.Instance.gameRunning)
        {
            scoreLabel.text = GameManager.Instance.playerScore.ToString();
        }
    }

    private void Start()
    {
        screenFlashCanvas = screenFlash.GetComponent<CanvasGroup>();
        levelNameText.text = "";
        scoreLabel.text = "0";
        gemsCollectedThisRunLabel.text = "0";
        gemNormalSprite = gemIcon.sprite;
        gemIconFill.fillAmount = 0;

    }

    // Button callback
    public void EnableMobileUI()
    {
        mobileUI.alpha = 1;
        mobileUI.interactable = true;
        mobileUI.blocksRaycasts = true;
    }

    // Button callback
    public void DisableMobileUI()
    {
        mobileUI.alpha = 0;
        mobileUI.interactable = false;
        mobileUI.blocksRaycasts = false;
    }

    private Sequence currentMessageSequence;
    public void ShowMessage(string message, float fadeInDuration, float displayDuration, float fadeOutDuration, bool blink = false)
    {
        messageText.text = "";
        messageBlinkingText.blink = blink;
        if (!blink) messageText.color = Color.white;

        if (currentMessageSequence != null)
        {
            // If a message is already displaying, interrupt it and fade it out immediately
            currentMessageSequence.Kill();
            messageCanvasGroup.DOFade(0, 0.05f).OnComplete(() => DisplayNewMessage(message, fadeInDuration, displayDuration, fadeOutDuration));
        }
        else
        {
            DisplayNewMessage(message, fadeInDuration, displayDuration, fadeOutDuration);
        }
    }

    private void DisplayNewMessage(string message, float fadeInDuration, float displayDuration, float fadeOutDuration)
    {
        messageText.text = message;
        currentMessageSequence = DOTween.Sequence()
            .Append(messageCanvasGroup.DOFade(1, fadeInDuration).SetEase(Ease.OutQuint))
            .AppendInterval(displayDuration)
            .Append(messageCanvasGroup.DOFade(0, fadeOutDuration).SetEase(Ease.InQuint))
            .OnComplete(() => currentMessageSequence = null);
    }

    public void UpdatePlayerLevel()
    {
        playerLevelLabel.text = PlayerData.Instance.Data.PlayerLevel.ToString();
    }

    public void UpdateDepth(float depth)
    {
        float roundedDepth = Mathf.Round(depth);
        depthLabel.text = $"{roundedDepth}";
    }

    public void UpdateGemMultiplier()
    {
        var value = GemController.Instance.totalGemMultiplier;

        gemMultiplierIntegerLabel.text = ((int)Mathf.Floor(value)).ToString();
        gemMultiplierRemainderLabel.text = value % 1 > 0 ? (value % 1).ToString().Substring(2) : "00";
    }

    public void ScreenFlash()
    {
        StartCoroutine(StartScreenFlash());
    }

    IEnumerator StartScreenFlash()
    {
        screenFlashCanvas.alpha = 1;
        yield return new WaitForSeconds(0.1f);
        screenFlashCanvas.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        screenFlashCanvas.alpha = 0;
    }

    public void PopGemText()
    {
        StartCoroutine(TextPop(gemsCollectedThisRunLabel));
        //StartCoroutine(TextPop(totalGemsOnAccountLabel));
    }

    public IEnumerator TextPop(TextMeshProUGUI text)
    {
        var normalScale = new Vector3(1, 1, 1);
        text.rectTransform.localScale = normalScale;

        var scale = new Vector3(1.35f, 1.35f, 1);
        text.rectTransform.DOScale(scale, 0.02f).SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(0.02f);

        text.rectTransform.DOScale(normalScale, 0.2f).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator GemIconFlashOnce()
    {
        if (PlayerManager.Instance.isBoosting) yield break;

        gemAnim.SetBool("flash", true);
        yield return new WaitForSeconds(0.02f);
        gemAnim.SetBool("flash", false);
    }

    public void EnableGemIconEffects()
    {
        gemIcon.sprite = gemNormalSprite;
        gemAnim.SetBool("superFlash", true);
        gemIconFill.fillAmount = 0;
        gemIconFill.enabled = false;
    }

    public void DisableGemIconEffects()
    {
        gemAnim.SetBool("superFlash", false);
        gemIcon.sprite = gemNormalSprite;
        gemIconFill.enabled = true;
    }

    //public void CreateFloatingText(string text, Color color, bool showGemIcon = false)
    //{
    //    var position = PlayerManager.Instance.player.transform.position + new Vector3(floatingTextOffsetX, floatingTextOffsetY, 0);
    //    var FloatingText = Instantiate(FloatingTextPrefab, position, Quaternion.identity, worldUI.transform);
    //    var floatingText = FloatingText.GetComponent<FloatingText>();
    //    floatingText.SetProperties(text, color, showGemIcon);
    //    FloatingText.transform.DOMoveY(position.y + floatingTextMoveAmount, 0.5f).SetEase(Ease.OutQuint);
    //}

    public IEnumerator ChangeLevelName(string levelName)
    {
        var newLevelName = LevelController.Instance.levelLoopNumber > 1 ? $"{levelName} {LevelController.Instance.levelLoopNumber}" : levelName;

        levelNameText.DOFade(0, 0.5f);
        yield return new WaitForSeconds(1.5f);
        levelNameText.text = newLevelName;
        levelNameText.DOFade(1, 1f);
    }

    //public void UpdateGemIconFill()
    //{
    //    gemIconFill.fillAmount = Utils.CalculateSliderValue(SkillController.Instance.currentGemsTowardSkill, SkillController.Instance.CurrentSkill.Data.gemPointQuota);
    //}

    public void ShowGemsChangedUIElement()
    {
        gemsChangedUIElement.SetProperties("", ColorPalette.Instance.colors[4], true);
        gemsChangedUIElement.TriggerElement();
    }

    //public void ShowGemsChangedUIElement(string text)
    //{
    //    gemsChangedUIElement.SetProperties(text, ColorPalette.Instance.colors[4], true);
    //    gemsChangedUIElement.TriggerElement();
    //}

    //public void ShowLivesChangedUIElement(string text)
    //{
    //    livesChangedUIElement.SetProperties(text, ColorPalette.Instance.colors[4], true);
    //    livesChangedUIElement.TriggerElement();
    //}



}
