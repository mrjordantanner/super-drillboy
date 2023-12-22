using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public class MapGenerator : MonoBehaviour
{
    #region Singleton
    public static MapGenerator Instance;
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

    #region Declarations

    public MapSection startingSection;
    bool setInitialCheckpoint;
    public bool isMapReady;

    [Header("Map Generation")]
    [Range(12, 20)]
    public int totalSegmentWeight = 12;
    public int createCheckPointAfterNumberofSegments = 3;
    public int changeWidthCheckpointQuota = 1;
    public int mapSectionInstancesToCreate = 2;
    public int maxGetMapSectionAttempts = 15;
    public int smallMapSectionWeight = 1,
        mediumMapSectionWeight = 2,
        largeMapSectionWeight = 4,
        extraLargeMapSectionWeight = 12;

    int checkpointsReachedThisLevel;
    int mapSectionsCreatedThisSegment;  // used by MapSection gizmos to mark beginning of Segment

    [Header("Generator Stats")]
    [ReadOnly] public double mapSectionRerolls;
    [ReadOnly] public double totalMapSectionsCreated;
    [ReadOnly] public double mapSectionRerollRate;

    [Header("Spawn Chance")]
    public float widthChangeChance = 1f;
    public float chanceToDecreaseWidth = 0.65f;
    public float levelMapSectionSpawnChance = 0.35f;

    [Header("Transition")]
    [ReadOnly] public bool isTransitionPending;
    [ReadOnly] public bool isFinishPending;

    [Header("Walls")]
    public bool startAtMaxWidth;
    [OddNumberRange(5, 11)] public int shaftWidth = 11;
    [ReadOnly] public int previousShaftWidth;

    [HideInInspector] public int wallBlockPoolSize = 5000;
    [HideInInspector] public int minShaftWidth = 5;
    [HideInInspector] public int maxShaftWidth = 11;
    [HideInInspector] public float wallBlockHeight;

    [Header("Prefabs & References")]
    public BlockSet transitionRoomWallBlockSet;
    public GameObject OutOfBoundsAreaPrefab;
    public GameObject TransitionRoomPrefab;
    public GameObject FinishRoomPrefab;  // Adv mode
    public GameObject CheckpointPrefab;  
    public GameObject SegmentTriggerPrefab;
    public GameObject WallBlockPrefab;
    public GameObject SolidBlockPrefab, DestBlockPrefab, DamageBlockPrefab, NitroBlockPrefab;
    public Sprite transitionRoomWallBlockSprite;
    public Material blockHitFlashMaterial;
    public Material blockHitFlashMaterial2;
    public GameObject WallBlockContainer;
    public GameObject ObjectContainer;

    [ReadOnly]
    public float boostBlockSpawnChance = 0.009f;

    [HideInInspector] public float blockRegenerationTime = 6f;
    [HideInInspector] public float blockHitFlashDuration = 0.015f;

    [Header("")]
    public ScrollingBackground[] scrollingBackgrounds;

    [Header("Stats")]
    public int
        blocksDestroyed,
        totalSegmentsCreated,
        blockGroupsSpawned,
        destBlocksSpawned,
        nitroBlocksSpawned;

    Transform lastNode;
    [HideInInspector]
    public GameObject[] BlockGroupBank;

    //List<ISegmentIdentifier> MapSections = new();
    //List<ISegmentIdentifier> Checkpoints = new();
    //List<ISegmentIdentifier> WallBlocks = new();
    //List<ISegmentIdentifier> BlockGroups = new();

    List<MapSection> MapSections = new();
    List<Checkpoint> Checkpoints = new();

    [HideInInspector] public List<WeightedMapSectionPool> LevelMapPools = new();
    [HideInInspector] public List<WallBlock> WallBlocks = new();
    [HideInInspector] public List<BlockGroup> BlockGroups = new();
    [HideInInspector] public List<GameObject> NitroBlocks = new();
    [HideInInspector] public BlockSet NitroBlockSet;
    Queue<GameObject> inactiveWallBlocks = new();

    [Serializable]
    public struct WeightedMapSectionPool
    {
        public int weight;
        public Dictionary<string, List<GameObject>> instances;
    }
    [HideInInspector] public WeightedMapSectionPool SmallMapPool, MediumMapPool, LargeMapPool, ExtraLargeMapPool;

    int getMapSectionAttempts = 0;
    int segmentsGeneratedWithoutCheckpoint;
    int currentWeightSum;
    #endregion

    void Update()
    {
        if (totalMapSectionsCreated > 0)
        {
            mapSectionRerollRate = Math.Round(mapSectionRerolls / totalMapSectionsCreated, 2);
        }
    }

    public void Init()
    {
        //GameManager.Instance.OnFrameCount += HandleFrameCountEvent;
        wallBlockHeight = WallBlockPrefab.transform.localScale.y;

        NitroBlockSet = Resources.Load<BlockSet>("BlockSets/BS-Nitro");
        BlockGroupBank = Resources.LoadAll<GameObject>("BlockGroups");

        SmallMapPool = CreatedWeightedMapSectionPool("MapSections/_Common/Small", smallMapSectionWeight);
        MediumMapPool = CreatedWeightedMapSectionPool("MapSections/_Common/Medium", mediumMapSectionWeight);
        LargeMapPool = CreatedWeightedMapSectionPool("MapSections/_Common/Large", largeMapSectionWeight);
        ExtraLargeMapPool = CreatedWeightedMapSectionPool("MapSections/_Common/ExtraLarge", extraLargeMapSectionWeight);

        foreach (var levelTheme in LevelController.Instance.LevelThemeBank)
        {
            var newMapPool = CreatedWeightedMapSectionPool($"MapSections/{levelTheme.levelName}", mediumMapSectionWeight);
             LevelMapPools.Add(newMapPool);
        }

        print($"Created {LevelMapPools.Count} LevelMapSectionPools");

        InitializeWallBlockPool();
        InitializeMap();
    }

    public void InitializeMap()
    {
        isMapReady = false;
        lastNode = startingSection.bottomNode;
        mapSectionRerollRate = totalMapSectionsCreated = mapSectionRerolls = 0;
        setInitialCheckpoint = true;
        isTransitionPending = false;
        isFinishPending = false;

        blocksDestroyed = totalSegmentsCreated = blockGroupsSpawned = destBlocksSpawned = nitroBlocksSpawned = 0;
    }

    public void StartMapGenerator()
    {
        LevelController.Instance.UpdateLevelTheme();
        LevelController.Instance.InitializeLevels();
        ApplyLevelThemeBackgrounds();

        if (startAtMaxWidth) shaftWidth = maxShaftWidth;
        previousShaftWidth = maxShaftWidth;
        StartCoroutine(GenerateNewSegment(true));
    }

    public void ClearMap()
    {
        foreach (var mapSection in MapSections)
        {
            mapSection.DisableMapSection();
        }
        MapSections.Clear();

        foreach (var checkpoint in Checkpoints)
        {
            Destroy(checkpoint.gameObject);
        }
        Checkpoints.Clear();

        foreach (var wallBlock in WallBlocks)
        {
            Destroy(wallBlock.gameObject);
        }
        WallBlocks.Clear();

        foreach (var blockGroup in BlockGroups)
        {
            Destroy(blockGroup.gameObject);
        }
        BlockGroups.Clear();

        foreach (var boostBlock in NitroBlocks)
        {
            Destroy(boostBlock.gameObject);
        }
        NitroBlocks.Clear();


    }

    public WeightedMapSectionPool CreatedWeightedMapSectionPool(string resourcesPath, int weight)
    {
        var sectionArray = Resources.LoadAll<GameObject>(resourcesPath);
        if (sectionArray.Length == 0) return new WeightedMapSectionPool();

        Dictionary<string, List<GameObject>> mapSectionInstances = new();

        foreach (var section in sectionArray)
        {
            var sectionList = new List<GameObject>();

            for (int i = 0; i < mapSectionInstancesToCreate; i++)
            {
                var NewSection = Instantiate(section, transform.position, Quaternion.identity, transform);
                NewSection.name = Utils.ShortName(section.name);
                NewSection.SetActive(false);
                sectionList.Add(NewSection);
            }

            mapSectionInstances.Add(section.name, sectionList);
        }

        //print($"Created WeightedMapSectionPool from Path {resourcesPath}.  MapSectionCount: {mapSectionInstances.Count}");

        return new WeightedMapSectionPool()
        {
            weight = weight,
            instances = mapSectionInstances
        };
    }

    Dictionary<string, List<GameObject>> DetermineMapSectionPool()
    {
        var remainingWeight = totalSegmentWeight - currentWeightSum;

        if (remainingWeight > 0)
        {
            // Equal to 1
            if (remainingWeight == SmallMapPool.weight)
            {
                currentWeightSum += SmallMapPool.weight;
                return SmallMapPool.instances;
            }

            // Either 2 or 3
            if (remainingWeight > SmallMapPool.weight && remainingWeight < LargeMapPool.weight)
            {
                //var sumOfSections = SmallMapPool.instances.Values.Count + MediumMapPool.instances.Values.Count;
                //var smallChance = SmallMapPool.instances.Values.Count / sumOfSections;
                //var mediumChance = MediumMapPool.instances.Values.Count / sumOfSections;

                if (Utils.CoinFlip())
                {
                    currentWeightSum += SmallMapPool.weight;
                    return SmallMapPool.instances;
                }
                else
                {
                    currentWeightSum += MediumMapPool.weight;
                    return MediumMapPool.instances;
                }
            }

            // Between 4 & 11 
            if (remainingWeight >= LargeMapPool.weight && remainingWeight < ExtraLargeMapPool.weight)
            {
                var threeChance = Random.value;
                if (threeChance <= 0.425f)
                {
                    currentWeightSum += SmallMapPool.weight;
                    return SmallMapPool.instances;
                }
                else if (threeChance > 0.425f && threeChance <= 0.85f)
                {
                    currentWeightSum += MediumMapPool.weight;
                    return MediumMapPool.instances;
                }
                else
                {
                    currentWeightSum += LargeMapPool.weight;
                    if (RollForLevelMapSection()) return GetLevelMapSectionPool();
                    return LargeMapPool.instances;
                }
            }

            // 12 and above
            if (remainingWeight >= ExtraLargeMapPool.weight)
            {
                var fourChance = Random.value;
                if (fourChance <= 0.25f)
                {
                    currentWeightSum += SmallMapPool.weight;
                    return SmallMapPool.instances;
                }
                else if (fourChance > 0.25f && fourChance <= 0.50f)
                {
                    currentWeightSum += MediumMapPool.weight;
                    return MediumMapPool.instances;
                }
                else if (fourChance > 0.50f && fourChance <= 0.75f)
                {
                    currentWeightSum += LargeMapPool.weight;
                    if (RollForLevelMapSection()) return GetLevelMapSectionPool();
                    return LargeMapPool.instances;
                }
                else
                {
                    currentWeightSum += ExtraLargeMapPool.weight;
                    return ExtraLargeMapPool.instances;
                }
            }
        }

        return SmallMapPool.instances;
    }

    bool RollForLevelMapSection()
    {
        if (LevelMapPools.Count > 0) return LevelMapPools[LevelController.Instance.selectedLevelIndex].instances.Count > 0 && Random.value <= levelMapSectionSpawnChance;
        else print("MapController - Warning: Selected level has no LevelMapPools");
        return false;
    }

    Dictionary<string, List<GameObject>> GetLevelMapSectionPool()
    {
        return LevelMapPools[LevelController.Instance.selectedLevelIndex].instances;
    }

    private void GenerateCheckpointOrSegmentTrigger(bool generateCheckpoint)
    {
         if (generateCheckpoint)
        {
            segmentsGeneratedWithoutCheckpoint = 0;
            var checkpointPosition = new Vector3(lastNode.transform.position.x, lastNode.transform.position.y - 0.5f, lastNode.transform.position.z);
            var checkpointWidth = previousShaftWidth > shaftWidth ? shaftWidth : previousShaftWidth;

            var NewCheckpoint = Instantiate(CheckpointPrefab, checkpointPosition, Quaternion.identity);
            NewCheckpoint.GetComponent<SpriteRenderer>().size = new Vector2(checkpointWidth, 0.5f);

            var checkpoint = NewCheckpoint.GetComponent<Checkpoint>();
            checkpoint.SegmentId = totalSegmentsCreated;
            checkpoint.isFirstCheckpointInLevel = isTransitionPending;

            if (setInitialCheckpoint)
            {
                checkpoint.isFirstCheckpointInLevel = true;
                setInitialCheckpoint = false;
            }

            Checkpoints.Add(checkpoint);
            AddFillerWallBlocks(checkpointPosition);

            //print($"Map: Generated checkpoint - Id:{totalSegmentsCreated}");
        }
        // Create invisible SegmentTrigger
        else
        {
            segmentsGeneratedWithoutCheckpoint++;
            var position = new Vector3(lastNode.transform.position.x, lastNode.transform.position.y - 0.5f, lastNode.transform.position.z);
            var width = maxShaftWidth;

            var NewTrigger = Instantiate(SegmentTriggerPrefab, position, Quaternion.identity);
            NewTrigger.GetComponent<SpriteRenderer>().size = new Vector2(width, 0.5f);

            var trigger = NewTrigger.GetComponent<SegmentTrigger>();
            trigger.SegmentId = totalSegmentsCreated;

            //print($"Map: Generated segment trigger - Id:{totalSegmentsCreated}");
        }
    }

    /// <summary>
    /// Generates a new MapSegment which is a group of smaller MapSections that share a common SegmentId and whose total weight (length) add up to TotalSegmentWeight.
    /// </summary>
    public IEnumerator GenerateNewSegment(bool forceGenerateCheckpoint = false)
    {
        //print($"Map: ==============================================");
        //print($"Map: ========== Generating New Segment ============");
        //print($"Map: ==============================================");

        var generateCheckpoint = forceGenerateCheckpoint || isTransitionPending || segmentsGeneratedWithoutCheckpoint == createCheckPointAfterNumberofSegments - 1;

        // Create transition room at the beginning of segment if transition is pending
        if (isTransitionPending)
        {
            // New Level always starts at maxWidth
            shaftWidth = maxShaftWidth; 
            StartCoroutine(GenerateMapSection(true));
        }

        GenerateCheckpointOrSegmentTrigger(generateCheckpoint);

        // Loop and create map sections from different mapSection pools until totalSegmentWeight has been reached
        currentWeightSum = 0;
        mapSectionsCreatedThisSegment = 0;
        do
        {
            yield return StartCoroutine(GenerateMapSection());
        }
        while (currentWeightSum < totalSegmentWeight);
       // print($"Map: Do/While loop complete - Id: {totalSegmentsCreated}");

        if (isFinishPending)
        {
            // isFinishPending = false;
            shaftWidth = maxShaftWidth;
            yield return StartCoroutine(GenerateMapSection(false, true));
        }

        //previousShaftWidth = isTransitionPending ? maxShaftWidth : shaftWidth;
        previousShaftWidth = shaftWidth;
        totalSegmentsCreated++;
    }

    void AddFillerWallBlocks(Vector3 centerPosition, bool isTransitionRoom = false)
    {
        var widthDifference = (previousShaftWidth - shaftWidth) / 2;
        var numberOfBlocks = Mathf.Abs(widthDifference) + 1;
        var fillDirection = Mathf.Sign(widthDifference);

        //print($"FILL -- prevWidth: {previousShaftWidth} / shaftWidth: {shaftWidth} / widthDiff: {widthDifference} / noBlocks: {numberOfBlocks} / fillDir: {fillDirection}");

        var blockOffsetX = new Vector3(1, 0, 0);
        Vector3 wallOffset = new(GetWallOffset(), 0, 0);

        var mapSectionWidth = 11;
        var halfMapSectionWidth = mapSectionWidth / 2;

        var leftStartPosition = centerPosition - (halfMapSectionWidth * Vector3.right) - blockOffsetX + wallOffset;
        var rightStartPosition = centerPosition + (halfMapSectionWidth * Vector3.right) + blockOffsetX - wallOffset;

        // Left fill blocks
        for (int i = 0; i < numberOfBlocks; i++)
        {
            var blockPosition = leftStartPosition - i * fillDirection * Vector3.right;
            SpawnWallBlock(blockPosition, totalSegmentsCreated, null, isTransitionRoom);
        }

        // Right fill blocks
        for (int i = 0; i < numberOfBlocks; i++)
        {
            var blockPosition = rightStartPosition + i * fillDirection * Vector3.right;
            SpawnWallBlock(blockPosition, totalSegmentsCreated, null, isTransitionRoom);
        }
    }

    public void SpawnWallBlock(Vector3 blockPosition, int SegmentId, MapSection parentMapSection, bool isTransitionRoom = false)
    {
        var Block = GetWallBlockFromPool();
        Block.transform.position = blockPosition;

        var block = Block.GetComponent<WallBlock>();
        block.SegmentId = SegmentId;
        block.parentMapSection = parentMapSection;
        var blockSet = isTransitionRoom ? transitionRoomWallBlockSet : LevelController.Instance.selectedLevel.block_wall;
        block.ApplyBlockSet(blockSet);
        WallBlocks.Add(block);
    }

    IEnumerator GenerateMapSection(bool isTransitionRoom = false, bool isFinishRoom = false)
    {
        GameObject NewSection;
        if (isTransitionRoom)
        {
            NewSection = Instantiate(TransitionRoomPrefab, lastNode.transform.position, Quaternion.identity, transform);
           // print($"Map: Transition Room - Id:{totalSegmentsCreated}");
        }
        else if (isFinishRoom)
        {
            NewSection = Instantiate(FinishRoomPrefab, lastNode.transform.position, Quaternion.identity, transform);
           // print($"Map: Finish Room - Id:{totalSegmentsCreated}");
        }
        else
        {
            NewSection = GetRandomMapSectionFromPool();
            if (NewSection)
            {
                NewSection.SetActive(true);
                NewSection.transform.position = lastNode.transform.position;

               // print($"Map: {NewSection.name} - Id:{totalSegmentsCreated}");
            }
            else
            {
                Debug.LogError("Failed to Generate Map Section");
                yield break;
            }
        }

        var mapSection = NewSection.GetComponent<MapSection>();
        mapSection.SegmentId = totalSegmentsCreated;
        mapSection.isFirstInSegment = mapSectionsCreatedThisSegment == 0;
        MapSections.Add(mapSection);
        mapSectionsCreatedThisSegment++;

        Vector3 newPosition = lastNode.parent.position + (lastNode.parent.localScale.y + NewSection.transform.localScale.y) * 0.5f * Vector3.down;
        NewSection.transform.position = newPosition;

        if (isTransitionRoom || isFinishRoom)
        {
            AddFillerWallBlocks(lastNode.position + new Vector3(0, 0.5f, 0), true);
            mapSection.GenerateWallBlocks(true);
        }
        else
        {
            mapSection.ApplyLevelTheme();
            mapSection.GenerateDynamicObjects();
        }

        lastNode = mapSection.bottomNode;
        totalMapSectionsCreated++;
        yield return new WaitForSeconds(0.02f);
    }

    void InitializeWallBlockPool()
    {
        //WallBlockPool = new List<GameObject>();
        for (int i = 0; i < wallBlockPoolSize; i++)
        {
            var Block = Instantiate(WallBlockPrefab, transform.position, Quaternion.identity, WallBlockContainer.transform);
            Block.name = "WallBlock";
            ReturnWallBlockToPool(Block);
            //WallBlockPool.Add(Block);
        }
    }

    public GameObject GetWallBlockFromPool()
    {
        if (inactiveWallBlocks.Count > 0)
        {
            var Block = inactiveWallBlocks.Dequeue();
            Block.SetActive(true);
            return Block;
        }

        Debug.LogError("Map Generator ran out of Wall Blocks in WallBlockPool");
        return null;
    }

    public void ReturnWallBlockToPool(GameObject Block)
    {
        Block.SetActive(false);
        inactiveWallBlocks.Enqueue(Block);
    }

    GameObject GetRandomMapSectionFromPool(int retryAttempts = 0)
    {
        var poolInstances = DetermineMapSectionPool();

        if (retryAttempts >= maxGetMapSectionAttempts)
        {
            Debug.LogError($"Map Generator made the max attempts of {retryAttempts} to get a MapSection.  Exiting loop and returning null.");
            getMapSectionAttempts = 0;
            return null;
        }

        var sectionReference = Utils.GetRandomFromDictionary(poolInstances);

        foreach (var instance in sectionReference.Value)
        {
            if (!instance.activeInHierarchy)
            {
                getMapSectionAttempts = 0;
                return instance;
            }
        }

        //print($"Map Generator tried to get MapSection: {sectionReference.Key} but it was already in use.  Rolling again.");
        mapSectionRerolls++;
        getMapSectionAttempts++;
        return GetRandomMapSectionFromPool(getMapSectionAttempts);
    }

    public void OnSegmentTriggerReached(SegmentTrigger trigger)
    {
        //GameManager.Instance.EnableFrameCounter(60);
        if (!isFinishPending) StartCoroutine(ProcessSegments(trigger.SegmentId));
    }

    public void OnCheckpointReached(Checkpoint checkpoint)
    {
        if (!isFinishPending) StartCoroutine(ProcessSegments(checkpoint.SegmentId));

        checkpointsReachedThisLevel++;
        if (checkpointsReachedThisLevel == LevelController.Instance.CheckpointsPerLevel)
        {
            if (GameManager.Instance.gameMode == GameMode.Survival) IncreaseLevel(); else isFinishPending = true;

            checkpointsReachedThisLevel = 0;
        }

        if (checkpoint.isFirstCheckpointInLevel)
        {
            StartCoroutine(HUD.Instance.ChangeLevelName(LevelController.Instance.selectedLevel.levelName));
        }

    }

    IEnumerator ProcessSegments(int segmentId)
    {
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(GenerateNewSegment());
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(DestroyPreviousSegment(segmentId));
    }

    IEnumerator DestroyPreviousSegment(int SegmentId)
    {
        if (SegmentId < 2) yield break;

        yield return new WaitForSeconds(0.1f);

        var SegmentIdToDestroy = SegmentId - createCheckPointAfterNumberofSegments - 1;

        //DestroyItemsMatchingSegmentId(MapSections, SegmentIdToDestroy);
        //DestroyItemsMatchingSegmentId(Checkpoints, SegmentIdToDestroy);
        //DestroyItemsMatchingSegmentId(WallBlocks, SegmentIdToDestroy);
        //DestroyItemsMatchingSegmentId(BlockGroups, SegmentIdToDestroy);

        // TODO these destroy loops could be optimized by saving each object that has the same segmentId in a separate list
        // Then, could just destroy each object in the list instead of having to iterate through a huge list and do segmentId comparisons
        // Also should consolidate into one method by using the interface, as above

       // int mapSectionsReturnedToPool = 0;
        for (int i = MapSections.Count - 1; i >= 0; i--)
        {
            var mapSection = MapSections[i];
            if (mapSection.SegmentId == SegmentIdToDestroy)
            {
                MapSections.RemoveAt(i);
                mapSection.DisableMapSection();
               // mapSection.gameObject.SetActive(false);
               // mapSectionsReturnedToPool++;
            }
        }
        //print($"{mapSectionsReturnedToPool} MapSections were returned to the pool.");

        for (int i = Checkpoints.Count - 1; i >= 0; i--)
        {
            var item = Checkpoints[i];
            if (item.SegmentId == SegmentIdToDestroy)
            {
                Checkpoints.RemoveAt(i);
                Destroy(item.gameObject);
            }
        }

        yield return new WaitForSeconds(0.1f);

        if (WallBlocks.Count > 0)
        {
            for (int i = WallBlocks.Count - 1; i >= 0; i--)
            {
                var item = WallBlocks[i];
                if (item.SegmentId == SegmentIdToDestroy)
                {
                    WallBlocks.RemoveAt(i);
                    ReturnWallBlockToPool(item.gameObject);
                }
            }
        }

        for (int i = BlockGroups.Count - 1; i >= 0; i--)
        {
            var item = BlockGroups[i];
            if (item.SegmentId == SegmentIdToDestroy)
            {
                BlockGroups.RemoveAt(i);
                Destroy(item.gameObject);
            }
        }
    }

    public void AdjustShaftWidth()
    {
        if (Random.value <= widthChangeChance)
        {
            if (shaftWidth == minShaftWidth)
            {
                IncreaseShaftWidth();
                return;
            }

            if (shaftWidth == maxShaftWidth)
            {
                DecreaseShaftWidth();
                return;
            }

            // Roll to decide if increase/decrease
            if (Random.value <= chanceToDecreaseWidth)
            {
                DecreaseShaftWidth();
            }
            else
            {
                IncreaseShaftWidth();

            }
        }
    }

    public void IncreaseShaftWidth()
    {
        int changeAmount = 2;
        if (Utils.CoinFlip())
        {
            changeAmount = Utils.CoinFlip() ? 4 : 6;
        }

        if (shaftWidth + changeAmount > maxShaftWidth)
        {
            shaftWidth = maxShaftWidth;
            return;
        }

        shaftWidth += changeAmount;
    }

    public void DecreaseShaftWidth()
    {
        int changeAmount = 2;
        if (Utils.CoinFlip())
        {
            changeAmount = Utils.CoinFlip() ? 4 : 6;
        }

        if (shaftWidth - changeAmount < minShaftWidth)
        {
            shaftWidth = minShaftWidth;
            return;
        }

        shaftWidth -= changeAmount;
    }

    public int GetWallOffset()
    {
        return (maxShaftWidth - shaftWidth) / 2;
    }

    public void UpdateScrollingBackground(Vector2 playerVelocity)
    {
        foreach (var bg in scrollingBackgrounds)
        {
            if (bg == null) continue;
            if (GameManager.Instance.gameOver) bg.scrollSpeed = 0;
            bg.scrollSpeed = playerVelocity.y * bg.speedPercentage / 10;
        }

    }

    public void ToggleScrollingBackgrounds(bool value)
    {
        foreach (var bg in scrollingBackgrounds)
        {
            bg.gameObject.SetActive(value);
        }
    }

    public void IncreaseLevel()
    {
        LevelController.Instance.SetDifficulty(LevelController.Instance.difficultyLevel + 1);
  
        if (GameManager.Instance.gameMode == GameMode.Survival)
        {
            LevelController.Instance.selectedLevelIndex++;
            LevelController.Instance.UpdateLevelTheme();
            isTransitionPending = true;
        }
    }

    public IEnumerator BackgroundTransition()
    {
        yield return new WaitForSeconds(0.25f);
        ApplyLevelThemeBackgrounds();
    }

    public void ApplyLevelThemeBackgrounds()
    {
        var materialFront = LevelController.Instance.selectedLevel.backgroundLayer_front ? LevelController.Instance.selectedLevel.backgroundLayer_front : null;
        var materialMid = LevelController.Instance.selectedLevel.backgroundLayer_mid ? LevelController.Instance.selectedLevel.backgroundLayer_mid : null;
        var materialBack = LevelController.Instance.selectedLevel.backgroundLayer_back ? LevelController.Instance.selectedLevel.backgroundLayer_back : null;

        scrollingBackgrounds[0].UpdateMaterial(materialFront);
        scrollingBackgrounds[1].UpdateMaterial(materialMid);
        scrollingBackgrounds[2].UpdateMaterial(materialBack);
    }

    //void HandleFrameCountEvent(int frameCount)
    //{
    //   //print("!! EVENT FIRED - Frames elapsed: " + frameCount);
    //}

    //public void DestroyItemsMatchingSegmentId(List<ISegmentIdentifier> list, int SegmentId)
    //{
    //    // Iterate through the list in reverse order to safely remove items
    //    for (int i = list.Count - 1; i >= 0; i--)
    //    {
    //        var item = list[i];
    //        if (item.SegmentId == SegmentId)
    //        {
    //            list.RemoveAt(i);
    //            Destroy(item.Entity);
    //        }
    //    }
    //}





}
