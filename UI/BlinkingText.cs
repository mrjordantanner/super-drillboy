using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BlinkingText : MonoBehaviour
{
    public bool blink = false;
    public Color color1, color2;
    public float blinkRate;
    float timer;
    TextMeshProUGUI[] text;
    bool color1enabled;

    public bool setStartColor = true;

    void Start()
    {
        text = GetComponentsInChildren<TextMeshProUGUI>();
        if (setStartColor)
        {
            SetTextColor(color1);
            color1enabled = true;
        }
        timer = blinkRate;
    }

    void SetTextColor(Color color)
    {
        foreach(var t in text)
        {
            t.color = color;
        }
    }

    void Update()
    {
        if (blink)
        {
            timer -= Time.unscaledDeltaTime;

            if (timer <= 0)
            {
                if (color1enabled)
                {
                    SetTextColor(color2);
                    color1enabled = false;
                }
                else
                {
                    SetTextColor(color1);
                    color1enabled = true;
                }

                timer = blinkRate;
            }
        }
    }
}
