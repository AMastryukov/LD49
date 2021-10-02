using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private GameObject grid;
    [SerializeField]
    private float gridSpacing = 1.5f;

    private Dictionary<Hex, Tile> gridOccupancy;

    public static Action OnTilePlaced;


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
        gridOccupancy = new Dictionary<Hex, Tile>();
    }

    void Start()
    {
        //GenerateHex(0, 0);
        return;
        
        List<Hex> neighbors = GetNeighbors(new Hex(0, 0));

        //foreach (Hex hex in neighbors)
        //{
        //    GenerateHex(hex);

        //}

        List<Hex> validSpots = GetAllValidSpots();

        foreach (Hex hex in validSpots)
        {
            //RegisterAndPlaceTile(hex);
            //print(hex.q + " " + hex.r);

        }

    }

    // Update is called once per frame
    //void Update()
    //{
    //    return;
    //    RaycastHit rayHit;
    //    if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out rayHit, Mathf.Infinity, ~grid.layer, QueryTriggerInteraction.Collide)
    //        && rayHit.transform.tag == grid.tag)
    //    {
    //        //print("Hit");
    //        //Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * rayHit.distance, Color.green);
    //        if (previewTile == null)
    //        {
    //            previewTile = Instantiate(hexTile, rayHit.point, Quaternion.identity, transform);
    //        }
    //        else
    //        {
    //            Hex hex = ClosestValidHex(rayHit.point);
    //            previewTile.transform.position = HexToPoint(hex);
    //            if (Input.GetMouseButtonDown(0))
    //            {
    //                //RegisterAndPlaceTile(hex);
    //                previewTile = null;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        //print("NoHit");
    //        Destroy(previewTile);
    //        previewTile = null;
    //    }

        
    //    //print(mainCamera.ScreenToWorldPoint(Input.mousePosition));

    //}


    public Vector3 HexToPoint(Vector2Int hex)
    {
        Vector3 zVec = new Vector3(0, 0, 1) * gridSpacing;
        Vector3 xVec = new Vector3(Mathf.Sqrt(3) / 2, 0, 1f / 2) * gridSpacing;
        
        return (hex.x * zVec) + (hex.y * xVec);
    }

    public Vector3 HexToPoint(Hex hex)
    {
        Vector3 zVec = new Vector3(0, 0, 1) * gridSpacing;
        Vector3 xVec = new Vector3(Mathf.Sqrt(3) / 2, 0, 1f / 2) * gridSpacing;

        return (hex.q * zVec) + (hex.r * xVec);
    }

    //public void RegisterAndPlaceTile(int q, int r)
    //{
    //    RegisterAndPlaceTile(new Hex(q, r));
    //}

    //public void RegisterAndPlaceTile(Hex hex)
    //{
    //    RegisterAndPlaceTile(hex, this.hexTile);
    //}

    /// <summary>
    /// Given a Tile and a hex coordinate (returned by lots of functions in this class), register the tile in the grid
    /// so that:
    /// 
    /// 1. The grid knows that the spot is occupied
    /// 2. The grid knows who occupies that spot
    /// </summary>
    /// <param name="hex">hexagonal coordinate</param>
    /// <param name="tileObject">tile to place on the grid</param>
    public void RegisterAndPlaceTile(Hex hex, Tile tileObject)
    {

        if (gridOccupancy.ContainsKey(hex))
        {
            Debug.LogError("A tile already exists here");
        }
        else
        {
            tileObject.transform.position = HexToPoint(hex);
            gridOccupancy.Add(hex, tileObject);
            OnTilePlaced?.Invoke();
        }
        
    }

    /// <summary>
    /// Find all the neighboring COORDINATES of a hex coordinate. May or may not be valid for placement.
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public List<Hex> GetNeighbors(Hex hex)
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
    /// Gets the neighboring tiles for a given tile.
    /// THIS HAS NOT BEEN OPTIMIZED SO DO NOT PUT IN UPDATE LOOP.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public List<Tile> GetNeighbors(Tile tile)
    {
        List<Hex> neighbors = GetNeighbors(GetHexCoordinates(tile));
        List<Tile> neighborTiles = new List<Tile>();

        Tile neighborTile;
        foreach(Hex hex in neighbors)
        {
            if(gridOccupancy.TryGetValue(hex, out neighborTile)){
                neighborTiles.Add(neighborTile);
            }
        }
        return neighborTiles;
    }

    /// <summary>
    /// Find the hex coordinate of a given hex tile on the grid. Throws and error if the Tile does not exist.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public Hex GetHexCoordinates(Tile tile)
    {
        foreach(Hex key in gridOccupancy.Keys)
        {
            if(gridOccupancy[key] = tile)
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
                List<Hex> neighbors = GetNeighbors(hex);

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
        foreach(Hex hex in gridOccupancy.Keys)
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

}