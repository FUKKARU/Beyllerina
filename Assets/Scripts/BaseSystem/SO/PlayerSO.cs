using System;
using UnityEngine;

namespace BaseSystem
{
    [CreateAssetMenu(menuName = "SO/BaseSystem/Player", fileName = "PlayerSO")]
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
        [Space(25)]
        [Header("�y�����֌W�z")] public BehaviourTable BehaviourTable;
        [Header("�y�_���[�W�����֌W�z")] public DamageTable DamageTable;
    }

    #region Table
    [Serializable]
    public class BehaviourTable
    {
        [Header("�x�C�̒��S���itransform.up�j���A\n�n�ʂ̖@���x�N�g���ɍ��킹�鑬�x")] public float PlayerMainAxisChangeSpeed;
        [Header("��]���̉�]���x�i��/�b�j")] public float AxisRotateSpeed;
        [Header("��]���̌X����ύX����Ԋu�i�b�j")] public float AxisSlopeChangeInterval;
        [Header("�x�C�̉�]���x�����{�܂ŋ��e���邩�imin,max�j")] public Vector2 RotationSpeedCoefRange;
        [Header("�΍��^�����n�߂鋫�E��HP���A�ő�HP�̉��{�ɂ��邩")] public float AxisSlopeStartHpCoef;
        [Header("�΍��^���ɂ����āA���S�����牽�x�X���邩�imin,max�j")] public Vector2 AxisSlopRange;
        [Header("Rigidbody.drag�i����R�j��\r\n�m�b�N�o�b�N�ϐ��̉��{�ɂ��邩")] public float DragCoef;
        [Header("KNOCKBACKED�̏����ɂ����āA\r\n�����ƓG�̉^���ʂ̑傫���̘a�́A\n���{�̗͂��u�ԓI�Ɏ󂯂邩")] public float PowerCoefOnKnockbacked;
        [Header("KNOCKBACKED�̏����ɂ����āA\r\n�u�ԓI�Ɏ󂯂�͂̍ŏ��l�i���Œ�ۏ؁j")] public float MinPowerOnKnockbacked;
        [Header("�J�E���^�[���ɁA�����̃x�C�̃m�b�N�o�b�N�ϐ������{�ɂ��邩")] public float KnockbackResistanceCoefOnCounter;
        [Header("�v�b�V�����s��A���b��IDLE��Ԃɖ߂���")] public float Duration2IdleOnPushFailed;
        [Header("�J�E���^�[���s��A���b��IDLE��Ԃɖ߂���")] public float Duration2IdleOnCounterFailed;
        [Header("Knockbacked��Ԃ���A���b��IDLE��Ԃɖ߂���")] public float Duration2IdleWhenKnockbacked;
        [Header("�N�[���^�C���̏��������b���Ƃɍs����")] public float CooltimeBehaviourInterval;
    }

    [Serializable]
    public class DamageTable
    {
        [Header("�_���[�W��H�炦��Ԋu�i�����G���ԁj")] public float DamagableDuration;
        [Header("�H�炦��_���[�W�̍ŏ��l")] public float MinDamage;
        [Header("�H�炦��_���[�W�̍ő�l")] public float MaxDamage;
        
        [Header("��ԕ␳�l\r\n�iIDLE/PUSH/COUNTER/KNOCKBACKED�j")] public float[] StateAdjustValue;
        [Header("�_���[�W�W��")] public float DamageCoef;
        [Header("�d��/��]���x/�m�b�N�o�b�N�ϐ��␳�l�ŁA\r\nx = d/2 �̎��̒l")] public float MrkAdjustValueY;
        [Header("�d��/��]���x/�m�b�N�o�b�N�ϐ��␳�l�ŁA\r\nx >= d �̎��ƌX�����������Ȃ� x( < d) ���A\r\n d �̉��{���ǂ���")] public float MrkAdjustValueX;
        [Header("�̗͕␳�l�i�΍��^�������Ă���/���Ă��Ȃ��j")] public float[] HpAdjustValue;

        [Header("�_���[�W�v�Z�̏ڍ�")] public DamageTableDescription DamageDescriptionTable;
    }

    [Serializable]
    public class DamageTableDescription
    {
        [Header("�_���[�W��\r\n�i��b�_���[�W�j��\r\n�i�X�e�[�^�X�␳�l�j��\r\n�i��ԕ␳�l�j��\r\n�i�_���[�W�W���j")]
        [Space(50)]
        [Header("��b�_���[�W��\r\n�i�b�����̉^���ʁb�{�b����̉^���ʁb�j")]
        [Header("�X�e�[�^�X�␳�l��\r\n�i�d�ʕ␳�l�j��\r\n�i��]���x�␳�l�j��\r\n�i�m�b�N�o�b�N�ϐ��␳�l�j��\r\n�i�̗͕␳�l�j")]
        [Header("��ԕ␳�l��\r\n�iIDLE�Ȃ�1�j�A\r\n�iPUSH�Ȃ�0.5�j�A\r\n�iCOUNTER�Ȃ�0�j�A\r\n�iKNOCKBACKED�Ȃ�2�j")]
        [Header("�_���[�W�W���F�����p")]
        [Space(50)]
        [Header("�i���l��x�A�f�t�H���g�l��d�A��L�̒l��Y,X�Ƃ���B�j\r\n�i�� k = (-2/d)log(2 - Y) �j\r\n�i�� m = (-k)exp(k(X - d)) �j\r\n�d�ʕ␳�l��\r\n��]���x�␳�l��\r\n�m�b�N�o�b�N�ϐ��␳�l��\r\n�ix < d �Ȃ� -exp(k(x - d)) + 2�j�A\r\n�i�����łȂ��Ȃ� m(x - d) + 1�j")]
        [Header("�̗͕␳�l��\r\n�i�΍��^�������Ă���Ȃ�0.9�j�A\r\n�i�����łȂ��Ȃ�1�j")]
        [Space(50)]

        [Header("�������Ȃ��萔")] public int Tmp;
    }
    #endregion
}
