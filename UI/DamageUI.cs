using UnityEngine;

public class DamageUI : MonoBehaviour
{
    #region Singleton
    public static DamageUI Instance;
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

    [Header("Health UI")]
    public Canvas worldSpaceCanvas;
    public GameObject HealthBarPrefab;
    public Vector2 healthBarOffset = new(0, 1f);
    public GameObject StaminaBarPrefab;
}

