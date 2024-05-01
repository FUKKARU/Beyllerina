using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGr_Scroll02 : MonoBehaviour
{
    [SerializeField] private GameObject BG01;
    [SerializeField] private float parallaxEffect00, RespownX = -15, DestroyX = 2;
    [SerializeField] private Vector3 RStarPos = new Vector3(-15f, -9.7f, -32f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        

        this.transform.position = new Vector3(BG01.transform.position.x + parallaxEffect00 * Time.deltaTime, BG01.transform.position.y, BG01.transform.position.z);

        if (transform.position.x >= RespownX)
        {
            //Debug.Log("•œŠˆƒ|ƒW“ž’…‚É‚á‚ñ");
            
        }

        if (transform.position.x >= DestroyX)
        {
            
            Destroy(this.gameObject);
            //Debug.Log("”j‰ó‚µ‚½‚µ‚ñ‚æ");

        }

        
    }
}
