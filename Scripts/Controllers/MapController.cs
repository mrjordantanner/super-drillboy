using Unity.Burst.CompilerServices;
using UnityEngine;


public class MapController : MonoBehaviour
{
    #region Singleton
    public static MapController Instance;
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

    public MapSection startingSection;
    public int segmentLength = 5;

    [HideInInspector]
    public Transform currentLastNode;

    [Header("Prefabs")]
    public GameObject CheckpointPrefab;
    public GameObject WallBlockPrefab;
    public GameObject CoinPrefab;


    [Header("Blocks")]
    public float blockCoinDropChance = 0.15f;
    public float blockRegenerationTime = 3f;
    [Range(0, -4)]
    public int wallOffsetX = 0;

    int totalSegmentsCreated;

    GameObject[] MapSectionBank;
    MapSection[] Sections;
    Checkpoint[] Checkpoints;

    public void Init()
    {
        MapSectionBank = Resources.LoadAll<GameObject>("MapSections");
        currentLastNode = startingSection.bottomNode;

        GenerateNewSegment();
    }

    public void GenerateNewSegment()
    {
        // Create new checkpoint for this Segment
        var NewCheckpoint = Instantiate(CheckpointPrefab, currentLastNode.transform.position, Quaternion.identity);
        var checkpoint = NewCheckpoint.GetComponent<Checkpoint>();
        checkpoint.segmentID = totalSegmentsCreated;

        // Generate segmentLength new MapSections
        for (int i = 0; i < segmentLength; i++)
        {
            var NewSection = Instantiate(GetRandomMapSection(), currentLastNode.transform.position, Quaternion.identity);
            var mapSection = NewSection.GetComponent<MapSection>();
            mapSection.segmentID = totalSegmentsCreated;

            Vector3 newPosition = currentLastNode.parent.position + (currentLastNode.parent.localScale.y + NewSection.transform.localScale.y) * 0.5f * Vector3.down;
            NewSection.transform.position = newPosition;

            currentLastNode = mapSection.bottomNode;

            GenerateWallBlocks(mapSection);
        }    

        totalSegmentsCreated++;

    }

    void GenerateWallBlocks(MapSection mapSection)
    {
        var offsetX = new Vector3(0.5f, 0, 0);
        var offsetY = new Vector3(0, 0.5f, 0);

        Vector3 wallOffset = new(wallOffsetX, 0, 0);

        var objectToMatch = mapSection.transform;
        var objectScale = objectToMatch.localScale;
        float objectLength = objectScale.y;
        float objectWidth = objectScale.x;
        float halfObjectLength = objectLength * 0.5f;

        float blockWidth = WallBlockPrefab.transform.localScale.y;
        float totalBlockLength = objectLength;
        int numberOfBlocks = Mathf.FloorToInt(totalBlockLength / (blockWidth));

        var startPosition = (objectToMatch.position - Vector3.down * halfObjectLength) - offsetY;

        // Left Wall
       var leftStartPosition = startPosition - (0.5f * objectWidth * Vector3.right) - offsetX - wallOffset;

        for (int i = 0; i < numberOfBlocks; i++)
        {
            var blockPosition = leftStartPosition + Vector3.down * (i * (blockWidth));
            var Block = Instantiate(WallBlockPrefab, blockPosition, Quaternion.identity);
            Block.GetComponent<WallBlock>().segmentID = mapSection.segmentID;
        }

        // Right Wall
        var rightStartPosition = startPosition + (0.5f * objectWidth * Vector3.right) + offsetX + wallOffset;

        for (int i = 0; i < numberOfBlocks; i++)
        {
            var blockPosition = rightStartPosition + Vector3.down * (i * (blockWidth));
            var Block = Instantiate(WallBlockPrefab, blockPosition, Quaternion.identity);
            Block.GetComponent<WallBlock>().segmentID = mapSection.segmentID;
        }
    }

    GameObject GetRandomMapSection()
    {
        int randomIndex = UnityEngine.Random.Range(0, MapSectionBank.Length);
        return MapSectionBank[randomIndex];
    }

    public void OnCheckpointReached(int segmentID)
    {
        Sections = FindObjectsOfType<MapSection>();
        Checkpoints = FindObjectsOfType<Checkpoint>();

        GenerateNewSegment();
        DestroyPreviousSegment(segmentID);
    }

    public void DestroyPreviousSegment(int segmentID)
    {
        if (segmentID < 2) return;
        var segmentIDToDestroy = segmentID - 2;

        foreach (var section in Sections)
        {
            if (section.segmentID == segmentIDToDestroy)
            {
                Destroy(section.gameObject);
            }
        }

        foreach (var checkpoint in Checkpoints)
        {
            if (checkpoint.segmentID == segmentIDToDestroy)
            {
                Destroy(checkpoint.gameObject);
            }
        }

        var wallBlocks = FindObjectsOfType<WallBlock>();
        foreach (var block in wallBlocks)
        {
            if (block.segmentID == segmentIDToDestroy)
            {
                Destroy(block.gameObject);
            }
        }
    }

    public void ReduceWallWidth()
    {
        if (wallOffsetX == -4) return;
        wallOffsetX--;
    }
}

