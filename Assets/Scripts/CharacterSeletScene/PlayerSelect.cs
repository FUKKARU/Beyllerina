using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterSelect
{
    public class PlayerSelect : MonoBehaviour
    {
        [SerializeField]
        private Transform Camera; //�J�����̈ʒu�����擾
        [SerializeField]
        private GameObject ObjForScale, SpotLight01, SpotLight1P, SpotLight2P, Chikuonki;
        [SerializeField]
        private Vector3 Rotate = new Vector3(0, 50f, 0); //y�̒l����b���Ɖ�]����p�x�i�܂�A������Ή�]���x�������Ȃ�j
        [SerializeField]
        private Vector3 SpotLightVec = new Vector3(); //�X�|�b�g���C�g�̃|�W�V����
        [SerializeField]
        private Vector3 ObjStartSize = new Vector3(0.45f, 0.45f, 0.45f); //���Ƃ��Ƃ̃I�u�W�F�N�g�̃T�C�Y
        [SerializeField]
        private Vector3 ObjPlusSize = new Vector3(0.15f, 0.15f, 0.15f); //�I����̃I�u�W�F�N�g�̃T�C�Y�̑���
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
        bool changeSceneOnce;// �Q�[���V�[���ւ̑J�ڂ͔����������H

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
                //Debug.Log("�������Ă����I");

                //�Փ˂����ꍇ�����g��
                ObjForScale.transform.localScale = new Vector3(ObjStartSize.x + ObjPlusSize.x, ObjStartSize.y + ObjPlusSize.y, ObjStartSize.z + ObjPlusSize.z);
                SpotLight01.transform.position = SpotLightVec;
                //BaseSystem.SoundManager.Instance.PlaySE(0);
                //Debug.Log("�I��ł���Ă��肪�Ƃ��I");


            }



        }

        private void OnCollisionExit(Collision collision)
        {
            if (gameObject.CompareTag("CharacterSelect_Ballerina"))
            {
                isBallerinaSelected = false;
            }

            SpotLight01.transform.position = new Vector3(0, 0, 0);
            //�Փ˂��Ȃ��ꍇ���ɖ߂�
            ObjForScale.transform.localScale = ObjStartSize;
            //Debug.Log("����...�I��ł���Ȃ���");
            //BaseSystem.SoundManager.Instance.PlaySE(1);




        }

        private void OnCollisionStay(Collision collision)
        {
            GameObject childUI = ObjForScale.transform.GetChild(1).gameObject;


            if (IA.InputGetter.Instance.IsSelect)
            {

                //Debug.Log("���N���b�N��������");


                //  this.transform.transform.position = Camera.transform.position + new Vector3(1f, -1.35f, 2.58f);


                //Debug.Log("�߂Â��Ă���");


                childUI.SetActive(true);




            }

            this.gameObject.transform.Rotate(Rotate * Time.deltaTime);


        }

        //�I�u�W�F�N�g��character�ɑ�����āASetPlayer�Őݒ肵��character�iCharacterDataScript)��character�i�����̕ϐ��j��������
        public void OnSlectCharacter1P(GameObject character01)
        {
            characterDate.SetPlayer01(character01);

            //Debug.Log("����͂�����");
        }

        public void OnSlectCharacter2P(GameObject character02)
        {
            characterDate.SetPlayer02(character02);

            //Debug.Log("����͂������̂QP");
        }

        public void OnCPU(GameObject CPU01)
        {
            characterDate.SetCPU01(CPU01);

            //Debug.Log("����͂������̂QP");
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