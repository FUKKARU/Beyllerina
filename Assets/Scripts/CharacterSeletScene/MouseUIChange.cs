using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUIChange : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursorArrow;

    //�Ȃ񂩂���ݒ肷��Ӗ��Ȃ�����
    [SerializeField]
    private Vector2 CursorVecror2 = new Vector2();
    // Start is called before the first frame update
    void Start()
    {
        //�J�[�\����Texture�QD���w��iTexture�RD�ATexture�͖����j
        Cursor.SetCursor(cursorArrow, CursorVecror2, CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
