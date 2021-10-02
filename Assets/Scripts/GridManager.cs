using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private GameObject previewTile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit rayHit;
        if(Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out rayHit, Mathf.Infinity, ~grid.layer, QueryTriggerInteraction.Collide))
        {
            //print("Hit");
            //Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * rayHit.distance, Color.green);
            if(previewTile == null){
                previewTile = Instantiate(hexTile, rayHit.point, Quaternion.identity, transform);
            }
            else
            {
                previewTile.transform.position = HexToPoint(PointToHex(rayHit.point));
                print(PointToHex(rayHit.point));
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
    private Vector2Int PointToHex(Vector3 position) {
        float q = ((2 / 3) * position.x) / gridSpacing;
        float r = (((-1 / 3) * position.x) + (Mathf.Sqrt(3)/3) * position.z) / gridSpacing;
        return new Vector2Int(Mathf.RoundToInt(q), Mathf.RoundToInt(r));
    }
    //function pixel_to_flat_hex(point):
    //var q = (2./ 3 * point.x) / size
    //var r = (-1./ 3 * point.x + sqrt(3) / 3 * point.y) / size
    //return hex_round(Hex(q, r))

    private Vector3 HexToPoint(Vector2Int hex)
    {
        float x = gridSpacing * (Mathf.Sqrt(3) * hex.x + (Mathf.Sqrt(3) / 2) * hex.y);
        float z = gridSpacing * (3 / 2) * hex.y;
        return new Vector3(x, grid.transform.position.y, z);
    }
    //function pointy_hex_to_pixel(hex):
    //var x = size * (sqrt(3) * hex.q + sqrt(3) / 2 * hex.r)
    //var y = size * (3./ 2 * hex.r)
    //return Point(x, y)
}
