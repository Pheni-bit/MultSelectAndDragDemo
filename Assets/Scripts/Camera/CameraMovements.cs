using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovements : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;
    float verticalCameraSpeed = 20;
    float horizontalCameraSpeed = 20;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        //Debug.Log(verticalInput);
        if (verticalInput != 0)
        {
            gameObject.transform.Translate(Vector3.forward * Time.unscaledDeltaTime * verticalCameraSpeed * verticalInput);
        }
        if (horizontalInput != 0)
        {
            gameObject.transform.Translate(Vector3.right * Time.unscaledDeltaTime * horizontalCameraSpeed * horizontalInput);
        }
    }
    public void ScrollChange(float FieldOfView)
    {
        verticalCameraSpeed = 0;
        horizontalCameraSpeed = 0;
    }
}
