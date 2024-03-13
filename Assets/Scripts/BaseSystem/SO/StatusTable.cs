using System;
using UnityEngine;

namespace BaseSystem
{
    [Serializable]
    public class StatusTable
    {
        [Header("�v���C�A�u�����ǂ���")] public bool IsPlayable;
        [Space(25)]
        [Header("�y���O�z")] public StatusTableName StatusTableName;
        [Header("�y�v���C�A�u���z")] public StatusTablePlayable StatusTablePlayable;
        [Header("�y�A���v���C�A�u���z")] public StatusTableUnPlayable StatusTableUnPlayable;
        [Header("�y�X�e�[�^�X���i�����l�j�z")] public StatusTableInitStatus StatusTableInitStatus;
    }

    [Serializable]
    public class StatusTableName
    {
        [Header("���O")] public string Name;
        [Header("�X�L���̖��O")] public string SkillName;
        [Header("�K�E�Z�̖��O")] public string SpecialName;
    }

    [Serializable]
    public class StatusTablePlayable
    {
        [Header("�v�b�V���L�[")] public KeyCode PushKey;
        [Header("�J�E���^�[�L�[")] public KeyCode CounterKey;
    }

    [Serializable]
    public class StatusTableUnPlayable
    {
        [Header("�v�b�V�����X�L�����v�b�V�����c")]
        [Header("�v�b�V�����X�L���̊Ԋu")] public float Push2SkillInterval;
        [Header("�v�b�V�����X�L���̊Ԋu�̌덷�i�}�j")] public float Push2SkillIntervalOffset;
        [Header("�X�L�����v�b�V���̊Ԋu")] public float Skill2PushInterval;
        [Header("�X�L�����v�b�V���̊Ԋu�̌덷�i�}�j")] public float Skill2PushIntervalOffset;
    }

    [Serializable]
    public class StatusTableInitStatus
    {
        [Header("�̗�")] public float Hp;
        [Header("�d��")] public float Weight;
        [Header("��]���x")] public float RotationSpeed;
        [Header("�v�b�V����")] public float PushPower;
        [Header("�m�b�N�o�b�N�ϐ�"), Range(0, 1)] public float KnockbackResistance;
        [Header("�K�E�Z�̔����ɕK�v�ȃQ�[�W��")] public float GaugeAmountUntilSpecial;
        [Header("�v�b�V���̃N�[���^�C��")] public float PushCoolTime;
        [Header("�J�E���^�[�̃N�[���^�C��")] public float CounterCoolTime;
        [Header("�X�L���̃N�[���^�C��")] public float SkillCooltime;
        [Header("�K�E�Z�̃N�[���^�C��")] public float SpecialCooltime;
    }
}
