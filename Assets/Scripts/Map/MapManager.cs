using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    // This is how the map generation positions Cells in the scene
    public static List<Vector3> Offsets = new List<Vector3>
    {
        Vector3.one,
        Vector3.one,
        Vector3.one,
        Vector3.one,
        Vector3.one,
        Vector3.one,
    };

    public List<Cell> Cells;
    public Cell OriginCell
    {
        get
        {
            if (Cells.Count > 0)
            {
                return Cells[0];
            }

            return null;
        }
    }

    private void Awake()
    {
        Cells = new List<Cell>();
    }
}
