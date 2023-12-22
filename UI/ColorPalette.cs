using UnityEngine;


public class ColorPalette : MonoBehaviour
{
    #region Singleton
    public static ColorPalette Instance;
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
        #endregion
    }

    public Color[] colors;
}
