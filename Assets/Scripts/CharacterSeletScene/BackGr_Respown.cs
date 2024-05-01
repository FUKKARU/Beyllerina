using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGr_Respown : MonoBehaviour
{
    [SerializeField] private GameObject BG01, BG02, BG03, BG04;
    [SerializeField] private float parallaxEffect, ScaleRandomX, ScaleRandomY, RespownX = -18f, DestroyX = 30f, PosRandomX; //,   count = 0;
    [SerializeField] private Vector3 LStarPos;
    [SerializeField] private Vector3 RStarPos = new Vector3(-31f, -9.7f, -32f), RStartPos_Uekomi;
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
        Vector3 LStartPos_Uekomi = new Vector3(RStartPos_Uekomi.x, RStartPos_Uekomi.y, -RStartPos_Uekomi.z);
        GameObject[] Power_respown = GameObject.FindGameObjectsWithTag("BackGroundsTree");
        GameObject[] Uekomi_respown = GameObject.FindGameObjectsWithTag("BackGroundsUekomi");
        GameObject[] OkunoTree_respown = GameObject.FindGameObjectsWithTag("BackGroundsOkunoTree");
        GameObject[] MountainShadow_respown = GameObject.FindGameObjectsWithTag("BackGroundsMShadow");
        this.transform.position = new Vector3(BG01.transform.position.x + parallaxEffect * Time.deltaTime, BG01.transform.position.y, BG01.transform.position.z);

        if (this.transform.position.x >= RespownX)
        {

            if (MountainShadow_respown.Length == 2)
            {

                Instantiate(BG04, RStartPos_Uekomi + new Vector3(65,-6.7f,0), Quaternion.identity);
                Instantiate(BG04, LStartPos_Uekomi + new Vector3(65,-6.7f,0), Quaternion.identity);

            }

            if (OkunoTree_respown.Length ==2)
            {

                Instantiate(BG03, RStartPos_Uekomi, Quaternion.identity);
                Instantiate(BG03, LStartPos_Uekomi, Quaternion.identity);

            }

            if (Uekomi_respown.Length == 2)
            {
                Instantiate(BG02, RStartPos_Uekomi , Quaternion.identity);
                Instantiate(BG02, LStartPos_Uekomi, Quaternion.identity);
            }
            
            //もし、“PowerUp”がついているオブジェクトの数＝２ならば
            if (Power_respown.Length <= 12)
            {
                for (int i = 0; i < 1; i++)
                {
                    ScaleRandomX = Random.Range(1.0f, 1.8f);
                    ScaleRandomY = Random.Range(1.0f, 1.6f);
                    PosRandomX = Random.Range(1.5f, 2.0f);
                    GameObject Power_respawn01 = Instantiate(BG01, RStarPos, Quaternion.identity);
                    GameObject Power_respawn02 = Instantiate(BG01, RStarPos - new Vector3(2 + PosRandomX,0,0), Quaternion.identity);
                    GameObject Power_respawn03 = Instantiate(BG01, RStarPos - new Vector3(5 + PosRandomX, 0, 0), Quaternion.identity);
                    Instantiate(BG01, RStarPos - new Vector3(7 + PosRandomX, 0, 0), Quaternion.identity);
                    GameObject Power_respawn01L = Instantiate(BG01, LStarPos, Quaternion.identity);
                    GameObject Power_respawn02L = Instantiate(BG01, LStarPos - new Vector3(2 + PosRandomX, 0, 0), Quaternion.identity);
                    GameObject Power_respawn03L = Instantiate(BG01, LStarPos - new Vector3(5 + PosRandomX, 0, 0), Quaternion.identity);
                    Instantiate(BG01, LStarPos - new Vector3(7 + PosRandomX, 0, 0), Quaternion.identity);

                    Power_respawn01.transform.localScale = new Vector3(ScaleRandomX, ScaleRandomY, 1);
                    Power_respawn01L.transform.localScale = new Vector3(ScaleRandomX, ScaleRandomY, 1);
                    Power_respawn02.transform.localScale = new Vector3(ScaleRandomX, ScaleRandomY, 1);
                    Power_respawn02L.transform.localScale = new Vector3(ScaleRandomX, ScaleRandomY, 1);
                    Power_respawn03.transform.localScale = new Vector3(ScaleRandomX, ScaleRandomY, 1);
                    Power_respawn03L.transform.localScale = new Vector3(ScaleRandomX, ScaleRandomY, 1);

                }
            }

            /*
            if (count < 1 )
            {

                

                GameObject CloneGameObject = Instantiate(BG01, RStarPos, Quaternion.identity);
                GameObject CloneGameObjectL = Instantiate(BG01, LStarPos, Quaternion.identity);
                //Debug.Log("生成されたぴょん");

                CloneGameObject.transform.localScale = new Vector3(ScaleRandomX, ScaleRandomY, 1);
                CloneGameObjectL.transform.localScale = new Vector3(ScaleRandomX, ScaleRandomY, 1);

                
                count = 2;

               
            }
            
            */



        }

        if (transform.position.x >= DestroyX)
        {
            Destroy(this.gameObject);
        }
    }

  

}
