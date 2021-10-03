using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private bool moveCameraOnClick = false;
    [SerializeField] private float cameraSpeed = 1;
    [SerializeField] private int pixelMargin = 5;
    [SerializeField] private float cameraMaxX = 5;
    [SerializeField] private float cameraMinX = -5;
    [SerializeField] private float cameraMaxZ = 0;
    [SerializeField] private float cameraMinZ = -15;

    private bool _allowClickCameraMovement = false;
    private Vector3 _initialPos;
    private float _clickCameraSpeed;

    private void Start()
    {
        _clickCameraSpeed = cameraSpeed / 5;
    }

    void Update()
    {
        if (!moveCameraOnClick)
        {
            CameraMove();
        }
        else
        {
            CameraClickMovement();
        }

        CameraKeyMovement();

        //Ensures that the camera stays within the defined bounds
        MaintainCameraBoundaries();
    }

    void CameraMove()
    {
        Vector3 mousePos = mainCamera.ScreenToViewportPoint(Input.mousePosition);
        if (mousePos.x * mainCamera.pixelWidth < pixelMargin) 
            transform.Translate(Vector3.left * Time.deltaTime * cameraSpeed, Space.World);
        if (mainCamera.pixelWidth - pixelMargin < mousePos.x * mainCamera.pixelWidth) 
            transform.Translate(Vector3.right * Time.deltaTime * cameraSpeed, Space.World);
        if (mousePos.y * mainCamera.pixelHeight < pixelMargin) 
            transform.Translate(Vector3.back * Time.deltaTime * cameraSpeed, Space.World);
        if (mainCamera.pixelHeight - pixelMargin < mousePos.y * mainCamera.pixelHeight) 
            transform.Translate(Vector3.forward * Time.deltaTime * cameraSpeed, Space.World);
    }

    void CameraClickMovement()
    {
        if (_allowClickCameraMovement)
        {
            if (Input.GetMouseButton(2))
            {
                Vector3 movementVector = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 8.16f)) - _initialPos;
                Vector3 pos = transform.position;
                pos.z -= movementVector.z*(_clickCameraSpeed);
                pos.x -= movementVector.x*(_clickCameraSpeed);
                transform.position = Vector3.Lerp(transform.position, pos, _clickCameraSpeed);
            }
        }
        if (Input.GetMouseButtonDown(2))
        {
            _initialPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 8.16f));
            _allowClickCameraMovement = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            _allowClickCameraMovement = false;
        }
    }

    void CameraKeyMovement()
    {
        Vector3 move = new Vector3(0, 0, 0);
        Vector3 pos = transform.position;

        if (Input.GetKey("w"))
        {
            move.z+= cameraSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s"))
        {
            move.z -= cameraSpeed * Time.deltaTime;
        }

        if (Input.GetKey("d"))
        {
            move.x += cameraSpeed * Time.deltaTime;
        }

        if (Input.GetKey("a"))
        {
            move.x -= cameraSpeed * Time.deltaTime;
        }

        move = move.normalized * Time.deltaTime * cameraSpeed;
        pos += move;

        transform.position = Vector3.Lerp(transform.position, pos, cameraSpeed);
    }

    void MaintainCameraBoundaries()
    {
        Vector3 pos = mainCamera.transform.position;
        if(mainCamera.transform.position.x>cameraMaxX)
        {
            pos.x = cameraMaxX;
            mainCamera.transform.position = pos;
        }
        else if(mainCamera.transform.position.x < cameraMinX)
        {
            pos.x = cameraMinX;
            mainCamera.transform.position = pos;
        }

        if (mainCamera.transform.position.z > cameraMaxZ)
        {
            pos.z = cameraMaxZ;
            mainCamera.transform.position = pos;
        }
        else if (mainCamera.transform.position.z < cameraMinZ)
        {
            pos.z = cameraMinZ;
            mainCamera.transform.position = pos;
        }

        //Updating initial mouse position for ClickMovement
        if(moveCameraOnClick)
        {
            _initialPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 8.16f));
        }
    }
}
