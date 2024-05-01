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
    private GameObject ObjForScale,  SpotLight01, SpotLight1P, SpotLight2P;
    [SerializeField]
    private Vector3 Rotate = new Vector3(0, 50f, 0); //yの値が一秒ごと回転する角度（つまり、あげれば回転速度が早くなる）
    [SerializeField]
    private Vector3 SpotLightVec = new Vector3();　//スポットライトのポジション
    [SerializeField]
    private Vector3 ObjStartSize = new Vector3(0.45f, 0.45f, 0.45f);　//もともとのオブジェクトのサイズ
    [SerializeField]
    private Vector3 ObjPlusSize = new Vector3(0.15f, 0.15f, 0.15f); //選択後のオブジェクトのサイズの増分
    [SerializeField]
    private float SpotLightHigh = 1.0f;

    bool isBallerinaSelected = false;



    private CharacterDate characterDate;


    [SerializeField] List<GameObject> CountObj = new List<GameObject>();
    bool changeSceneOnce;// ゲームシーンへの遷移は発動したか？

    private void Start()
    {
        changeSceneOnce = false;
        ObjForScale.transform.localScale = ObjStartSize;
        characterDate = FindObjectOfType<CharaGameManager>().GetCharacterDate();
        
    }


    IEnumerator CountAndStart()
    {
        CountObj[0].SetActive(true);
        yield return new WaitForSeconds(1);
        CountObj[0].SetActive(true);
        CountObj[1].SetActive(true);
        yield return new WaitForSeconds(1);
        CountObj[1].SetActive(true);
        CountObj[2].SetActive(true);
        yield return new WaitForSeconds(1);
        CountObj[2].SetActive(true);
        CountObj[3].SetActive(true);
        yield return new WaitForSeconds(0.3f);
        LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Game, true);
    }

    private void Update()
    {
        if (isBallerinaSelected && IA.InputGetter.Instance.IsSelect && !changeSceneOnce || Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CountAndStart());
            changeSceneOnce = true;
        }

        //if (Input.GetButton("Cancel001"))
        //{
            

        //    GameObject childUI = ObjForScale.transform.GetChild(1).gameObject;
        //    childUI.SetActive(false);

        //    Cursor.lockState = CursorLockMode.Locked;


        //}
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("MainCamera"))
        {
            if (gameObject.CompareTag("CharacterSelect_Ballerina"))
            {
                isBallerinaSelected = true;
            }
            SpotLightVec = new Vector3(ObjForScale.transform.position.x, ObjForScale.transform.position.y + SpotLightHigh, ObjForScale.transform.position.z);
            //Debug.Log("当たっているよ！");
           
            //衝突した場合少し拡大
            ObjForScale.transform.localScale = new Vector3(ObjStartSize.x + ObjPlusSize.x, ObjStartSize.y + ObjPlusSize.y, ObjStartSize.z + ObjPlusSize.z);
            SpotLight01.transform.position = SpotLightVec;
            //BaseSystem.SoundManager.Instance.PlaySE(0);
            //Debug.Log("選んでくれてありがとう！");

        
        }


        
    }

    private void OnCollisionExit(Collision collision)
    {
        if (gameObject.CompareTag("CharacterSelect_Ballerina"))
        {
            isBallerinaSelected = false;
        }
        SpotLightVec = new Vector3(0, 0, 0);
        //衝突しない場合元に戻る
        ObjForScale.transform.localScale = ObjStartSize;
        //Debug.Log("えっ...選んでくれないの");
        //BaseSystem.SoundManager.Instance.PlaySE(1);
        



    }

    private void OnCollisionStay(Collision collision)
    {
        GameObject childUI = ObjForScale.transform.GetChild(1).gameObject;
       
        
        if (Input.GetButton("Decide"))
        {
            
            //Debug.Log("左クリック推したよ");

            
   　　//  this.transform.transform.position = Camera.transform.position + new Vector3(1f, -1.35f, 2.58f);

           
            //Debug.Log("近づいてきた");


            childUI.SetActive(true);


            Cursor.lockState = CursorLockMode.None;

        }

        this.gameObject.transform.Rotate(Rotate * Time.deltaTime);


    }

    //オブジェクトをcharacterに代入して、SetPlayerで設定したcharacter（CharacterDataScript)にcharacter（ここの変数）を代入した
    public void OnSlectCharacter1P(GameObject character01)
    {
        characterDate.SetPlayer01(character01);
  
        //Debug.Log("これはおした");
    }
    
    public void OnSlectCharacter2P(GameObject character02)
    {
        characterDate.SetPlayer02(character02);

        //Debug.Log("これはおしたの２P");
    }

    public void OnCPU(GameObject CPU01)
    {
        characterDate.SetCPU01(CPU01);

        //Debug.Log("これはおしたの２P");
    }

    public void Light1P()
    {
        SpotLightVec = new Vector3(ObjForScale.transform.position.x, ObjForScale.transform.position.y + SpotLightHigh, ObjForScale.transform.position.z);
        SpotLight1P.transform.position = SpotLightVec;
    }

    public void Light2P()
    {
        SpotLightVec = new Vector3(ObjForScale.transform.position.x, ObjForScale.transform.position.y + SpotLightHigh, ObjForScale.transform.position.z);
        SpotLight2P.transform.position = SpotLightVec;
    }

}
