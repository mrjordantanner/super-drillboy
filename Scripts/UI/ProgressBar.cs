using UnityEngine;
using UnityEngine.UI;


public class ProgressBar : MonoBehaviour
{
    [HideInInspector]
    public CanvasGroup canvasGroup;
    public Slider slider;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    [ReadOnly]
    public bool onCooldown;
    [ReadOnly]
    public float cooldownProgressTime;
    [ReadOnly]
    public float cooldown;


    private void Update()
    {
        if (onCooldown)
        {
            cooldownProgressTime += Time.deltaTime;

            float progress = cooldownProgressTime / cooldown;

            slider.value = progress;

            if (cooldownProgressTime >= cooldown)
            {
                cooldownProgressTime = 0f;
                onCooldown = false;
                slider.value = slider.maxValue;
            }
        }
    }

    public void StartCooldown()
    {
        slider.value = 0;
        onCooldown = true;
    }
}
