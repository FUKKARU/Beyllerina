using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTranking : MonoBehaviour
{
    
    /// マウスポインターを投影するCanvasコンポーネントの参照
    [SerializeField] 
    private Canvas canvas;
    /// マウスポインターを投影するCanvasのRectTransformコンポーネントの参照
    [SerializeField] 
    private RectTransform canvasTransform;
    /// マウスポインターのRectTransformコンポーネントの参照
    [SerializeField] 
    private RectTransform cursorTransform;
    [SerializeField]
    private 

    void Update()
    {
        // CanvasのRectTransform内にあるマウスの座標をローカル座標に変換する
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform,Input.mousePosition,canvas.worldCamera,out var mousePosition);

        // ポインターをマウスの座標に移動させる
        cursorTransform.anchoredPosition = new Vector2(mousePosition.x, mousePosition.y);
    }
}
