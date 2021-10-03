using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Tile Tile { get; set; } = null;
    public List<Cell> Neighbors { get; set; }

    private void Awake()
    {
        Neighbors = new List<Cell>();
    }
}
