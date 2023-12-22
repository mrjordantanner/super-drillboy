using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ScorePanel : MenuPanel
{
    public GameObject DoneButton;
    public TextMeshProUGUI
        panelHeader,
        maxDepthReachedLabel,
        gemsCollectedLabel,
        cheatsWereUsedLabel,
        totalScoreLabel;

    public float tallyUpdateInterval = 0.01f;
    int tallyUpdateIncrement = 50;

    int maximumDepthReachedValue;
    int totalGemsCollected;
    int totalScore;
    int depthPoints;
    int gemPoints;

    bool tallyDepth;
    bool tallyGems;
    bool depthTallyCompleted, gemsTallyCompleted, sequenceCompleted;

    float timer;
    bool tallyTimerEnabled;

    private void Start()
    {
        panelHeader.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (sequenceCompleted) return;

        if (Utils.ClickOrTap())
        {
            SkipCurrentTally();
        }

        if (tallyTimerEnabled)
        {
            timer += Time.unscaledDeltaTime;
        }

        if (tallyTimerEnabled && timer >= tallyUpdateInterval)
        {
            timer = 0;

            // Tally depth points
            if (tallyDepth && maximumDepthReachedValue > 0)
            {
                var value = Mathf.Min(tallyUpdateIncrement, maximumDepthReachedValue);

                depthPoints += value;
                totalScore += value;
                maximumDepthReachedValue -= value;

                AudioManager.Instance.soundBank.CollectSmallGem.Play();
                UpdateLabels();
            }
            else if (tallyDepth && maximumDepthReachedValue == 0)
            {
                StopDepthTally();
                UpdateLabels();
                StartCoroutine(StartGemsTally());

                AudioManager.Instance.soundBank.CollectSmallGem.Play();
            }

            // Tally gem points
            if (tallyGems && totalGemsCollected > 0)
            {
                var value = Mathf.Min(tallyUpdateIncrement, totalGemsCollected);

                gemPoints += value;
                totalScore += value;
                totalGemsCollected -= value;

                AudioManager.Instance.soundBank.CollectSmallGem.Play();
                UpdateLabels();
            }
            else if (tallyGems && totalGemsCollected == 0)
            {
                StopGemsTally();
                UpdateLabels();
                CompleteSequence();
            }
        }

    }

    // Show Panel header
    // short delay
    // Tally up points for depth
    // Tally up points for gems
    // Display total score and show DONE button
    IEnumerator StartTallySequence()
    {
        sequenceCompleted = false;
        gemsTallyCompleted = false;
        depthTallyCompleted = false;
        yield return new WaitForSecondsRealtime(1f);

        panelHeader.gameObject.SetActive(true);
        StartCoroutine(StartDepthTally());
    }

    void SkipCurrentTally()
    {
        if (tallyDepth && !depthTallyCompleted)
        {
            StopDepthTally();
            UpdateLabels();
            StartCoroutine(StartGemsTally());
        }
        else if (tallyGems && !gemsTallyCompleted)
        {
            StopGemsTally();
            UpdateLabels();
            CompleteSequence();
        }
    }

    IEnumerator StartDepthTally()
    {
        yield return new WaitForSeconds(1f);
        tallyTimerEnabled = true;
        tallyDepth = true;
        depthTallyCompleted = false;
    }

    void StopDepthTally()
    {
        tallyTimerEnabled = false;
        tallyDepth = false;
        depthTallyCompleted = true;

        depthPoints = (int)PlayerManager.Instance.maxDepthReached;
        totalScore = depthPoints;
        maximumDepthReachedValue = 0;

        AudioManager.Instance.soundBank.CollectSmallGem.Play();
    }

    IEnumerator StartGemsTally()
    {
        yield return new WaitForSeconds(0.5f);
        tallyTimerEnabled = true;
        tallyGems = true;
        gemsTallyCompleted = false;
    }

    void StopGemsTally()
    {
        tallyTimerEnabled = false;
        tallyGems = false;
        gemsTallyCompleted = true;

        gemPoints = (int)GemController.Instance.gemsCollectedThisRun;
        totalScore = gemPoints + (int)PlayerManager.Instance.maxDepthReached;
        totalGemsCollected = 0;

        AudioManager.Instance.soundBank.CollectSmallGem.Play();
    }

    void CompleteSequence()
    {
        sequenceCompleted = true;
        tallyTimerEnabled = false;
        DoneButton.SetActive(true);
        AudioManager.Instance.soundBank.CollectSmallGem.Play();

        int actualTotalScore = (int)(PlayerManager.Instance.maxDepthReached + GemController.Instance.gemsCollectedThisRun);

        // Check that calculation is valid
        if (totalScore != actualTotalScore)
        {
            Debug.LogError($"SCORE CALCULATION ERROR:  CalculatedScore: {totalScore} -- ActualScore: {actualTotalScore}");
            totalScore = actualTotalScore;
        }

        UpdateLabels();
        Currency.Instance.SyncLocalDataWithCloud();
        PlayerData.Instance.SaveAllAsync();
     }

    void UpdateLabels()
    {
        maxDepthReachedLabel.text = maximumDepthReachedValue.ToString();
        gemsCollectedLabel.text = totalGemsCollected.ToString();
        totalScoreLabel.text = totalScore.ToString();
    }

    // Menu button callback
    public void ShowScorePanel()
    {
        cheatsWereUsedLabel.text = DevTools.Instance.devToolsWereUsed ? "Dev Tools were used\r\n\r\nNot eligible for leaderboard" : "";
        cheatsWereUsedLabel.text = cheatsWereUsedLabel.text.ToUpper();

        Show();

        maximumDepthReachedValue = (int)PlayerManager.Instance.maxDepthReached;
        totalGemsCollected = (int)GemController.Instance.gemsCollectedThisRun;
        UpdateLabels();

        StartCoroutine(StartTallySequence());
    }

}
