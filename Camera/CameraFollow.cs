using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    #region Singleton
    public static CameraFollow Instance;
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

    public Transform Target;
    public float smoothTime = 0.3f;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.15f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 originalPosition;

    bool shouldShake;

    void LateUpdate()
    {
        if (Target != null)
        {
            Vector3 targetPosition = new(Target.position.x, Target.position.y, -10);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }

        if (shouldShake && !GameManager.Instance.gamePaused)
        {
            transform.position = originalPosition + Random.insideUnitSphere * shakeIntensity;
        }
    }

    public void ShakeCamera()
    {
        StartCoroutine(CameraShake());
    }

    IEnumerator CameraShake()
    {
        shakeIntensity = 0.1f;
        shakeDuration = 0.2f;

        originalPosition = transform.position;
        shouldShake = true;
        yield return new WaitForSeconds(shakeDuration);

        transform.position = originalPosition;
        shouldShake = false;
    }
}
