using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGr_Scroll : MonoBehaviour
{
    [SerializeField] private GameObject BG01, BG02, BG03, BG04;
    [SerializeField] private float parallaxEffect01, parallaxEffect02, parallaxEffect03;
    [SerializeField] private Vector3 LStarPos;
    [SerializeField] private Vector3 RStarPos;
    [SerializeField] private bool isRespown;

    void Start()
    {
        LStarPos = new Vector3(RStarPos.x, RStarPos.y, -RStarPos.z);
        
        
    }

    void Update()
    {
        if (isRespown == true)
        {
            Instantiate(BG01, LStarPos, Quaternion.identity);
            Instantiate(BG02, LStarPos, Quaternion.identity);
            Instantiate(BG03, LStarPos, Quaternion.identity);
            Instantiate(BG04, LStarPos, Quaternion.identity);
            Instantiate(BG01, RStarPos, Quaternion.identity);
            Instantiate(BG02, RStarPos, Quaternion.identity);
            Instantiate(BG03, RStarPos, Quaternion.identity);
            Instantiate(BG04, RStarPos, Quaternion.identity);
            isRespown = false;
        }
    }
    private void FixedUpdate()
    {
       

        
        BG01.transform.position = new Vector3(BG01.transform.position.x * Time.deltaTime, BG01.transform.position.y, BG01.transform.position.z);
        BG02.transform.position = new Vector3(BG01.transform.position.x * Time.deltaTime * parallaxEffect01, BG02.transform.position.y, BG02.transform.position.z);
        BG03.transform.position = new Vector3(BG01.transform.position.x * Time.deltaTime * parallaxEffect02, BG03.transform.position.y, BG03.transform.position.z);
        BG04.transform.position = new Vector3(BG01.transform.position.x * Time.deltaTime * parallaxEffect03, BG04.transform.position.y, BG04.transform.position.z);



       

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("BackGrounds"))
        {
            Destroy(BG01);
            Destroy(BG02);
            Destroy(BG03);
            Destroy(BG04);
            

            isRespown = true;
        }
    }
}
