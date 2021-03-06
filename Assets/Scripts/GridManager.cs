using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Really cool article here: https://www.redblobgames.com/grids/hexagons/#pixel-to-hex

/// <summary>
/// These are Hexagonal Axial coordinated.
/// Think of them like a Vector2 for a hexagonal grid.
/// </summary>
public struct Hex
{
    public int q { get; set; }
    public int r { get; set; }
    public Hex(int q, int r)
    {
        this.q = q;
        this.r = r;
    }
}

/// <summary>
/// Create a virtual grid that can be used to find vacant spots and place tiles.
/// Ideally call GetAllValidSpots() to get a hex coordinate for the grid.
/// This hex coordinate can then be passed to RegisterAndPlaceTile() to place a tile on the grid and mark the spot as occupied.
/// </summary>
public class GridManager : MonoBehaviour
{
    public static Action OnTilePlacementConfirmed;
    public static Action OnTileEnterHex;
    public static Action OnTileLeaveHex;

    public bool IsTileHoveringAboveValidHex = false;
    public Tile LastTilePlaced;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float gridSpacing = 1.5f;
    [SerializeField] private float snapRange = 8f;

    private Dictionary<Hex, Tile> gridOccupancy;

    [SerializeField]
    private List<Tile> tilePrefabs;
    //private Dictionary<Hex, string> gridRestrictions;

    [SerializeField] private GameObject emptyTilePrefab;
    [SerializeField] private GameObject previewTilePrefab;
    [SerializeField] private GameObject restrictedTilePrefab;
    [SerializeField] private GameObject dustParticle;
    [SerializeField] private GameObject destructionParticle;

    private GameObject previewTile;
    private List<GameObject> restrictedTiles;
    private TileTray _tileTray;
    private AudioManager _audioManager;

    /// <summary>
    /// Getall the tile that have been placed on the grid.
    /// This is Useful if you need to make calculations beased off of
    /// values associated with the tiles.
    /// </summary>
    public List<Tile> GetTiles()
    {
        return new List<Tile>(gridOccupancy.Values);
    }

