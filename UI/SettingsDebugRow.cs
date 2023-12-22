using TMPro;
using UnityEngine;

public class SettingsDebugRow : MonoBehaviour
{
    public Setting Setting;
    public TextMeshProUGUI settingName, settingValue, settingSaved, settingDefault;

    public void UpdateRow()
    {
        settingName.text = Setting.name;
        settingValue.text = Setting.Value.ToString("F2");
        settingSaved.text = Setting.PlayerPrefsValue.ToString("F2");
        settingDefault.text = Setting.Default.ToString("F2");
    }
}

