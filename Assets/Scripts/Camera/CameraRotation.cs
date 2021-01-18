using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    float rotate;
    float rotateSpeed = 40;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotate = Input.GetAxisRaw("Rotate");
        //Debug.Log(rotate);
        if (rotate != 0)
        {
            this.transform.Rotate(new Vector3(0,rotate,0) * Time.unscaledDeltaTime * rotateSpeed);
        }
    }
}
