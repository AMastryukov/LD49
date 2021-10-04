using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileTray : MonoBehaviour
{
    public static Action OnTilePlaced;
    public static Action OnTileGrabbed;
    public static Action OnTileReleased;

    public bool IsEnabled { get; set; } = false;

    [SerializeField] private List<Transform> tilePlaceHolders;
    [SerializeField] private Transform tileSpawn;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Camera mainCamera;

    private int tileCapacity;
    private Tile grabbedTile;
    private List<Tile> tileTrayTiles;
    private GridManager _gridManager;

    public Vector3 SpawnPosition => tileSpawn.transform.position;
    public Tile GrabbedTile => grabbedTile;

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

        tile.transform.SetParent(transform);
        tile.transform.localScale = Vector3.one * 0.75f;

        tileTrayTiles.Add(tile);

        UpdateTilePositions();

        return true;
    }

    public bool TryGrabTile()
    {
        if (!IsEnabled || grabbedTile != null)
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
            hitTile.PlayGrabbedSound();
        }

        grabbedTile?.transform.DOKill();
        grabbedTile?.transform.DOScale(Vector3.one * 1f, 0.25f).SetEase(Ease.OutQuad);

        OnTileGrabbed?.Invoke();
        _gridManager.VisualizeRestrictions(grabbedTile);

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

            OnTilePlaced?.Invoke();
            
            grabbedTile = null;
            return true;
        }

        return false;
    }

    public bool TryReleaseTile()
    {
        if (grabbedTile != null && grabbedTile.tileState == ETileState.Grabbed)
        {
            grabbedTile.transform.DOScale(Vector3.one * 0.75f, 0.25f).SetEase(Ease.OutQuad);

            grabbedTile.tileState = ETileState.Neutral;
            grabbedTile = null;

            UpdateTilePositions(0.5f);

            OnTileReleased?.Invoke();
            _gridManager.VisualizeRestrictions();

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
            grabbedTile.transform.position = rayHit.point + Vector3.up * 0.75f;
        }

        return true;
    }

    private void UpdateTilePositions(float speed = 1f, Ease ease = Ease.OutQuad)
    {
        for (int i = 0; i < tileTrayTiles.Count; i++)
        {
            tileTrayTiles[i].transform.DOLocalMove(tilePlaceHolders[i].localPosition, speed).SetEase(ease);
        }
    }
}


