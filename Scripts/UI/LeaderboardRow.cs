using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LeaderboardRow : MonoBehaviour
{
    public TextMeshProUGUI rankText, userNameText, scoreText, killsText;
    
    [HideInInspector]
    public BlinkingText blinkingText;

    private void Awake()
    {
        blinkingText = GetComponent<BlinkingText>();
    }
}
