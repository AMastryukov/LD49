using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class plays through the game in the backend
// This is for game testing and balance purposes
public class GameSimulator : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private List<Tile> tilePrefabs;

    private GridManager _gridManager;

    private void Awake()
    {
        _gridManager = FindObjectOfType<GridManager>();
    }

    public void PlaceRandomTile()
    {
        Tile randomTilePrefab = tilePrefabs[UnityEngine.Random.Range(0, tilePrefabs.Count - 1)];
        Tile randomTileInstance = Instantiate(randomTilePrefab, _gridManager.transform, true);

        //////////////////////////////////////////////////////
        /// DO TILE CONFIGURATION HERE  //////////////////////
        //////////////////////////////////////////////////////

        List<Hex> spots = _gridManager.GetAllValidSpots();

        _gridManager.RegisterAndPlaceTile(randomTileInstance, spots[UnityEngine.Random.Range(0, spots.Count)]);
    }
}
