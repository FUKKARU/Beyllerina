using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JstForTest : MonoBehaviour
{
    [SerializeField]
    private Camera camera_object; //カメラを取得
    [SerializeField]
    private float RaycastDistance = 30.0f; 
    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private GameObject ObjForScale, SpotLight1P, SpotLight2P;
    [SerializeField]
    private Collision CameraCollision;
    [SerializeField]
    private Vector3 Rotate = new Vector3(0, 50f, 0); //yの値が一秒ごと回転する角度（つまり、あげれば回転速度が早くなる）
    [SerializeField]
    private Vector3 SpotLightVec = new Vector3();　//スポットライトのポジション
    [SerializeField]
    private Vector3 ObjStartSize = new Vector3(0.45f, 0.45f, 0.45f);　//もともとのオブジェクトのサイズ
   
    



    private CharacterDate characterDate;
    private RaycastHit hit; //レイキャストが当たったものを取得する入れ物

    void Start()
    {
        ObjForScale.transform.localScale = ObjStartSize;
        characterDate = FindObjectOfType<CharaGameManager>().GetCharacterDate();
    }
    void Update()
    {
        Ray ray = camera_object.ScreenPointToRay(Input.mousePosition);
        
        

        if (Physics.Raycast(ray, out hit, RaycastDistance))  //マウスのポジションからRayを投げて何かに当たったらhitに入れる
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            mousePos = camera_object.ScreenToWorldPoint(mousePos);

            string objectName = hit.collider.gameObject.name; //オブジェクト名を取得して変数に入れる
            GameObject Me = GameObject.Find(objectName);

            GameObject childUI = ObjForScale.transform.GetChild(1).gameObject;



            Debug.Log(objectName + "当たっているよ！"); //オブジェクト名をコンソールに表示

            Me.transform.Rotate(Rotate * Time.deltaTime); //カーソル合わせたら回転

            Debug.DrawRay(transform.position, mousePos - transform.position, Color.blue);

            if (Input.GetButton("Decide")) //マウスがクリックされたら
            {
                var Forenabled = camera_object.GetComponent<CameraRotate>();
                Debug.Log("左クリック推したよ");

                childUI.SetActive(true);

                Forenabled.enabled = !Forenabled.enabled;

                
                Cursor.lockState = CursorLockMode.None;
            }

            


        }

        if (Input.GetButton("Cancel001"))
        {
            

            GameObject childUI = ObjForScale.transform.GetChild(1).gameObject;
            childUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;

        }

    }

    

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

    public void OnCPU(GameObject CPU01)
    {
        characterDate.SetCPU01(CPU01);

        Debug.Log("これはおしたの２P");
    }

    public void Light1P()
    {
        SpotLight1P.transform.position = SpotLightVec;
    }

    public void Light2P()
    {
        SpotLight2P.transform.position = SpotLightVec;
    }
}

