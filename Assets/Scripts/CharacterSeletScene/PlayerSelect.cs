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
    private string sceneName;
    [SerializeField]
    private GameObject ObjForScale, CharacterMenu;
  //  [SerializeField]
   // private float angle = 1.0f;
    

    Dictionary<string, int> DecidedCharacter = new Dictionary<string, int>();


    float StartNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
       
        //CharacterMenu.SetActive(false);

        //Destroy(this.gameObject);
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (Input.GetButton("Decide"))
        {
            
            Debug.Log("左クリック推したよ");

            //Instantiate<GameObject>(this.gameObject, this.gameObject.transform.position, Quaternion.identity);

            this.transform.transform.position = Camera.transform.position + new Vector3(1f, -1.35f, 2.58f);
            this.transform.Rotate(new Vector3(31f, 0, 0));
            
            
            Debug.Log("近づけてきた！");

            CharacterMenu.SetActive(true);


        }

        while (CharacterMenu.activeSelf)
        {

            

        }
    }
    //次のシーンへ
    public void changeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
