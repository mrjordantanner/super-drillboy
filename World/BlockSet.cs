using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "Blocks/BlockSet")]
public class BlockSet : ScriptableObject
{
    public Sprite[] variants;
    public Sprite[] spriteExtensions;

    public bool favorPrimaryVariant;
    public float variantSpawnChance = 0.2f;
    public float extensionSpawnChance = 0.2f;

    public GameObject DestroyVFX;
    public SoundEffect destroySound;

    [Header("Preview")]
    [Range(1, 6)]
    public int previewRowLength = 4;
    [Range(30, 90)]
    public int previewSize = 60;

    public Sprite GetVariant()
    {
        // Use first variant by default
        var variant = variants[0];

        // Include or exclude primaryVariant when randomizing
        var startingIndex = favorPrimaryVariant ? 1 : 0;
        var spawnChance = favorPrimaryVariant ? variantSpawnChance : 1;

        if (variants.Length > 1)
        {
            if (Random.value <= spawnChance)
            {
                var index = Random.Range(startingIndex, variants.Length);
                variant = variants[index];
            }
        }

        return variant;
    }

    public Sprite RollForSpriteExtension()
    {
        Sprite extension = null;

        if (spriteExtensions.Length == 0 || spriteExtensions == null)
        {
            return null;
        }

        if (Random.value <= extensionSpawnChance)
        {
            var index = Random.Range(0, spriteExtensions.Length);
            extension = spriteExtensions[index];
        }

        return extension;
    }
}
