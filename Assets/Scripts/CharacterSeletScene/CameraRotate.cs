using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraRotate : MonoBehaviour
{
    float side;
    float ver;
    float speed = 2f;
    
   

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    
    void Update()
    {
        // Debug.Log(Input.GetAxis(ÅgMouse XÅh));
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");
        side += h * speed;
        ver += v * speed;

        transform.rotation = Quaternion.Euler(ver, -side, 0f);
    }

    

    
}
