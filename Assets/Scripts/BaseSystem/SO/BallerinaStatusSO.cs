using UnityEngine;
using UnityEngine.Rendering;

namespace BaseSystem
{
    [CreateAssetMenu(menuName = "SO/BaseSystem/Status/Ballerina", fileName = "BallerinaStatusSO")]
    public class BallerinaStatusSO : ScriptableObject
    {
        #region QOL���㏈��
        // �ۑ����Ă���ꏊ�̃p�X
        public const string PATH = "SO/BaseSystem/Status/BallerinaStatusSO";

        // ����
        private static BallerinaStatusSO _entity;
        public static BallerinaStatusSO Entity
        {
            get
            {
                // ���A�N�Z�X���Ƀ��[�h����
                if (_entity == null)
                {
                    _entity = Resources.Load<BallerinaStatusSO>(PATH);

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

        public StatusTable StatusTable;
        [Header("�X�L���������ɉ��{�̗͂������邩")] public float SkillPushPowerCoef;
        [Header("�X�L���������̏㏸����")] public float SkillRiseDur;
        [Header("�X�L���������̏㏸���x")] public float SkillRiseHeight;
        [Header("�X�L���������̉��~����")] public float SkillRushDur;
        [Header("�X�L���������̎��Ԏw��")] public int  SkillPow;
        [Header("�X�L���������Ɏ󂯂�_���[�W�����{�ɂ��邩")] public float OnSkillGenericDamageCoefCoef;
        [Header("�X�L���������̎󂯂�_���[�W�̕ω����A\r\n�ːi���Ă��牽�b��Ɍ��ɖ߂���")] public float OnSkillDamageNormalizeDur;
        [Header("�K�E�Z�������ɗ^����_���[�W�����{�ɂ��邩")] public float OnSpecialGenericDamageCoefCoef;
        [Header("�K�E�Z�̌p������")] public float SpecialDur;
        [Header("���㎞�ɁA�^����_���[�W�������̈�ɂ��邩�i����j")] public float OnWeakGenericDamageCoefCoef;
        [Header("�����Ԃ̌p������")] public float WeakDur;
    }
}
