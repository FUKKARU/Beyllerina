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
       
        //CharacterMenu.SetActive(false);

        //Destroy(this.gameObject);
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (Input.GetButton("Decide"))
        {
            
            Debug.Log("���N���b�N��������");

            //Instantiate<GameObject>(this.gameObject, this.gameObject.transform.position, Quaternion.identity);

            this.transform.transform.position = Camera.transform.position + new Vector3(1f, -1.35f, 2.58f);
            this.transform.Rotate(new Vector3(31f, 0, 0));
            
            
            Debug.Log("�߂Â��Ă����I");

            CharacterMenu.SetActive(true);


        }

        while (CharacterMenu.activeSelf)
        {

            

        }
    }
    //���̃V�[����
    public void changeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
