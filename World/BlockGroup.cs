using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class BlockGroup : MonoBehaviour, ISegmentIdentifier
{
    public float destructibleBlockSpawnChance = 0.5f;
    public BlockSpawnPoint[] spawnPoints;
    public GameObject Entity { get { return gameObject; } }
    public int SegmentId { get; set; }

    private void Awake()
    {
        spawnPoints = GetComponentsInChildren<BlockSpawnPoint>();
    }

    private void Start()
    {
        StartCoroutine(SpawnBlocks());
    }

    public IEnumerator SpawnBlocks()
    {
        yield return new WaitForSeconds(0.2f);

        var existingBlocks = GetComponentsInChildren<Block>();
        if (existingBlocks.Any())
        {
            foreach (var block in existingBlocks)
            {
                block.ApplyBlockSetByType();
            }
        }

        foreach (var spawnPoint in spawnPoints)
        {
            var position = spawnPoint.transform.position;
            spawnPoint.boxCollider.enabled = false;

            if (Random.value <= destructibleBlockSpawnChance)
            {
                var NewBlock = Instantiate(MapGenerator.Instance.DestBlockPrefab, position, Quaternion.identity, transform);
                var block = NewBlock.GetComponent<Block>();
                block.ApplyBlockSet(LevelController.Instance.selectedLevel.block_destructible);

                MapGenerator.Instance.destBlocksSpawned++;
            }
        }

        foreach (var spawnPoint in spawnPoints)
        {
            Destroy(spawnPoint.gameObject);
        }
    }
}
