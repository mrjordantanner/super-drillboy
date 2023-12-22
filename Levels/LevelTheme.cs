using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Level/LevelTheme")]
public class LevelTheme : ScriptableObject
{
    //public bool isUnlocked;
    public string levelName;
    public AudioClip music;
    public Sprite levelPreviewSprite;
    public Sprite previewIcon;

    [Header("Adventure Properties")]
    public int currentDifficulty;            // TODO use playerPrefs
    public int maxDifficulty = 1;            // TODO save this to cloud as custom data associated with the level inventory item             
    //public int currentCheckpointsPerLevel;
    public int basePlayCost = 50;
    public float baseGemRewardBonus;

    [Header("Blocks")]
    public BlockSet block_solid;
    public BlockSet block_destructible;
    public BlockSet block_damage;
    public BlockSet block_wall;

    [Header("Backgrounds")]
    public Material backgroundLayer_front;
    public Material backgroundLayer_mid;
    public Material backgroundLayer_back;

    [Header(" ")]
    [Range(1, 6)]
    public int previewRowLength = 5;
    [Range(30, 90)]
    public int previewSize = 60;

    [Header(" ")]
    public DecorativeObjectData[] decorativeObjects;

    public void Init()
    {
        currentDifficulty = 1;
        decorativeObjects = Resources.LoadAll<DecorativeObjectData>($"DecorativeObjects/{levelName}");
    }



}