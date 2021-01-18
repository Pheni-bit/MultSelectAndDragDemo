using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    // Start is called before the first frame update
    float cameraZoom;
    Camera mainCamera;
    float currentFOV;
    float zoomSpeed = 20;
    void Start()
    {
        mainCamera = this.gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        cameraZoom = Input.GetAxisRaw("Mouse ScrollWheel");
        if (cameraZoom != 0) 
        {
            currentFOV = mainCamera.fieldOfView;
            currentFOV -= cameraZoom * zoomSpeed;
            if (currentFOV < 90.0f && currentFOV > 6.0f)
            {
                mainCamera.fieldOfView = currentFOV;
            }
        }

    }
}
