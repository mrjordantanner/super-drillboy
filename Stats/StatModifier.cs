using System.Collections.Generic;
using UnityEngine;
using System;


// from https://forum.unity.com/threads/tutorial-character-stats-aka-attributes-system.504095/

public enum StatModType
{
    Flat,
    PercentAdd,
    PercentMult,
    Multiply
}

public enum StatModSource
{
    NotSet,
    EquippedSuit,
    CharacterUpgrade
}

[Serializable]
[CreateAssetMenu(menuName = "Stats/Stat Modifier")]
public class StatModifier : ScriptableObject
{
    public float Value;
    public StatModType ModType;
    [ReadOnly] public StatModSource Source;

    // Can be as generic as "PlusOne".
    // Source can be set dynamically when the StatMod is applied to the CharacterStat.
    // Various Upgrade UI elements can hold references to a StatModifier that will be applied to the appropiate CharacterStat when purchased.
    // Upgrades could hold more than one StatModifier, like an equippable Suit or other item that has several different properties
    // A Player's owned StatMods will be held in their Inventory and synced with the Cloud.
}
