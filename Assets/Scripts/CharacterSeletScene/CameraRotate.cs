using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraRotate : MonoBehaviour
{
    float side;
    float ver;
    float speed = 2f;
    [SerializeField] GameObject mainCamera;
    [SerializeField] Vector3 cameraStartPos = new Vector3 (19f,180f,0);
    
   

    void Start()
    {

        mainCamera.transform.localEulerAngles = cameraStartPos;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine("ForStartCamera");
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

    IEnumerator ForStartCamera()
    {

        this.gameObject.GetComponent<CameraRotate>().enabled = false;

        yield return new WaitForSeconds(1f);

        this.gameObject.GetComponent<CameraRotate>().enabled = true;
    }

}
