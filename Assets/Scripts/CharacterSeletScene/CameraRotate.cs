using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Rendering.CameraUI;



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
    float additionalPower;
    [SerializeField] GameObject angleDetector;
    void Update()
    {


        Physics.Raycast(angleDetector.transform.position, Vector3.up, out RaycastHit hit, 6.0f);
        Vector2 val = IA.InputGetter.Instance.ValueDirection;
        float h = -val.x;
        float v = -val.y;
        float div = 5;
        if (Vector3.Distance(angleDetector.transform.position, hit.transform.gameObject.transform.position) > 2)
        {
            additionalPower -= (Vector3.Distance(angleDetector.transform.position, hit.transform.gameObject.transform.position) - 2 ) / div;
        }
        else if(Vector3.Distance(angleDetector.transform.position, hit.transform.gameObject.transform.position) < 1.5f)
        {
            additionalPower += (1.5f - Vector3.Distance(angleDetector.transform.position, hit.transform.gameObject.transform.position)) / div;
        }
        else
        {
            additionalPower = 0;
        }
        side += h * speed;
        ver += (v + additionalPower)*speed;
        // Debug.Log(Input.GetAxis(gMouse Xh));


        transform.rotation = Quaternion.Euler(ver, -side, 0f);
    }

    IEnumerator ForStartCamera()
    {

        this.gameObject.GetComponent<CameraRotate>().enabled = false;

        yield return new WaitForSeconds(1f);

        this.gameObject.GetComponent<CameraRotate>().enabled = true;
    }

}
