using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpritePulse : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    public bool startOnAwake = false;
    public Color ColorA = Color.white, ColorB = Color.black;
    Material material;
    public Ease easing = Ease.InSine;
    public float transitionDuration = 2f;

    Tween pulseTween;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        material.color = ColorA;

        if (startOnAwake)
        {
            StartPulse();
        }
    }

    public void StartPulse()
    {
        pulseTween = material.DOColor(ColorB, transitionDuration).SetEase(easing).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopPulse()
    {
        if (pulseTween != null)
        {
            pulseTween.Kill();
        }

    }


}
