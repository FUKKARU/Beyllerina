using UnityEngine;

namespace BaseSystem
{
    [CreateAssetMenu(menuName = "SO/Player", fileName = "PlayerSO")]
    public class PlayerSO : ScriptableObject
    {
        #region QOL���㏈��
        // �ۑ����Ă���ꏊ�̃p�X
        public const string PATH = "BaseSystem/SO/PlayerSO";

        // ����
        private static PlayerSO _entity;
        public static PlayerSO Entity
        {
            get
            {
                // ���A�N�Z�X���Ƀ��[�h����
                if (_entity == null)
                {
                    _entity = Resources.Load<PlayerSO>(PATH);

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

        [Header("���O���o�����ǂ����iWarning��Error�͏����j")] public bool IsShowNormalLog;
        [Header("�n�ʂƂ��Ďg���I�u�W�F�N�g�̃��C���[")] public LayerMask WhatIsGround;
        [Header("�x�C�̃^�O")] public string BeyTagName;
        [Header("�d�͂����{�ɂ��邩")] public float GravityScale;
        [Header("�v���C���[�̍���")] public float PlayerHeight;
        [Header("���S�ւ̈ړ����x")] public float SpeedTowardCenter;
        [Header("�x�C�̒��S���itransform.up�j���A\n�n�ʂ̖@���x�N�g���ɍ��킹�鑬�x")] public float PlayerMainAxisChangeSpeed;
        [Header("��]���̉�]���x�i��/�b�j")] public float AxisRotateSpeed;
        [Header("��]���̌X����ύX����Ԋu�i�b�j")] public float AxisSlopeChangeInterval;
        [Header("�x�C�̉�]���x�����{�܂ŋ��e���邩�imin,max�j")] public Vector2 RotationSpeedCoefRange;
        [Header("�΍��^�����n�߂鋫�E��HP���A�ő�HP�̉��{�ɂ��邩")] public float AxisSlopeStartHpCoef;
        [Header("�΍��^���ɂ����āA���S�����牽�x�X���邩�imin,max�j")] public Vector2 AxisSlopRange;
        [Header("Rigidbody.drag�i����R�j��\r\n�m�b�N�o�b�N�ϐ��̉��{�ɂ��邩")] public float DragCoef;
        [Header("KNOCKBACKED�̏����ɂ����āA\r\n�����ƓG�̉^���ʂ̑傫���̘a�́A\n���{�̗͂��u�ԓI�Ɏ󂯂邩")] public float PowerCoefOnKnockbacked;
        [Header("KNOCKBACKED�̏����ɂ����āA\r\n�u�ԓI�Ɏ󂯂�͂̍ŏ��l�i���Œ�ۏ؁j")] public float MinPowerOnKnockbacked;
        [Header("�_���[�W��H�炦��Ԋu�i�����G���ԁj")] public float DamagableDuration;
        [Header("�Ԃ��������A����̑��x�̉��{�̃_���[�W��H�炤��")] public float DamageCoef;
        [Header("�v�b�V�����A�󂯂�_���[�W�����{�ɂ��邩")] public float DamageCoefOnPush;
        [Header("�m�b�N�o�b�N���ꂽ���A�󂯂�_���[�W�����{�ɂ��邩")] public float DamageCoefOnKnockbacked;
        [Header("�J�E���^�[���ɁA�����̃x�C�̃m�b�N�o�b�N�ϐ������{�ɂ��邩")] public float KnockbackResistanceCoefOnCounter;
        [Header("�v�b�V�����s��A���b��IDLE��Ԃɖ߂���")] public float Duration2IdleOnPushFailed;
        [Header("�J�E���^�[���s��A���b��IDLE��Ԃɖ߂���")] public float Duration2IdleOnCounterFailed;
        [Header("Knockbacked��Ԃ���A���b��IDLE��Ԃɖ߂���")] public float Duration2IdleWhenKnockbacked;
    }
}
