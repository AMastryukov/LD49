using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static Action OnTilePlaced;

    private static float _cellHeight = 0.7f;
    private static float _cellWidth = 1f;
    // This is how the map generation positions Cells in the scene
    public static List<Vector3> Offsets = new List<Vector3>
    {
        new Vector3(-_cellWidth, 0, _cellHeight * 2.5f),
        new Vector3(_cellWidth, 0, _cellHeight * 2.5f),
        new Vector3(_cellWidth * 2f, 0, 0),
        new Vector3(_cellWidth, 0, _cellHeight * -2.5f),
        new Vector3(-_cellWidth, 0, _cellHeight * -2.5f),
        new Vector3(-_cellWidth * 2f, 0, 0),
    };

    [SerializeField] private GameObject cellPrefab;

    public List<Cell> Cells { get; set; }
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

    private void Start()
    {

    }

    // Returns an IEnumerable that contains coordinates, or Cells, or whatever else, where the player can place the currently held Tile
    public IEnumerable FindAllValidLocations(Tile tile)
    {
        // Return an empty list of cells for now
        return new List<Cell>();
    }

    // Instantiate the tile prefab on top of the cell
    // Returns true if placed successfully, false if not
    public bool TryPlaceTileAt(GameObject tilePrefab, Cell cell)
    {
        var newTile = Instantiate(tilePrefab).GetComponent<Tile>();

        newTile.transform.SetParent(cell.transform);

        cell.Tile = newTile;
        Cells.Add(cell);

        OnTilePlaced?.Invoke();

        return true;
    }

    private void GenerateTest()
    {
        // Generate the origin cell
        var originCell = Instantiate(cellPrefab, Vector3.zero, Quaternion.identity);
        Cells.Add(originCell.GetComponent<Cell>());

        GenerateNeighboringCells(originCell.GetComponent<Cell>());
    }

    private void GenerateNeighboringCells(Cell cell)
    {
        for (int i = 0; i < 6; i++)
        {
            if (cell.Neighbors[i] == null)
            {
                var newCell = Instantiate(cellPrefab, cell.transform.position + Offsets[i], Quaternion.identity);
                Cells.Add(newCell.GetComponent<Cell>());

                cell.Neighbors[i] = newCell.GetComponent<Cell>();
            }
        }
    }
}
