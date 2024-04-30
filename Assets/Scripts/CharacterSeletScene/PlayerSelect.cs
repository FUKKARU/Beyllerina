using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class PlayerSelect : MonoBehaviour
{
    [SerializeField]
    private Transform Camera; //�J�����̈ʒu�����擾
    [SerializeField]
    private GameObject ObjForScale,  SpotLight01, SpotLight1P, SpotLight2P;
    [SerializeField]
    private Vector3 Rotate = new Vector3(0, 50f, 0); //y�̒l����b���Ɖ�]����p�x�i�܂�A������Ή�]���x�������Ȃ�j
    [SerializeField]
    private Vector3 SpotLightVec = new Vector3();�@//�X�|�b�g���C�g�̃|�W�V����
    [SerializeField]
    private Vector3 ObjStartSize = new Vector3(0.45f, 0.45f, 0.45f);�@//���Ƃ��Ƃ̃I�u�W�F�N�g�̃T�C�Y
    [SerializeField]
    private Vector3 ObjPlusSize = new Vector3(0.15f, 0.15f, 0.15f); //�I����̃I�u�W�F�N�g�̃T�C�Y�̑���
    [SerializeField]
    private float SpotLightHigh = 1.0f;

    bool isBallerinaSelected = false;



    private CharacterDate characterDate;
    

    private void Start()
    {
        ObjForScale.transform.localScale = ObjStartSize;
        characterDate = FindObjectOfType<CharaGameManager>().GetCharacterDate();
        
    }

    private void Update()
    {
        if (isBallerinaSelected && IA.InputGetter.Instance.IsSelect)
        {
            LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Game, true);
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
            if (gameObject.name == "ballerina_sit")
            {
                isBallerinaSelected = true;
            }
            SpotLightVec = new Vector3(ObjForScale.transform.position.x, ObjForScale.transform.position.y + SpotLightHigh, ObjForScale.transform.position.z);
            Debug.Log("�������Ă����I");
           
            //�Փ˂����ꍇ�����g��
            ObjForScale.transform.localScale = new Vector3(ObjStartSize.x + ObjPlusSize.x, ObjStartSize.y + ObjPlusSize.y, ObjStartSize.z + ObjPlusSize.z);
            SpotLight01.transform.position = SpotLightVec;
            //BaseSystem.SoundManager.Instance.PlaySE(0);
            Debug.Log("�I��ł���Ă��肪�Ƃ��I");

        
        }


        
    }

    private void OnCollisionExit(Collision collision)
    {
        if (gameObject.name == "ballerina_sit")
        {
            isBallerinaSelected = false;
        }
        SpotLightVec = new Vector3(0, 0, 0);
        //�Փ˂��Ȃ��ꍇ���ɖ߂�
        ObjForScale.transform.localScale = ObjStartSize;
        Debug.Log("����...�I��ł���Ȃ���");
        //BaseSystem.SoundManager.Instance.PlaySE(1);
        



    }

    private void OnCollisionStay(Collision collision)
    {
        GameObject childUI = ObjForScale.transform.GetChild(1).gameObject;
       
        
        if (Input.GetButton("Decide"))
        {
            
            Debug.Log("���N���b�N��������");

            
   �@�@//  this.transform.transform.position = Camera.transform.position + new Vector3(1f, -1.35f, 2.58f);

           
            Debug.Log("�߂Â��Ă���");


            childUI.SetActive(true);


            Cursor.lockState = CursorLockMode.None;

        }

        this.gameObject.transform.Rotate(Rotate * Time.deltaTime);


    }

    //�I�u�W�F�N�g��character�ɑ�����āASetPlayer�Őݒ肵��character�iCharacterDataScript)��character�i�����̕ϐ��j��������
    public void OnSlectCharacter1P(GameObject character01)
    {
        characterDate.SetPlayer01(character01);
  
        Debug.Log("����͂�����");
    }
    
    public void OnSlectCharacter2P(GameObject character02)
    {
        characterDate.SetPlayer02(character02);

        Debug.Log("����͂������̂QP");
    }

    public void OnCPU(GameObject CPU01)
    {
        characterDate.SetCPU01(CPU01);

        Debug.Log("����͂������̂QP");
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
