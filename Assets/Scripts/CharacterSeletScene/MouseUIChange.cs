using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUIChange : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursorArrow;

    //なんかこれ設定する意味ないかも
    [SerializeField]
    private Vector2 CursorVecror2 = new Vector2();
    // Start is called before the first frame update
    void Start()
    {
        //カーソルのTexture２Dを指定（Texture３D、Textureは無理）
        Cursor.SetCursor(cursorArrow, CursorVecror2, CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
