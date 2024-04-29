using System;
using UnityEngine;
using UnityEngine.Serialization;

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

        [Header("�y�f�o�b�O�֌W�z")] public DebugTable Dbg;
        [Space(25)]
        [Header("�n�ʂƂ��Ďg���I�u�W�F�N�g�̃��C���[")] public LayerMask WhatIsGround;
        [Header("�x�C�̃^�O")] public string BeyTagName;
        [Header("�d�͂����{�ɂ��邩")] public float GravityScale;
        [Header("�v���C���[�̍���")] public float PlayerHeight;
        [Header("���S�ւ̈ړ����x")] public float SpeedTowardCenter;
        [Header("�J������'theta'�͈̔�(min, max)")] public Vector2 CameraThetaRange;
        [Header("�J������'angle'�͈̔�(min, max)")] public Vector2 CameraAngleRange;
        [Header("�J�����������O�Ղ́A�~�̔��a")] public float CameraRadius;
        [Header("�J�������X�e�[�W��1������̂ɂ����鎞��[s]")] public float CameraDuration;
        [Header("������y���W�ɑ΂���A���OUI��y���W(offset)")] public float NameUIOffset;
        [Header("���������u�Ԃ��牽�b���HP���񕜂��邩")] public float UntilHpRecoverDur;
        [Header("�������ɁA�ő�HP - ����HP�̉�%���񕜂��邩")] public float HpRecoverRatio;
        [Header("HP���񕜂��Ă��牽�b��ɃV�[���J�ڂ��邩")] public float FromHpRecoverDur;
        [Header("K.O. �̕��������b�ŉ�ʂ̒����Ɏ����Ă��邩")] public float KODur;
        [Header("K.O.�ɂȂ��Ă���A���b�ŃN���b�N�\�ɂ��邩")] public float OnKOClickDur;
        [Space(25)]
        [Header("�y�����֌W�z")] public BehaviourTable BehaviourTable;
        [Header("�y���C�g�̉��o�֌W�z")] public LightDirectionTable LightDirectionTable;
        [Header("�y�_���[�W�����֌W�z")] public DamageTable DamageTable;
    }

    #region Table
    [Serializable]
    public class DebugTable
    {
        [Header("���O���o���iWarning��Error�͏����j")] public bool IsShowNormalLog;
        [Header("�v���C�A�u���̎󂯂�_���[�W��100�{�ɂ���")] public bool P_DamageMul;
        [Header("�A���v���C�A�u���̎󂯂�_���[�W��100�{�ɂ���")] public bool U_DamageMul;
        [Header("�v���C�A�u���̎󂯂�_���[�W��0�{�ɂ���")] public bool P_DamageImmune;
        [Header("�A���v���C�A�u���̎󂯂�_���[�W��0�{�ɂ���")] public bool U_DamageImmune;
        [Header("�y�戵���Ӂz\r\n�v�b�V���A�J�E���^�[�A�X�L���A�K�E�Z��\r\n�N�[���^�C����1�b�ɂ��A\r\n�K�E�Z�̔����ɕK�v�ȃ|�C���g��1�ɂ���B")] public bool IsInfinityAction;
    }

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
        [Header("�N�[���^�C�����󂯂Ă��Ȃ��Ƃ��AGauge�̓����x"), Range(0, 255)] public byte GaugeAOnCooltime;
        [Header("���b�����Ƀ|�C���g�𒙂߂邩")] public float PointDur;
        [Header("�Z�b�����ɒ��߂�|�C���g�̗�")] public int PointAmount;
        [Header("�{�[�i�X�|�C���g�̗�\r\n�i�v�f���ŁA�̗͂̔��苫�E�����ʂ���B�̗͂����Ȃ��������̏��j")] public int[] BonusPoint;
        [Header("Clamp���� r �i���a�j�imin/max�j")] public Vector2 ClampR;
        [Header("Clamp���� y �i�����j�imin/max�j")] public Vector2 ClampY;
        [Header("��������ʒu")] public float FallJudgeY;
        [Header("�����C���ʒu")] public float FallResetY;
        [Header("Playable respawn point tag")] public string P_RePosTag;
        [Header("UnPlayable respawn point tag")] public string U_RePosTag;
    }

    [Serializable]
    public class LightDirectionTable
    {
        [Header("���C�g�̃f�t�H���g�̐F")] public Color LightNormalColor;
        [Header("�����^���̃f�t�H���g��intensity")] public float LampNormalIntensity;
        [Header("�K�E�Z�������ɁA���C�g�����F�ɂ��邩")] public Color LightSpecialColor;
        [Header("�K�E�Z�������ɁA�����^����intensity��������ɂ��邩")] public float LampSpecialIntensity;
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
        [Header("�_���[�W��\r\n�i��b�_���[�W�j��\r\n�i�X�e�[�^�X�␳�l�j��\r\n�i��ԕ␳�l�j��\r\n�i�_���[�W�W���j * \r\n�i�ėp�_���[�W�W���j")]
        [Space(50)]
        [Header("��b�_���[�W��\r\n�i�b�����̉^���ʁb�{�b����̉^���ʁb�j")]
        [Header("�X�e�[�^�X�␳�l��\r\n�i�d�ʕ␳�l�j��\r\n�i��]���x�␳�l�j��\r\n�i�m�b�N�o�b�N�ϐ��␳�l�j��\r\n�i�̗͕␳�l�j")]
        [Header("��ԕ␳�l��\r\n�iIDLE�Ȃ�1�j�A\r\n�iPUSH�Ȃ�0.5�j�A\r\n�iCOUNTER�Ȃ�0�j�A\r\n�iKNOCKBACKED�Ȃ�2�j")]
        [Header("�_���[�W�W���F�����p")]
        [Header("�ėp�_���[�W�W���F�e�x�C���Ƃ̒����p�B�����I�Ɍv�Z�����")]
        [Space(50)]
        [Header("�i���l��x�A�f�t�H���g�l��d�A��L�̒l��Y,X�Ƃ���B�j\r\n�i�� k = (-2/d)log(2 - Y) �j\r\n�i�� m = (-k)exp(k(X - d)) �j\r\n�d�ʕ␳�l��\r\n��]���x�␳�l��\r\n�m�b�N�o�b�N�ϐ��␳�l��\r\n�ix < d �Ȃ� -exp(k(x - d)) + 2�j�A\r\n�i�����łȂ��Ȃ� m(x - d) + 1�j")]
        [Header("�̗͕␳�l��\r\n�i�΍��^�������Ă���Ȃ�0.9�j�A\r\n�i�����łȂ��Ȃ�1�j")]
        [Space(50)]

        [Header("�������Ȃ��萔")] public int Tmp;
    }
    #endregion
}
