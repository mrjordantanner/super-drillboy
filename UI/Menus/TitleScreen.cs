using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TitleScreen : MenuPanel
{
    public IntroSequence introSequence;

    private void Start()
    {
        if (GameManager.Instance.testMode != TestMode.Normal) return;

        Show();
        introSequence.gameObject.SetActive(true);
        StartCoroutine(introSequence.BeginSequence());
    }

    private void Update()
    {
        if (!isShowing) return;

        if (Utils.ClickOrTap())
        {
            if (introSequence.isSequencePlaying)
            {
                introSequence.InterruptSequence();
            }
            else
            {
                // TODO play other sound here?
                Menu.Instance.PlayClickSound();
                StartCoroutine(Menu.Instance.MainMenuTransitionFromTitle());
                introSequence.tapToStartLabel.SetActive(false);
            }
        }
    }



}
