using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTray : MonoBehaviour
{
    public static Action OnSpaceOnTray;

    [SerializeField] private List<Transform> tilePlaceHolders;
    [SerializeField] private Transform tileSpawn;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float returnSpeed = 1f;

    private int tileCapacity;
    private Tile grabbedTile;
    private List<Tile> tileTrayTiles;
    private GridManager _gridManager;

    private void Awake()
    {
        _gridManager = FindObjectOfType<GridManager>();

        tileCapacity = tilePlaceHolders.Count;
        tileTrayTiles = new List<Tile>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {   
            TryGrabTile();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!TryPlaceTile())
            {
                TryReleaseTile();
            }
            
            _gridManager.EndPreview();
        }

        if (Input.GetMouseButton(0))
        {
            if (grabbedTile != null)
            {
                TryMoveGrabbedTile();
                _gridManager.PreviewTile();
            }
        }

        // Move neutral tiles back to the tray
        for (int i = 0; i < tileTrayTiles.Count; i++)
        {
            Tile tile = tileTrayTiles[i];
            if(tile == grabbedTile)
            {
                // Don't reset this tile
                continue;
            }

            Vector3 pos = tilePlaceHolders[i].position;

            // Only if the tile is far
            if(Vector3.Distance(tile.transform.position, pos) > 0.01)
            {
                tile.transform.position = Vector3.Lerp(tile.transform.position, pos, returnSpeed * Time.deltaTime);
            }
            
        }
    }

    public bool IsSpaceOnTray()
    {
        return tileTrayTiles.Count != tileCapacity;
    }

    public bool TryAddTileToTray(Tile tile)
    {
        if (!IsSpaceOnTray())
        {
            return false;
        }

        tileTrayTiles.Add(tile);

        return true;
    }

    public bool TryGrabTile()
    {
        if (grabbedTile != null)
        {
            return false;
        }

        RaycastHit rayHit;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out rayHit, Mathf.Infinity, LayerMask.GetMask("Tile"), QueryTriggerInteraction.Collide))
        {
            Tile hitTile = rayHit.transform.gameObject.GetComponent<Tile>();

            if (hitTile == null || hitTile.tileState != ETileState.Neutral)
            {
                return false;
            }
            
            grabbedTile = hitTile;
            hitTile.tileState = ETileState.Grabbed;
        }

        return true;
    }

    public bool TryPlaceTile()
    {
        if (grabbedTile != null)
        {
            if (!_gridManager.RegisterAndPlaceTile(grabbedTile)) 
            {
                return false;
            }

            tileTrayTiles.Remove(grabbedTile);
            grabbedTile.tileState = ETileState.Placed;

            OnSpaceOnTray?.Invoke();
            
            grabbedTile = null;
            return true;
        }

        return false;
    }

    public bool TryReleaseTile()
    {
        if (grabbedTile != null && grabbedTile.tileState == ETileState.Grabbed)
        {
            grabbedTile.tileState = ETileState.Neutral;
            grabbedTile = null;

            return true;
        }

        return false;
    }

    public bool TryMoveGrabbedTile()
    {
        if (grabbedTile == null)
        {
            return false;
        }
        
        RaycastHit rayHit;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out rayHit, Mathf.Infinity, LayerMask.GetMask("Grid"), QueryTriggerInteraction.Collide))
        {
            grabbedTile.transform.position = rayHit.point;
        }

        return true;
    }
}


