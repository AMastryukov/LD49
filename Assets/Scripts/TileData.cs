using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Tile Data")]
public class TileData : ScriptableObject
{
    public string Name;
    public int MilitaryInfluence;
    public int EconomyInfluence;
    public int CultureInfluence;
}
