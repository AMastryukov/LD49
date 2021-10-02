using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Tile Tile = null;
    public List<Cell> Neighbors;

    private void Awake()
    {
        Neighbors = new List<Cell>();
    }
}
