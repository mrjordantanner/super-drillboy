using UnityEngine;

/// <summary>
/// Stores info for a specific decorative object.  An array of these is contained in each LevelTheme.
/// </summary>
[CreateAssetMenu(menuName = "Decor/Decorative Object")]
public class DecorativeObjectData : ScriptableObject
{
    public GameObject prefab;
    public float spawnRateByDepth = 100f;    // object will spawn after player travels this distance in depth    
    [Range(-7, 7)]
    public float offsetX;
    public float offsetY;
    [ReadOnly]
    public float lastSpawnDepth;   // cached value of the depth the player was when the object spawned


}
