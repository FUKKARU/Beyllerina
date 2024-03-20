using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JstForTest : MonoBehaviour
{
    [SerializeField]
    private Camera camera_object; //�J�������擾
    [SerializeField]
    private float RaycastDistance = 30.0f; 
    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private GameObject ObjForScale, SpotLight1P, SpotLight2P;
    [SerializeField]
    private Collision CameraCollision;
    [SerializeField]
    private Vector3 Rotate = new Vector3(0, 50f, 0); //y�̒l����b���Ɖ�]����p�x�i�܂�A������Ή�]���x�������Ȃ�j
    [SerializeField]
    private Vector3 SpotLightVec = new Vector3();�@//�X�|�b�g���C�g�̃|�W�V����
    [SerializeField]
    private Vector3 ObjStartSize = new Vector3(0.45f, 0.45f, 0.45f);�@//���Ƃ��Ƃ̃I�u�W�F�N�g�̃T�C�Y
   
    



    private CharacterDate characterDate;
    private RaycastHit hit; //���C�L���X�g�������������̂��擾������ꕨ

    void Start()
    {
        ObjForScale.transform.localScale = ObjStartSize;
        characterDate = FindObjectOfType<CharaGameManager>().GetCharacterDate();
    }
    void Update()
    {
        Ray ray = camera_object.ScreenPointToRay(Input.mousePosition);
        
        

        if (Physics.Raycast(ray, out hit, RaycastDistance))  //�}�E�X�̃|�W�V��������Ray�𓊂��ĉ����ɓ���������hit�ɓ����
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            mousePos = camera_object.ScreenToWorldPoint(mousePos);

            string objectName = hit.collider.gameObject.name; //�I�u�W�F�N�g�����擾���ĕϐ��ɓ����
            GameObject Me = GameObject.Find(objectName);

            GameObject childUI = ObjForScale.transform.GetChild(1).gameObject;



            Debug.Log(objectName + "�������Ă����I"); //�I�u�W�F�N�g�����R���\�[���ɕ\��

            Me.transform.Rotate(Rotate * Time.deltaTime); //�J�[�\�����킹�����]

            Debug.DrawRay(transform.position, mousePos - transform.position, Color.blue);

            if (Input.GetButton("Decide")) //�}�E�X���N���b�N���ꂽ��
            {
                var Forenabled = camera_object.GetComponent<CameraRotate>();
                Debug.Log("���N���b�N��������");

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
        SpotLight1P.transform.position = SpotLightVec;
    }

    public void Light2P()
    {
        SpotLight2P.transform.position = SpotLightVec;
    }
}

