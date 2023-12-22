using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;
using System.Collections;

public class Drops : MonoBehaviour
{
    #region Singleton
    public static Drops Instance;
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

        Init();
    }

    [ReadOnly] public float smallGemDropChance = 0.18f;
    [ReadOnly] public float largeGemDropChance = 0.03f;
    [ReadOnly] public float armorDropChance = 0.003f;
    //[ReadOnly] public float extraLifeDropChance = 0.0003f;

    public GameObject smallGemPrefab;
    public GameObject largeGemPrefab, armorPickupPrefab;//, extraLifePickupPrefab;

    [Header("Stats")]
    [ReadOnly]
    public int largeGemsDropped;
    [ReadOnly]
    public int smallGemsDropped,
        armorPickupsDropped;
        //extraLifePickupsDropped;

    public void Init()
    {
        largeGemsDropped = smallGemsDropped = armorPickupsDropped = 0;
    }

    public void RollForDrops(Block block)
    {
        MapGenerator.Instance.blocksDestroyed++;
        var container = MapGenerator.Instance.ObjectContainer.transform;

        // Chance to drop armor
        var armorVal = Random.value;
        if (armorVal <= armorDropChance)
        {
            var HealthPickup = Instantiate(armorPickupPrefab, block.transform.position, Quaternion.identity, container);
            HealthPickup.GetComponent<Pickup>().wasDropped = true;
            armorPickupsDropped++;
        }

        // Chance to drop either large or small gem
        var val = Random.value;
        if (val <= largeGemDropChance)
        {
            var LargeGem = Instantiate(largeGemPrefab, block.transform.position, Quaternion.identity, container);
            LargeGem.GetComponent<Pickup>().wasDropped = true;
            largeGemsDropped++;
            return;
        }

        var val2 = Random.value;
        if (val2 <= smallGemDropChance)
        {
            var SmallGem = Instantiate(smallGemPrefab, block.transform.position, Quaternion.identity, container);
            SmallGem.GetComponent<Pickup>().wasDropped = true;
            smallGemsDropped++;
        }
    }

}
