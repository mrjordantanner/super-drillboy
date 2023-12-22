using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Setting")]
public class Setting : ScriptableObject
{
    public float Default;
    [ReadOnly]
    public float _value;
    [ReadOnly]
    public float PlayerPrefsValue;
    public bool allowValueToBeZero;

    public void SetToDefault()
    {
        _value = Default;
    }

    public float Value
    {
        get
        {
            if (!allowValueToBeZero && _value == 0)
            {
                _value = Default;
            }
        
            return _value;
        }
        set
        {
            _value = value;
        }
    }

    public void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetFloat(name, Value);
        PlayerPrefsValue = Value;
        PlayerPrefs.Save();
    }

    public void LoadFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(name))
        {
            Value = PlayerPrefs.GetFloat(name);
            PlayerPrefsValue = Value;
            return;
        }

        Value = Default;
    }



}