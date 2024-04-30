using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class photo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ScreenCapture.CaptureScreenshot("enemyface.png");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
