using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineJuice : MonoBehaviour
{
    public bool isActive;
    public SpriteRenderer spriteRenderer;

    [Header("Randomization")]
    public bool randomStartDelay = true;
    public bool randomDirection = true;
    public float maxStartDelay = 1f;

    [Header("Motion")]
    public bool sineMotion;
    public float motionFrequency = 0f;
    public float motionX = 0f;
    public float motionY = 0f;
    public float motionZ = 0f;
    float motionTimeCounter = 0f;

    [Header("Sine 2.0")]
    public float amplitude = 1f;     // The amplitude of the sine wave
    public float frequency = 1f;     // The frequency of the sine wave
    public float speed = 1f;         // The speed of the movement

    [HideInInspector]
    public float initialPositionY;  // The initial X position of the object


    [Header("Scale")]
    public bool sineScale;
    public float scaleFrequency;
    public float scaleAmount;
    float scaleTimeCounter = 0f;
    float startDelayTimer;

    [Header("Fade")]
    public bool sineFade;
    public float fadeFrequency;
    public float fadeAmount;
    float fadeTimeCounter = 0f;

    [Header("Alpha Pulse")]
    public bool pulse;
    public float pulseFrequency;
    public float pulseAmount;
    float pulseTimeCounter = 0f;

    Rigidbody2D rb;

    private void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        Init();
    }

    public void Init()
    {
        if (randomDirection)
        {
            var randomX = Random.value;
            if (randomX <= .50f)
            {
                motionX = -motionX;
            }

            var randomY = Random.value;
            if (randomY <= .50f)
            {
                motionY = -motionY;
            }

            var randomZ = Random.value;
            if (randomZ <= .50f)
            {
                motionZ = -motionZ;
            }
        }

        if (randomStartDelay)
        {
            startDelayTimer = Random.Range(0, maxStartDelay);
            isActive = false;
        }
        else
        {
            isActive = true;
        }

        initialPositionY = transform.localPosition.y;
    }

    void Update()
    {
        if (!isActive)
        {
            startDelayTimer -= Time.deltaTime;
            if (startDelayTimer <= 0)
            {
                isActive = true;
            }
        }

        if (isActive)
        {

            if (sineScale)
            {
                scaleTimeCounter += Time.deltaTime * scaleFrequency;
                transform.localScale = new Vector2(Mathf.Sin(scaleTimeCounter) * scaleAmount, Mathf.Sin(scaleTimeCounter) * scaleAmount);
            }

            if (sineMotion)
            {
                //motionTimeCounter += Time.deltaTime * motionFrequency;
                ////transform.position += new Vector3(Mathf.Cos(motionTimeCounter) * motionX, Mathf.Sin(motionTimeCounter) * motionY, Mathf.Sin(motionTimeCounter) * motionZ);
                //transform.localPosition += new Vector3(Mathf.Sin(motionTimeCounter) * motionX, Mathf.Sin(motionTimeCounter) * motionY, Mathf.Sin(motionTimeCounter) * motionZ);

                //Calculate the new X position using a sine wave
                float newY = initialPositionY + Mathf.Sin(Time.time * frequency) * amplitude;

                // Calculate the translation vector based on the new X position
                Vector3 translation = new(0f, newY - transform.localPosition.y, 0f);

                // Apply the translation to the object's local position
                transform.Translate(speed * Time.deltaTime * translation, Space.Self);





            }

            if (sineFade && spriteRenderer != null)
            {
                fadeTimeCounter += Time.deltaTime * fadeFrequency;
                float alpha = spriteRenderer.material.color.a;
                alpha = Mathf.Sin(fadeTimeCounter) * fadeAmount;

            }

            if (pulse && spriteRenderer != null)
            {
                pulseTimeCounter += Time.deltaTime * pulseFrequency;
                Color newColor = new(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Sin(pulseTimeCounter) * pulseAmount);
                spriteRenderer.color = newColor;


            }
        }
    }


}

