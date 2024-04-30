using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGr_Scroll : MonoBehaviour
{
    [SerializeField] private GameObject BG01, BG02, BG03, BG04, BG05;
    [SerializeField] private float parallaxEffect00,parallaxEffect01, parallaxEffect02, parallaxEffect03, parallaxEffect04, ScaleRandom, count;
    [SerializeField] private Vector3 LStarPos;
    [SerializeField] private Vector3 RStarPos;
    

    void Start()
    {


        LStarPos = new Vector3(RStarPos.x, RStarPos.y, -RStarPos.z);
        
        
    }

    void Update()
    {

       

        /*
        if (isRespown == true)
        {
            Instantiate(this, LStarPos, Quaternion.identity);
            this.transform.localScale = new Vector3(ScaleRandom, 1, 1);
            Instantiate(this, LStarPos + new Vector3(0, 0, 1), Quaternion.identity);
            Instantiate(this, LStarPos + new Vector3(0, 0, 2), Quaternion.identity);
            Instantiate(this, LStarPos + new Vector3(0, 0, 3), Quaternion.identity);
            
            isRespown = false;
        }
        */

        BG01.transform.position = new Vector3(BG01.transform.position.x + parallaxEffect00 * Time.deltaTime, BG01.transform.position.y, BG01.transform.position.z);
        BG02.transform.position = new Vector3(BG02.transform.position.x + parallaxEffect01 * Time.deltaTime, BG02.transform.position.y, BG02.transform.position.z);
        BG03.transform.position = new Vector3(BG03.transform.position.x + parallaxEffect02 * Time.deltaTime , BG03.transform.position.y, BG03.transform.position.z);
        BG04.transform.position = new Vector3(BG04.transform.position.x + parallaxEffect03 * Time.deltaTime , BG04.transform.position.y, BG04.transform.position.z);
        BG04.transform.position = new Vector3(BG05.transform.position.x + parallaxEffect04 * Time.deltaTime, BG05.transform.position.y, BG05.transform.position.z);

        if (transform.position.x >= 30f)
        {
            Destroy(this.gameObject);
            
        }
    }
   

    
}
