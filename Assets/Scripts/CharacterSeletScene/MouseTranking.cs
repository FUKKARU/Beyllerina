using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTranking : MonoBehaviour
{

    /// マウスポインターを投影するCanvasコンポーネントの参照
    private Vector3 mouseVector;

    private Vector3 targetObj;

    void Update()
    {
        //マウスのポジション
        mouseVector = Input.mousePosition;
        //マウスに追従するオブジェクトのポジション
        targetObj = Camera.main.ScreenToWorldPoint(new Vector3(mouseVector.x, mouseVector.y, 10));
        this.transform.position = targetObj;
        
    }
}
