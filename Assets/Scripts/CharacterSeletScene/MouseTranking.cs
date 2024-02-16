using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTranking : MonoBehaviour
{
    
    /// �}�E�X�|�C���^�[�𓊉e����Canvas�R���|�[�l���g�̎Q��
    [SerializeField] 
    private Canvas canvas;
    /// �}�E�X�|�C���^�[�𓊉e����Canvas��RectTransform�R���|�[�l���g�̎Q��
    [SerializeField] 
    private RectTransform canvasTransform;
    /// �}�E�X�|�C���^�[��RectTransform�R���|�[�l���g�̎Q��
    [SerializeField] 
    private RectTransform cursorTransform;
    [SerializeField]
    private 

    void Update()
    {
        // Canvas��RectTransform���ɂ���}�E�X�̍��W�����[�J�����W�ɕϊ�����
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform,Input.mousePosition,canvas.worldCamera,out var mousePosition);

        // �|�C���^�[���}�E�X�̍��W�Ɉړ�������
        cursorTransform.anchoredPosition = new Vector2(mousePosition.x, mousePosition.y);
    }
}
