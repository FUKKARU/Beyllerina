using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Status/BreakDancer", fileName = "BreakDancerStatusSO")]
public class BreakDancerStatusSO : ScriptableObject
{
    #region QOL���㏈��
    // �ۑ����Ă���ꏊ�̃p�X
    public const string PATH = "SO/Status/BreakDancerStatusSO";

    // ����
    private static BreakDancerStatusSO _entity;
    public static BreakDancerStatusSO Entity
    {
        get
        {
            // ���A�N�Z�X���Ƀ��[�h����
            if (_entity == null)
            {
                _entity = Resources.Load<BreakDancerStatusSO>(PATH);

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

    public StatusSO Status;
}
