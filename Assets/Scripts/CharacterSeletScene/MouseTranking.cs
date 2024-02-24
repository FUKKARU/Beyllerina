using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTranking : MonoBehaviour
{

    /// �}�E�X�|�C���^�[�𓊉e����Canvas�R���|�[�l���g�̎Q��
    private Vector3 mouseVector;

    private Vector3 targetObj;

    void Update()
    {
        //�}�E�X�̃|�W�V����
        mouseVector = Input.mousePosition;
        //�}�E�X�ɒǏ]����I�u�W�F�N�g�̃|�W�V����
        targetObj = Camera.main.ScreenToWorldPoint(new Vector3(mouseVector.x, mouseVector.y, 10));
        this.transform.position = targetObj;
        
    }
}
