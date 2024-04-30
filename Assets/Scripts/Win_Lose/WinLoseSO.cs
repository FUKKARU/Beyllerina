using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Win_Lose
{
    [CreateAssetMenu(menuName = "SO/Win_Lose/WinLoseSO", fileName = "WinLoseSO")]
    public class WinLoseSO : ScriptableObject
    {
        #region QOL���㏈��
        // �ۑ����Ă���ꏊ�̃p�X
        public const string PATH = "Win_Lose/WinLoseSO";

        // ����
        private static WinLoseSO _entity;
        public static WinLoseSO Entity
        {
            get
            {
                // ���A�N�Z�X���Ƀ��[�h����
                if (_entity == null)
                {
                    _entity = Resources.Load<WinLoseSO>(PATH);

                    // ���[�h�o���Ȃ������ꍇ�̓G���[���O��\��
                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [Header("�y�J�����̉��o�֌W�z")] public CameraDirectionTable CameraDir;
    }

    [Serializable]
    public class CameraDirectionTable
    {
        [Header("�J�n�ʒu")] public Vector3 StartPosition;
        [FormerlySerializedAs("EndPositon"), Header("�I���ʒu")] public Vector3 EndPosition;
        [Header("�J�n��]")] public Vector3 StartRotation;
        [Header("�I����]")] public Vector3 EndRotation;
        [Header("����")] public float Duration;
    }
}