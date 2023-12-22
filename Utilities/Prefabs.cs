using UnityEngine;
using UnityEngine;
using UnityEngine;


public class Prefabs : MonoBehaviour
{
    #region Singleton
    public static Prefabs Instance;
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



}
