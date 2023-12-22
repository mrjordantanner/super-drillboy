using UnityEngine;

public class ChangeCameraColor : MonoBehaviour
{
    public Color[] targetColors;
    public float fadeDuration = 10.0f; 

    private Camera mainCamera;
    private int currentIndex = 0;
    private Color currentColor;
    private Color nextColor;
    private float startTime;

    private void Start()
    {
        mainCamera = Camera.main;

        if (targetColors.Length > 0)
        {
            currentIndex = 0;
            currentColor = targetColors[currentIndex];
            mainCamera.backgroundColor = currentColor;
            nextColor = targetColors[(currentIndex + 1) % targetColors.Length];
            startTime = Time.time;
        }
    }

    private void Update()
    {
        if (targetColors.Length < 2)
        {
            return;
        }

        float t = (Time.time - startTime) / fadeDuration;

        if (t >= 1f)
        {
            currentIndex = (currentIndex + 1) % targetColors.Length;
            currentColor = targetColors[currentIndex];
            nextColor = targetColors[(currentIndex + 1) % targetColors.Length];
            startTime = Time.time;
            t = 0f;
        }

        mainCamera.backgroundColor = Color.Lerp(currentColor, nextColor, t);
    }
}
