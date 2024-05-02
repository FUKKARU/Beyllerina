using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterSelect
{
    public class PlayerSelect : MonoBehaviour
    {
        [SerializeField]
        private Transform Camera; //カメラの位置情報を取得
        [SerializeField]
        private GameObject ObjForScale, SpotLight01, SpotLight1P, SpotLight2P, Chikuonki;
        [SerializeField]
        private Vector3 Rotate = new Vector3(0, 50f, 0); //yの値が一秒ごと回転する角度（つまり、あげれば回転速度が早くなる）
        [SerializeField]
        private Vector3 SpotLightVec = new Vector3(); //スポットライトのポジション
        [SerializeField]
        private Vector3 ObjStartSize = new Vector3(0.45f, 0.45f, 0.45f); //もともとのオブジェクトのサイズ
        [SerializeField]
        private Vector3 ObjPlusSize = new Vector3(0.15f, 0.15f, 0.15f); //選択後のオブジェクトのサイズの増分
        [SerializeField]
        private float SpotLightHigh = 1.0f;
        [SerializeField] AudioClip ClickSound;
        AudioSource audioSource;
        [SerializeField] CameraShake_CS cameraShakeScript;
        [SerializeField] Image crosshair;
        [SerializeField] Collider cameraCollider;

        bool isBallerinaSelected = false;



        private CharacterDate characterDate;


        public GameObject Count3;
        public GameObject Count2;
        public GameObject Count1;
        public GameObject CountGo;
        bool changeSceneOnce;// ゲームシーンへの遷移は発動したか？

        private void Start()
        {
            audioSource = Chikuonki.GetComponent<AudioSource>();
            changeSceneOnce = false;
            ObjForScale.transform.localScale = ObjStartSize;
            characterDate = FindObjectOfType<CharaGameManager>().GetCharacterDate();

        }


        IEnumerator CountAndStart()
        {
            cameraCollider.enabled = false;
            cameraShakeScript.IsCameraShakeOn = false;
            crosshair.enabled = false;

            Count3.SetActive(true);
            yield return new WaitForSeconds(1);
            Count3.SetActive(false);
            Count2.SetActive(true);
            yield return new WaitForSeconds(1);
            Count2.SetActive(false);
            Count1.SetActive(true);
            yield return new WaitForSeconds(1);
            Count1.SetActive(false);
            CountGo.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Game, true);
        }

        private void Update()
        {
            if (isBallerinaSelected && IA.InputGetter.Instance.IsSelect && !changeSceneOnce)
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
                    audioSource.PlayOneShot(ClickSound);
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

            SpotLight01.transform.position = new Vector3(0, 0, 0);
            //衝突しない場合元に戻る
            ObjForScale.transform.localScale = ObjStartSize;
            //Debug.Log("えっ...選んでくれないの");
            //BaseSystem.SoundManager.Instance.PlaySE(1);




        }

        private void OnCollisionStay(Collision collision)
        {
            GameObject childUI = ObjForScale.transform.GetChild(1).gameObject;


            if (IA.InputGetter.Instance.IsSelect)
            {

                //Debug.Log("左クリック推したよ");


                //  this.transform.transform.position = Camera.transform.position + new Vector3(1f, -1.35f, 2.58f);


                //Debug.Log("近づいてきた");


                childUI.SetActive(true);




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
}