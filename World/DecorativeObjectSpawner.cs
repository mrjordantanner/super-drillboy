using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;


public class DecorativeObjectSpawner : MonoBehaviour
{
    #region Singleton
    public static DecorativeObjectSpawner Instance;
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

    }

    #endregion

   // public float offsetY = 100f;
    public float destroyAfterDistance = 1000;

    //public List<DecorativeObject> SpawnedObjects = new();

    [Header("Gizmos")]
    public float lineLength = 1f;
    public Color gizmoColor = Color.yellow;

    private void Start()
    {
        Init();
        InvokeRepeating(nameof(CheckDepth), 0, 0.5f);
    }

    private void Init()
    {
        foreach (var levelThemes in LevelController.Instance.LevelThemeBank)
        {
            foreach (var data in levelThemes.decorativeObjects)
            {
                data.lastSpawnDepth = 0;
            }

        }
    }

    void CheckDepth()
    {
        if (!GameManager.Instance.gameRunning || MapGenerator.Instance.isTransitionPending) return;

        var currentDepth = PlayerManager.Instance.currentDepth;
        var decorativeObjects = LevelController.Instance.selectedLevel.decorativeObjects;

        foreach (var data in decorativeObjects)
        {
            if (currentDepth - data.lastSpawnDepth >= data.spawnRateByDepth)
            {
                SpawnObject(data);
                //print($"Spawned decorative object: {data.prefab.name}");
            }
        }

        // TODO do we need to do this every second?  make separate invoke and method?
        //foreach (var obj in SpawnedObjects)
        //{
        //    if (currentDepth - obj.depthOnSpawn >= destroyAfterDistance)
        //    {
        //         TODO iterate through the list backwards to remove items safely
        //        SpawnedObjects.Remove(obj);
        //        if (obj.gameObject != null)
        //        {
        //            Destroy(obj.gameObject);
        //        }

        //    }
        //}
    }

    public void SpawnObject(DecorativeObjectData data)
    {
        data.lastSpawnDepth = PlayerManager.Instance.currentDepth;

        //var objectLength = data.prefab.transform.localScale.y;
        //var offsetY = transform.position.y - 0.5f * objectLength;// * Vector3.down;
        //var spawnOffset = new Vector3(data.offsetX, offsetY, 0);
        // var offset = new Vector3(data.offsetX, offsetY, 0);
        //var targetPosition = new Vector3(transform.position.x + data.offsetX, transform.position.y, transform.position.z);

        // Calculate the desired position for top alignment
        // Vector3 alignmentPosition = targetPosition - new Vector3(0f, GetComponent<BoxCollider>().size.y / 2f, 0f);

        // Move the GameObject to the alignment position
        //  transform.Translate(alignmentPosition - transform.position, Space.World);

        var offset = new Vector3(data.offsetX, data.offsetY, 0);

        if (data.prefab == null)
        {
            print("Attempted to spawn decorative object but it was null");
            return;
        }

        var Obj = Instantiate(data.prefab, transform.position + offset, Quaternion.identity, transform);


        var decor = Obj.GetComponent<DecorativeObject>();
        decor.depthOnSpawn = PlayerManager.Instance.currentDepth;

        //Obj.transform.Translate(new Vector3(data.offsetX, newPositionY, 0));
        //SpawnedObjects.Add(decor);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        Gizmos.DrawLine(transform.position, transform.position + transform.up * (lineLength / 2));
        Gizmos.DrawLine(transform.position, transform.position + transform.up * -(lineLength / 2));

        var leftPoint = (Vector2)transform.position - new Vector2(lineLength, 0f);
        var rightPoint = (Vector2)transform.position + new Vector2(lineLength, 0f);

        Gizmos.DrawLine(leftPoint, rightPoint);

        // Draw text at the right end
        //var style = new GUIStyle();
        //style.normal.textColor = gizmoColor;
        //style.fontSize = 12;

        //var textPosition = new Vector3(rightPoint.x, rightPoint.y, 0f);
        //Handles.Label(textPosition, "DecorSpawner", style);
    }

}


