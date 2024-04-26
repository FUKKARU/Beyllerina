using UnityEngine;

namespace BaseSystem
{
    [CreateAssetMenu(menuName = "SO/BaseSystem/Status/Enemy1", fileName = "Enemy1StatusSO")]
    public class Enemy1StatusSO : ScriptableObject
    {
        #region QOL���㏈��
        // �ۑ����Ă���ꏊ�̃p�X
        public const string PATH = "BaseSystem/SO/Status/Enemy1StatusSO";

        // ����
        private static Enemy1StatusSO _entity;
        public static Enemy1StatusSO Entity
        {
            get
            {
                // ���A�N�Z�X���Ƀ��[�h����
                if (_entity == null)
                {
                    _entity = Resources.Load<Enemy1StatusSO>(PATH);

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
        [Header("�v�b�V���̃G�t�F�N�g")] public GameObject PushEffectObj;
        [Header("�v�b�V���O�̉��o�����b�����邩")] public float PushDirDur;
        [Header("�X�L���g�p�����{�傫���Ȃ邩")] public float SkillSizeCoef;
        [Header("�X�L���g�p�����ʂ����{�ɂ��邩")] public float SkillWeightCoef;
        [Header("�X�L���g�p���ǂ��܂ŏ㏸���邩")] public float SkillHeight;
        [Header("�X�L���g�p���ǂ̂悤�ȃX�s�[�h�ŏ㏸���邩")] public float SkillSpeed;
        [Header("�X�L���̌p������")] public float SkillDuration;
    }
}
