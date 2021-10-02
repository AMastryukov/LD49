using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private float cameraSpeed = 1;
    [SerializeField]
    private int pixelMargin = 5;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = mainCamera.ScreenToViewportPoint(Input.mousePosition);
        if (mousePos.x * mainCamera.pixelWidth < pixelMargin) transform.Translate(Vector3.left * Time.deltaTime * cameraSpeed, Space.World);
        if (mainCamera.pixelWidth - pixelMargin < mousePos.x * mainCamera.pixelWidth) transform.Translate(Vector3.right * Time.deltaTime * cameraSpeed, Space.World);
        if (mousePos.y * mainCamera.pixelHeight < pixelMargin) transform.Translate(Vector3.back * Time.deltaTime * cameraSpeed, Space.World);
        if (mainCamera.pixelHeight - pixelMargin < mousePos.y * mainCamera.pixelHeight) transform.Translate(Vector3.forward * Time.deltaTime * cameraSpeed, Space.World);

    }
}
