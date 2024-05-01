using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGr_Respown : MonoBehaviour
{
    [SerializeField] private GameObject BG01;
    [SerializeField] private float parallaxEffect, ScaleRandomX, ScaleRandomY,  RespownX = -18f, DestroyX = 30f,   count = 0;
    [SerializeField] private Vector3 LStarPos;
    [SerializeField] private Vector3 RStarPos = new Vector3(-31f, -9.7f, -32f);
    [SerializeField]
    private bool isRespown;

    // Start is called before the first frame update
    void Start()
    {
        LStarPos = new Vector3(RStarPos.x, RStarPos.y, -RStarPos.z);
        
    }

    // Update is called once per frame
    void Update()
    {
        
        this.transform.position = new Vector3(BG01.transform.position.x + parallaxEffect * Time.deltaTime, BG01.transform.position.y, BG01.transform.position.z);

        if (this.transform.position.x >= RespownX)
        {




            if (count < 1 )
            {

                ScaleRandomX = Random.Range(1.0f, 1.8f);
                ScaleRandomY = Random.Range(1.0f, 1.8f);

                GameObject CloneGameObject = Instantiate(BG01, RStarPos, Quaternion.identity);
                GameObject CloneGameObjectL = Instantiate(BG01, LStarPos, Quaternion.identity);
                //Debug.Log("¶¬‚³‚ê‚½‚Ò‚å‚ñ");

                CloneGameObject.transform.localScale = new Vector3(ScaleRandomX, ScaleRandomY, 1);
                CloneGameObjectL.transform.localScale = new Vector3(ScaleRandomX, ScaleRandomY, 1);

                
                count = 2;

               
            }
            




        }

        if (transform.position.x >= DestroyX)
        {
            Destroy(this.gameObject);
        }
    }

  

}
