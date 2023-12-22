using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Player Data")]
public class LocalPlayerData : ScriptableObject
{
    [ReadOnly] public string PlayerName;
    [ReadOnly] public int XP;
    [ReadOnly] public int TotalXP;
    [ReadOnly] public int PlayerLevel;
    [ReadOnly] public int PlayerAvatarIndex;   // Could possibly use name of image instead of index

    [Header("Default Values")]
    public string defaultPlayerName = "Player";

    public async Task ResetToDefaults()
    {
        PlayerName = defaultPlayerName;
        XP = TotalXP = 0;
        PlayerLevel = 1;
        PlayerAvatarIndex = 0;

        StatsBar.Instance.Refresh();
        await PlayerData.Instance.SaveAllAsync();
    }

}
