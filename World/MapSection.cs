using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MapSection : MonoBehaviour//, ISegmentIdentifier
{
    [ReadOnly]
    public int numberOfTimesUsed;
    bool hasBeenInitialized;

    public Transform topNode, bottomNode;

    //public GameObject Entity { get { return gameObject; } }
    public int SegmentId;// { get; set; }
    public bool isFirstInSegment;

    //public Block.BlockType[,] grid;
    //int gridWidth, gridHeight;

    [HideInInspector]
    public Block[] blocks;
    [HideInInspector]
    public Pickup[] pickups;

    GameObject leftOutOfBoundsArea, rightOutOfBoundsArea;

    Vector3 blockOffsetX = new(0.5f, 0, 0);
    Vector3 blockOffsetY = new(0, 0.5f, 0);
    Vector3 wallOffset = new();

    int numberOfBlocks;
    float mapSectionHeight, 
        mapSectionWidth, 
        halfMapSectionHeight, 
        halfMapSectionWidth;

    void Awake()
    {
        blocks = GetComponentsInChildren<Block>();
        foreach (var block in blocks)
        {
            block.parentMapSection = this;
        }
        pickups = GetComponentsInChildren<Pickup>();
        foreach (var pickup in pickups)
        {
            pickup.parentMapSection = this;
        }

        // gridHeight = (int)transform.localScale.y;

        wallOffset = new(MapGenerator.Instance.GetWallOffset(), 0, 0);

        mapSectionHeight = transform.localScale.y;
        mapSectionWidth = transform.localScale.x;
        halfMapSectionHeight = mapSectionHeight * 0.5f;
        halfMapSectionWidth = mapSectionWidth * 0.5f;
    }

    private void OnEnable()
    {
        EnableMapSection();
    }

    void EnableMapSection()
    {
        if (hasBeenInitialized)
        {
            if (numberOfTimesUsed > 0)
            {
                foreach (var block in blocks)
                {
                    if (block.isDestroyed)
                    {
                        block.EnableBlock();
                    }
                }
            }

            numberOfTimesUsed++;
        }

        hasBeenInitialized = true;

        foreach (var pickup in pickups)
        {
            pickup.EnablePickup();
        }
    }

    public void DisableMapSection()
    {
        Destroy(leftOutOfBoundsArea);
        Destroy(rightOutOfBoundsArea);
        gameObject.SetActive(false);
    }

    public void ApplyLevelTheme()
    {
        if (blocks.Length > 0)
        {
            foreach (var block in blocks)
            {
                block.ApplyBlockSetByType();
            }
        }
    }

    Vector3 CalculateDynamicObjectStartPosition()
    {
        return transform.position - Vector3.down * halfMapSectionHeight - blockOffsetY;
    }

    public void GenerateDynamicObjects()
    {
        GenerateWallBlocks();
        SpawnExtraHazardBlockGroups();
        SpawnBoostBlocks();
        CreateOutOfBoundsAreas();
    }

    public void GenerateWallBlocks(bool isTransitionRoom = false)
    {
        //wallOffset = isTransitionRoom ? Vector3.zero : new(MapController.Instance.GetWallOffset(), 0, 0);
        wallOffset = new(MapGenerator.Instance.GetWallOffset(), 0, 0);
        var startPosition = CalculateDynamicObjectStartPosition();
        numberOfBlocks = Mathf.FloorToInt(mapSectionHeight / MapGenerator.Instance.wallBlockHeight);

        // Left Wall blocks
        var leftStartPosition = startPosition - (halfMapSectionWidth * Vector3.right) - blockOffsetX + wallOffset;

        for (int i = 0; i < numberOfBlocks; i++)
        {
            var blockPosition = leftStartPosition + Vector3.down * (i * MapGenerator.Instance.wallBlockHeight);
            MapGenerator.Instance.SpawnWallBlock(blockPosition, SegmentId, this, isTransitionRoom);
        }

        // Right Wall blocks
        var rightStartPosition = startPosition + (halfMapSectionWidth * Vector3.right) + blockOffsetX - wallOffset;

        for (int i = 0; i < numberOfBlocks; i++)
        {
            var blockPosition = rightStartPosition + Vector3.down * (i * MapGenerator.Instance.wallBlockHeight);
            MapGenerator.Instance.SpawnWallBlock(blockPosition, SegmentId, this, isTransitionRoom);
        }

        //print($"WALLS - mapSection: {name} / isTransitionRoom: {isTransitionRoom} / wallOffset: {wallOffset.x} / numberOfBlocks: {numberOfBlocks} / prevWidth: {MapController.Instance.previousShaftWidth} / shaftWidth: {MapController.Instance.shaftWidth}");
    }

    void SpawnExtraHazardBlockGroups()
    {
        var startPosition = CalculateDynamicObjectStartPosition();
        numberOfBlocks = Mathf.FloorToInt(mapSectionHeight / MapGenerator.Instance.wallBlockHeight);

        for (int i = 0; i < numberOfBlocks; i++)
        {
            if (Random.value <= LevelController.Instance.ExtraHazardSpawnRate)
            {
                var randomPosX = Random.Range(-5, 5);
                var blockPositionX = new Vector3(randomPosX, 0, 0);
                var blockPositionY = startPosition + Vector3.down * (i * MapGenerator.Instance.wallBlockHeight);
                var spawnPos = blockPositionX + blockPositionY;

                int randomIndex = Random.Range(0, MapGenerator.Instance.BlockGroupBank.Length);
                var BlockGroupToSpawn = MapGenerator.Instance.BlockGroupBank[randomIndex];
                var BlockGroup = Instantiate(BlockGroupToSpawn, spawnPos, Quaternion.identity, MapGenerator.Instance.ObjectContainer.transform);
                var blockGroup = BlockGroup.GetComponent<BlockGroup>();
                blockGroup.SegmentId = SegmentId;

                MapGenerator.Instance.BlockGroups.Add(blockGroup);
                MapGenerator.Instance.blockGroupsSpawned++;
            }
        }
    }

    void SpawnBoostBlocks()
    {
        var startPosition = CalculateDynamicObjectStartPosition();
        numberOfBlocks = Mathf.FloorToInt(mapSectionHeight / MapGenerator.Instance.wallBlockHeight);

        for (int i = 0; i < numberOfBlocks; i++)
        {
            if (Random.value <= MapGenerator.Instance.boostBlockSpawnChance)
            {
                var randomPosX = Random.Range(-5, 5);
                var blockPositionX = new Vector3(randomPosX, 0, 0);
                var blockPositionY = startPosition + Vector3.down * (i * MapGenerator.Instance.wallBlockHeight);
                var spawnPos = blockPositionX + blockPositionY;
                var NitroBlock = Instantiate(MapGenerator.Instance.NitroBlockPrefab, spawnPos, Quaternion.identity, MapGenerator.Instance.ObjectContainer.transform);

                MapGenerator.Instance.NitroBlocks.Add(NitroBlock);
                MapGenerator.Instance.nitroBlocksSpawned++;
            }
        }
    }

    void CreateOutOfBoundsAreas()
    {
        var blockWidth = 1f / 11f;
        var blockOffset = blockOffsetX * 4;
        var outOfBoundsWidth = Mathf.Abs(MapGenerator.Instance.GetWallOffset()) * blockWidth;

        var leftWallPosition = transform.position - (0.5f * mapSectionWidth * Vector3.right) - blockOffset + wallOffset;
        var leftPlayAreaBoundary = transform.position - (0.5f * mapSectionWidth * Vector3.right);
        var leftCenterPoint = (leftWallPosition + leftPlayAreaBoundary) / 2;
        leftOutOfBoundsArea = CreateOutOfBoundsArea(leftCenterPoint, outOfBoundsWidth);

        var rightWallPosition = transform.position + (0.5f * mapSectionWidth * Vector3.right) + blockOffset - wallOffset;
        var rightPlayAreaBoundary = transform.position + (0.5f * mapSectionWidth * Vector3.right);
        var rightCenterPoint = (rightWallPosition + rightPlayAreaBoundary) / 2;
        rightOutOfBoundsArea = CreateOutOfBoundsArea(rightCenterPoint, outOfBoundsWidth);
    }

    GameObject CreateOutOfBoundsArea(Vector3 position, float width)
    {
        var OutOfBoundsArea = Instantiate(MapGenerator.Instance.OutOfBoundsAreaPrefab, position, Quaternion.identity);
        OutOfBoundsArea.transform.localScale = new Vector3(width, transform.localScale.y, 1);
        var area = OutOfBoundsArea.GetComponent<DisableExistingObject>();
        area.parentMapSection = this;
        area.segmentId = SegmentId;
        OutOfBoundsArea.transform.SetParent(transform);
        OutOfBoundsArea.transform.DOScaleX(MapGenerator.Instance.GetWallOffset() * (1f / 11f) - 0.02f, 0f);
        return OutOfBoundsArea;
    }

    private void OnDrawGizmos()
    {
        if (isFirstInSegment) DrawSectionMarker(topNode.transform.position, 30, Color.white, $"Segment_{SegmentId}");
        DrawSectionMarker(topNode.transform.position, 10, Color.red, $"{Utils.ShortName(name)}");
    }

    private void DrawSectionMarker(Vector2 center, float lineLength, Color color, string text)
    {
        Gizmos.color = color;

        var leftPoint = center - new Vector2(lineLength, 0f);
        var rightPoint = center + new Vector2(lineLength, 0f);

        Gizmos.DrawLine(leftPoint, rightPoint);

        // Draw text at the right end
        var style = new GUIStyle();
        style.normal.textColor = color;
        style.fontSize = 12;

        var textPosition = new Vector3(rightPoint.x, rightPoint.y, 0f);
        Handles.Label(textPosition, text, style);
    }
}
