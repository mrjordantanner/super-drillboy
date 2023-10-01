using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using static Cinemachine.DocumentationSortingAttribute;


public class HUD : MonoBehaviour
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
        #endregion
    }

    [Header("Debug Labels")]
    public TextMeshProUGUI velocityLabel;
    public TextMeshProUGUI depthLabel;
    public TextMeshProUGUI coinsLabel;
    public TextMeshProUGUI livesRemaining;

    public TextMeshProUGUI gameClock;

    public CanvasGroup canvasGroup;
    public CanvasGroup worldUI;
    public ScreenFader screenFader;
    public GameObject screenFlash;

    public TextMeshProUGUI checkpointsReached;

    [Header("Misc")]
    public CanvasGroup messageCanvasGroup;
    public TextMeshProUGUI messageText;
    public BlinkingText messageBlinkingText;

    [Header("Cursors")]
    public bool useCustomCursor = true;
    public Texture2D customCursor;

    [Header("Score")]
    public TextMeshProUGUI playerScoreText;


    private void Start()
    {
        if (useCustomCursor)
        {
            Cursor.SetCursor(customCursor, new Vector2(0.5f, 0.5f), CursorMode.Auto);
        }


        livesRemaining.text = PlayerManager.Instance.lives.ToString();
    }

    IEnumerator messageRoutine;
    public void ShowMessage(string text, float duration)
    {
        var upperText = text.ToUpper();
        messageRoutine = Message(upperText, duration);
        StartCoroutine(messageRoutine);
    }

    bool messageShowing;
    Tween subFadeIn, subFadeOut;

    public IEnumerator Message(string text, float duration, bool blink = false)
    {
        messageText.text = "";

        if (messageShowing)
        {
            //StopCoroutine(subMessageRoutine);
            subFadeIn.Kill();
            subFadeOut.Kill();
            messageCanvasGroup.DOFade(0, 0f);
        }

        messageShowing = true;
        messageText.text = text.ToUpper();

        if (blink)
        {
            messageBlinkingText.blink = true;
        }
        else
        {
            messageBlinkingText.blink = false;
            messageText.color = Color.white;
        }

        var fadeInTime = 0.08f;
        var fadeOutTime = 1.5f;

        subFadeIn = messageCanvasGroup.DOFade(1, fadeInTime).SetEase(Ease.Linear);
        yield return new WaitForSecondsRealtime(fadeInTime);

        yield return new WaitForSecondsRealtime(duration - fadeInTime - fadeOutTime);

        subFadeOut = messageCanvasGroup.DOFade(0, fadeOutTime).SetEase(Ease.InSine);
        yield return new WaitForSecondsRealtime(fadeOutTime);

        messageText.text = "";
        messageBlinkingText.blink = false;
        messageShowing = false;
    }

    public void UpdateVelocity(float velocity)
    {
        var absVel = Mathf.Abs(velocity);
        float roundedVel = Mathf.Round(absVel);
        velocityLabel.text = roundedVel.ToString();
    }

    public void UpdateDepth(float depth)
    {
        float roundedDepth = Mathf.Round(depth);
        depthLabel.text = roundedDepth.ToString();
    }

    public void ScreenFlash()
    {
        StartCoroutine(StartScreenFlash());
    }

    IEnumerator StartScreenFlash()
    {
        screenFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        screenFlash.SetActive(false);

    }

}


