using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateSetter : MonoBehaviour
{
    void Awake()
    {
        QualitySettings.vSyncCount = 0; // VSync��OFF�ɂ���
        Application.targetFrameRate = GameStateSO.Entity.TargetFrameRate; // �^�[�Q�b�g�t���[�����[�g��ݒ�
    }
}
