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
    private GameObject ObjForScale, CharacterMenu;
    [SerializeField]
    private Vector3 Rotate = new Vector3(0, 50f, 0); //�V�����x�N�g���R���`
    



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
            Debug.Log("�������Ă����I");
            //�Փ˂����ꍇ�����g��
            ObjForScale.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            Debug.Log("�I��ł���Ă��肪�Ƃ��I");

                
        }


        
    }

    private void OnCollisionExit(Collision collision)
    {
        

        //�Փ˂��Ȃ��ꍇ���ɖ߂�
        ObjForScale.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        Debug.Log("����...�I��ł���Ȃ���");

    



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
    
}