    private void Awake()
    {
        if(mainCamera == null)
        {
            Debug.LogError("Missing main camera reference");
        }

        gridOccupancy = new Dictionary<Hex, Tile>();
        //gridRestrictions = new Dictionary<Hex, string>();
        restrictedTiles = new List<GameObject>();

        _tileTray = FindObjectOfType<TileTray>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    public Vector3 HexToPoint(Hex hex)
    {
        Vector3 zVec = new Vector3(0, 0, 1) * gridSpacing;
        Vector3 xVec = new Vector3(Mathf.Sqrt(3) / 2, 0, 1f / 2) * gridSpacing;

        return (hex.q * zVec) + (hex.r * xVec);
    }

    public bool IsValidPlacement(Hex hex, Tile tile)
    {
        // Check logic here
        List<Tile> neighbors = GetNeighbors(hex);

        foreach (Tile neighbor in neighbors)
        {
            if (neighbor.Type == tile.Type && tile.Type != ETileType.None)
            {
                return false;
            }
        }

        return true;
    }

    //public void UpdateRestrictions(Hex hex, Tile tile)
    //{
    //    //First clear the restriction on this hex
    //    if(gridRestrictions.ContainsKey(hex)) gridRestrictions.Remove(hex);

    //    //check neighbors and add random restrictions for them if they arent already restricted
    //    List<Hex> neighbors = GetNeighborsHex(hex);
    //    foreach(Hex n in neighbors)
    //    {
    //        if (isEmptySpot(n) && !gridRestrictions.ContainsKey(n))
    //        {
    //            gridRestrictions.Add(n, tilePrefabs[UnityEngine.Random.Range(0, tilePrefabs.Count)].Name);
    //        }
    //    }

    //    VisualizeRestrictions();
    //}

    public void EndVisualizeRestrictions()
    {
        foreach (GameObject obj in restrictedTiles)
        {
            Destroy(obj);
        }

        restrictedTiles.Clear();
    }

    public void VisualizeRestrictions(Tile tile = null)
    {
        EndVisualizeRestrictions();

        if (tile == null)
        {
            foreach (Hex spot in GetAllValidSpots())
            {
                GameObject newHex = Instantiate(emptyTilePrefab, HexToPoint(spot), Quaternion.identity, transform);
                restrictedTiles.Add(newHex);
            }
        }
        else
        {
            foreach (Hex spot in GetAllValidSpots())
            {
                GameObject newHex;

                if (IsValidPlacement(spot, tile))
                {
                    newHex = Instantiate(previewTilePrefab, HexToPoint(spot), Quaternion.identity, transform);
                }
                else
                {
                    newHex = Instantiate(restrictedTilePrefab, HexToPoint(spot), Quaternion.identity, transform);
                }

                restrictedTiles.Add(newHex);
            }
        }
    }

    public void ReplaceTile(Tile tile, Tile replacement)
    {
        Vector3 initialDisplacement = Vector3.down;
        replacement.transform.position = tile.transform.position + initialDisplacement;
        replacement.transform.rotation = tile.transform.rotation;

        replacement.transform.DOMove(tile.transform.position, 2);

        var hex = GetHexCoordinates(tile);

        RemoveTile(tile);

        replacement.TileState = ETileState.Placed;
        gridOccupancy.Add(hex, replacement);
    }

    public void RemoveTile(Tile tile)
    {
        gridOccupancy.Remove(GetHexCoordinates(tile));
        
        
        tile.transform.DOShakePosition(10, 0.03f * new Vector3(1, 0, 1), 100);
        tile.transform.DOMoveY(-2, 10).OnComplete(() =>
        {
            

            Destroy(tile.gameObject);
            
        });

        Destroy(Instantiate(destructionParticle, tile.transform.position, Quaternion.Euler(-90, 0, 0)), 5f);
        _audioManager.PlaySound(AudioManager.Sounds.TileDestory);




    }

    /// <summary>
    /// Given a Tile and a hex coordinate (returned by lots of functions in this class), register the tile in the grid
    /// so that:
    /// 
    /// 1. The grid knows that the spot is occupied
    /// 2. The grid knows who occupies that spot
    /// </summary>
    /// <param name="tileObject">tile to place on the grid</param>
    /// <param name="hex">hexagonal coordinate</param>
    public bool RegisterAndPlaceTile(Tile tileObject, Hex hex, bool silent = false)
    {
        if (gridOccupancy.ContainsKey(hex))
        {
            Debug.LogError("A tile already exists here");
            return false;
        }
        else
        {
            LastTilePlaced = tileObject;

            if (!IsValidPlacement(hex, tileObject))
            {
                // This could be sketch because the game manager doesn't know these values are changing
                tileObject.Pillars.Culture = 0;
                tileObject.Pillars.Economy = 0;
                tileObject.Pillars.Military = 0;
            }

            tileObject.transform.SetParent(transform, true);

            tileObject.transform.DORotate(new Vector3(0, 60 * UnityEngine.Random.Range(6, 11), 0), 0.25f).SetEase(Ease.OutCirc);
            tileObject.transform.DOMove(HexToPoint(hex) + Vector3.up * 0.75f, 0.1f).SetEase(Ease.OutQuad)
                .OnComplete(()=>
                {
                    tileObject.transform.DOMove(HexToPoint(hex), 0.35f).SetEase(Ease.InCirc)
                    .OnComplete(()=>
                    {   
                        Destroy(Instantiate(dustParticle, HexToPoint(hex), Quaternion.Euler(-90, 0, 0)), 5f);
                        if (!silent)
                        {
                            _audioManager.PlaySound(AudioManager.Sounds.TilePlace);
                        }

                    });
                });


            tileObject.TileState = ETileState.Placed;
            gridOccupancy.Add(hex, tileObject);

            if (!silent)
            {
                OnTilePlacementConfirmed?.Invoke();
            }

            EndPreview();
            VisualizeRestrictions(null);
        }

        return true;
    }

    public bool RegisterAndPlaceTile(Tile tileObject)
    {
        if (previewTile != null)
        {
            return RegisterAndPlaceTile(tileObject, ClosestValidHex(previewTile.transform.position));
        }
        
        return false;
    }

    /// <summary>
    /// Find all the neighboring COORDINATES of a hex coordinate. May or may not be valid for placement.
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public List<Hex> GetNeighborsHex(Hex hex)
    {
        List<Hex> neighbors = new List<Hex>();
        neighbors.Add(new Hex(hex.q + 1, hex.r));
        neighbors.Add(new Hex(hex.q - 1, hex.r));
        neighbors.Add(new Hex(hex.q, hex.r + 1));
        neighbors.Add(new Hex(hex.q, hex.r - 1));
        neighbors.Add(new Hex(hex.q + 1, hex.r - 1));
        neighbors.Add(new Hex(hex.q - 1, hex.r + 1));
        return neighbors;
    }

    /// <summary>
    /// Gets the neighboring tiles for a given HEX COORDINATE.
    /// THIS HAS NOT BEEN OPTIMIZED SO DO NOT PUT IN UPDATE LOOP.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public List<Tile> GetNeighbors(Hex hex) 
    {
        List<Hex> neighbors = GetNeighborsHex(hex);
        List<Tile> neighborTiles = new List<Tile>();

        Tile neighborTile;

        foreach (Hex nex in neighbors)
        {
            if (gridOccupancy.TryGetValue(nex, out neighborTile))
            {
                neighborTiles.Add(neighborTile);
            }
        }

        return neighborTiles;
    }

    /// <summary>
    /// Gets the neighboring tiles for a given tile.
    /// THIS HAS NOT BEEN OPTIMIZED SO DO NOT PUT IN UPDATE LOOP.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public List<Tile> GetNeighbors(Tile tile)
    {
        Hex hex = GetHexCoordinates(tile);
        return GetNeighbors(hex);
    }

    /// <summary>
    /// Find the hex coordinate of a given hex tile on the grid. Throws and error if the Tile does not exist.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public Hex GetHexCoordinates(Tile tile)
    {
        foreach (Hex key in gridOccupancy.Keys)
        {
            if (gridOccupancy[key] == tile)
            {
                return key;
            }
        }

        throw new KeyNotFoundException();
    }

    /// <summary>
    /// Check if this hex spot on the grid contains a tile
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public bool isOccupiedSpot(Hex hex)
    {
        return gridOccupancy.ContainsKey(hex);
    }

    /// <summary>
    /// Check if this hex spot on the grid does not contain a tile
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public bool isEmptySpot(Hex hex)
    {
        return !isOccupiedSpot(hex);
    }

    /// <summary>
    /// Find all valid hex spots available for tile placement.
    /// Good idea to call this before calling RegisterAndPlaceTile().
    /// </summary>
    /// <returns></returns>
    public List<Hex> GetAllValidSpots()
    {
        // This data structure is weird please ignore
        Dictionary<Hex, Hex> validSpots = new Dictionary<Hex, Hex>();

        if (gridOccupancy.Count == 0)
        {
            // If grid is emtpy, the only valid spot is the origin
            validSpots.Add(new Hex(0, 0), new Hex(0, 0));
        }
        else
        {
            foreach (Hex hex in gridOccupancy.Keys)
            {
                List<Hex> neighbors = GetNeighborsHex(hex);

                foreach (Hex neighbor in neighbors)
                {
                    if (!validSpots.ContainsKey(neighbor) && isEmptySpot(neighbor))
                    {
                        validSpots.Add(neighbor, neighbor);
                    }
                }
            }
        }
        
        return new List<Hex>(validSpots.Values);
    }

    /// <summary>
    /// Find the closest hex coordinate to a real world point.
    /// This is useful when selecting tiles.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Hex ClosestHex(Vector3 point)
    {
        float dist = Mathf.Infinity;
        Hex closest = new Hex(0, 0);

        foreach (Hex hex in gridOccupancy.Keys)
        {
            float new_dist = Vector3.Distance(point, HexToPoint(hex));

            if ( new_dist < dist)
            {
                dist = new_dist;
                closest = hex;
            }
        }

        return closest;
    }

    /// <summary>
    /// Given a point in world space, returns the hex coordinate of the closest valid/empty spot on the grid.
    /// This is useful when figuring out where the tile will be placed.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Hex ClosestValidHex(Vector3 point)
    {
        float dist = Mathf.Infinity;
        Hex closest = new Hex(0, 0);

        foreach (Hex hex in GetAllValidSpots())
        {
            if (!isEmptySpot(hex)) continue;

            float new_dist = Vector3.Distance(point, HexToPoint(hex));
            
            if (new_dist < dist)
            {
                dist = new_dist;
                closest = hex;
            }
        }

        return closest;
    }

    public void PreviewTile()
    {
        RaycastHit rayHit;
        
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out rayHit, Mathf.Infinity, LayerMask.GetMask("Grid"), QueryTriggerInteraction.Collide))
        {            
            Hex hex = ClosestValidHex(rayHit.point);
            Vector3 newPos = HexToPoint(hex);

            if (Vector3.Distance(rayHit.point, newPos) < snapRange)
            {
                if (previewTile == null)
                {
                    previewTile = Instantiate(previewTilePrefab, rayHit.point, Quaternion.identity, transform);
                }

                IsTileHoveringAboveValidHex = IsValidPlacement(hex, _tileTray.GrabbedTile);

                if (previewTile.transform.position != newPos)
                {
                    OnTileEnterHex?.Invoke();
                }

                previewTile.transform.position = newPos;
            }
            else
            {
                EndPreview();
            }
        }
        else
        {
            EndPreview();
        }
    }

    public void EndPreview()
    {
        if (previewTile != null)
        {
            IsTileHoveringAboveValidHex = false;

            Destroy(previewTile);
            previewTile = null;

            OnTileLeaveHex?.Invoke();
        }
    }
}