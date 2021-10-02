using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private GameObject grid;
    [SerializeField]
    private float gridSpacing = 1;
    [SerializeField]
    private GameObject hexTile;

    private Dictionary<Hex, GameObject> gridOccupancy;
    private GameObject previewTile;
    // Start is called before the first frame update

    private void Awake()
    {
        gridOccupancy = new Dictionary<Hex, GameObject>();
    }

    void Start()
    {
        

        GenerateHex(0, 0);
        List<Hex> neighbors = GetNeighbors(new Hex(0, 0));

        //foreach (Hex hex in neighbors)
        //{
        //    GenerateHex(hex);

        //}

        List<Hex> validSpots = GetAllValidSpots();

        foreach (Hex hex in validSpots)
        {
            GenerateHex(hex);
            //print(hex.q + " " + hex.r);

        }

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit rayHit;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out rayHit, Mathf.Infinity, ~grid.layer, QueryTriggerInteraction.Collide)
            && rayHit.transform.tag == grid.tag)
        {
            //print("Hit");
            //Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * rayHit.distance, Color.green);
            if (previewTile == null)
            {
                previewTile = Instantiate(hexTile, rayHit.point, Quaternion.identity, transform);
            }
            else
            {
                Hex hex = ClosestValidHex(rayHit.point);
                previewTile.transform.position = HexToPoint(hex);
                if (Input.GetMouseButtonDown(0))
                {
                    GenerateHex(hex);
                    previewTile = null;
                }
            }
        }
        else
        {
            //print("NoHit");
            Destroy(previewTile);
            previewTile = null;
        }

        
        //print(mainCamera.ScreenToWorldPoint(Input.mousePosition));

    }
    // Really cool article here: https://www.redblobgames.com/grids/hexagons/#pixel-to-hex

    private Vector3 HexToPoint(Vector2Int hex)
    {
        Vector3 zVec = new Vector3(0, 0, 1) * gridSpacing;
        Vector3 xVec = new Vector3(Mathf.Sqrt(3) / 2, 0, 1f / 2) * gridSpacing;
        
        return (hex.x * zVec) + (hex.y * xVec);
    }

    private Vector3 HexToPoint(Hex hex)
    {
        Vector3 zVec = new Vector3(0, 0, 1) * gridSpacing;
        Vector3 xVec = new Vector3(Mathf.Sqrt(3) / 2, 0, 1f / 2) * gridSpacing;

        return (hex.q * zVec) + (hex.r * xVec);
    }

    private void GenerateHex(int q, int r)
    {
        GenerateHex(new Hex(q, r));
    }

    private void GenerateHex(Hex hex)
    {
        GameObject tile = Instantiate(hexTile, HexToPoint(new Vector2Int(hex.q, hex.r)), Quaternion.identity);
        if (gridOccupancy.ContainsKey(hex))
        {
            Debug.LogError("A tile already exists here");
        }
        else
        {
            gridOccupancy.Add(hex, tile);
        }
        
    }

    private List<Hex> GetNeighbors(Hex hex)
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


    private bool isOccupiedSpot(Hex hex)
    {
        return gridOccupancy.ContainsKey(hex);
    }
    private bool isEmptySpot(Hex hex)
    {
        return !isOccupiedSpot(hex);
    }

    private List<Hex> GetAllValidSpots()
    {

        Dictionary<Hex, Hex> validSpots = new Dictionary<Hex, Hex>();
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
        return new List<Hex>(validSpots.Values);
    }

    private Hex ClosestHex(Vector3 point)
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

    private Hex ClosestValidHex(Vector3 point)
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