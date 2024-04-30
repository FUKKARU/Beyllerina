using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGr_Scroll : MonoBehaviour
{
    [SerializeField] private GameObject BG01, BG02, BG03, BG04;
    [SerializeField] private float parallaxEffect01, parallaxEffect02, parallaxEffect03, PlusPos;
    [SerializeField] private Vector3 LStarPos;
    [SerializeField] private Vector3 RStarPos;
    [SerializeField] private bool isRespown;

    void Start()
    {
        LStarPos = new Vector3(RStarPos.x, RStarPos.y, -RStarPos.z);
        
        
    }

    void Update()
    {

        float ScrollTime = Time.deltaTime * parallaxEffect01;
        if (isRespown == true)
        {
            Instantiate(BG01, LStarPos, Quaternion.identity);
            Instantiate(BG02, LStarPos + new Vector3(0, 0, 1), Quaternion.identity);
            Instantiate(BG03, LStarPos + new Vector3(0, 0, 2), Quaternion.identity);
            Instantiate(BG04, LStarPos + new Vector3(0, 0, 3), Quaternion.identity);
            Instantiate(BG01, RStarPos, Quaternion.identity);
            Instantiate(BG02, RStarPos + new Vector3(0, 0, -1), Quaternion.identity);
            Instantiate(BG03, RStarPos + new Vector3(0, 0, -2), Quaternion.identity);
            Instantiate(BG04, RStarPos + new Vector3(0, 0, -3), Quaternion.identity);
            isRespown = false;
        }

        BG01.transform.position = new Vector3(BG01.transform.position.x + PlusPos * Time.deltaTime, BG01.transform.position.y, BG01.transform.position.z);
        BG02.transform.position = new Vector3(BG02.transform.position.x + parallaxEffect01 * Time.deltaTime, BG02.transform.position.y, BG02.transform.position.z);
        BG03.transform.position = new Vector3(BG03.transform.position.x + parallaxEffect02 * Time.deltaTime , BG03.transform.position.y, BG03.transform.position.z);
        BG04.transform.position = new Vector3(BG04.transform.position.x + parallaxEffect03 * Time.deltaTime , BG04.transform.position.y, BG04.transform.position.z);
    }
   

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("BackGrounds"))
        {
            Destroy(BG01);
            
            

            isRespown = true;
        }
    }
}
