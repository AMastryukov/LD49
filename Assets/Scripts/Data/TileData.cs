using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ETileType
{
    Military,
    Economy,
    Culture,
    None
}

[CreateAssetMenu(menuName ="Tile Data")]
public class TileData : ScriptableObject
{
    public string Name;
    public ETileType TileType;

    // We can't use Pillars object because
    // we need to have these exposed in Inspector
    public int Military;
    public int Economy;
    public int Culture;
}
