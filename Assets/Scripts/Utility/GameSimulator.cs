using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class plays through the game in the backend
// This is for game testing and balance purposes
public class GameSimulator : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private List<GameObject> tilePrefabs;

    private MapManager _mapManager;

    private void Awake()
    {
        _mapManager = FindObjectOfType<MapManager>();
    }

    public void PlaceRandomTile()
    {
        // Generate a cell and random tile, then register it with the map manager
        var newCell = Instantiate(cellPrefab).GetComponent<Cell>();

        var randomTile = tilePrefabs[UnityEngine.Random.Range(0, tilePrefabs.Count - 1)];
        _mapManager.TryPlaceTileAt(randomTile, newCell);
    }
}
