using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class PlayerSelect : MonoBehaviour
{
    [SerializeField]
    private Transform Camera; //カメラの位置情報を取得
    [SerializeField]
    private GameObject ObjForScale, CharacterMenu;
    [SerializeField]
    private Vector3 Rotate = new Vector3(0, 50f, 0); //新しいベクトル３を定義
    



    private CharacterDate characterDate;
    

    private void Start()
    {
        
        characterDate = FindObjectOfType<CharaGameManager>().GetCharacterDate();
    }

    private void Update()
    {
        if (Input.GetButton("Cancel001"))
        {
            GameObject childUI = ObjForScale.transform.GetChild(1).gameObject;
           childUI.SetActive(false);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.name == "Select")
        {
            Debug.Log("当たっているよ！");
            //衝突した場合少し拡大
            ObjForScale.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            Debug.Log("選んでくれてありがとう！");

                
        }


        
    }

    private void OnCollisionExit(Collision collision)
    {
        

        //衝突しない場合元に戻る
        ObjForScale.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        Debug.Log("えっ...選んでくれないの");

    



    }

    private void OnCollisionStay(Collision collision)
    {
        GameObject childUI = ObjForScale.transform.GetChild(1).gameObject;
        
        if (Input.GetButton("Decide"))
        {
            
            Debug.Log("左クリック推したよ");

            
   　　//  this.transform.transform.position = Camera.transform.position + new Vector3(1f, -1.35f, 2.58f);

           
            Debug.Log("近づいてきた");


            childUI.SetActive(true);

            


        }

        this.gameObject.transform.Rotate(Rotate * Time.deltaTime);


    }

    //オブジェクトをcharacterに代入して、SetPlayerで設定したcharacter（CharacterDataScript)にcharacter（ここの変数）を代入した
    public void OnSlectCharacter1P(GameObject character01)
    {
        characterDate.SetPlayer01(character01);
  
        Debug.Log("これはおした");
    }
    
    public void OnSlectCharacter2P(GameObject character02)
    {
        characterDate.SetPlayer02(character02);

        Debug.Log("これはおしたの２P");
    }
    
}
