using UnityEngine;

namespace BaseSystem
{
    [CreateAssetMenu(menuName = "SO/BaseSystem/Status/BreakDancer", fileName = "BreakDancerStatusSO")]
    public class BreakDancerStatusSO : ScriptableObject
    {
        #region QOL���㏈��
        // �ۑ����Ă���ꏊ�̃p�X
        public const string PATH = "SO/BaseSystem/Status/BreakDancerStatusSO";

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

        public StatusTable StatusTable;
        [Header("�X�L���g�p���ɁA��]���������b�ŋt�ɂ��邩")] public float SkillRotChangeDur;
        [Header("�X�L���̌p������")] public float SkillDur;
        [Header("�K�E�Z�g�p���ɁA�v�b�V���͂����{�ɂ��邩")] public float SpecialPushPowerCoefCoef;
        [Header("�K�E�Z�g�p���ɁA��]���x�����{�ɂ��邩")] public float SpecialRotSpeedCoef;
        [Header("�K�E�Z�̌p������")] public float SpecialDur;
    }
}
