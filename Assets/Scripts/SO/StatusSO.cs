using UnityEngine;

[System.Serializable]
public class StatusSO
{
    [Header("�X�e�[�^�X�̏����l�ݒ�")]
    [Header("�f�o�b�O�p�F�W�����v�@�\���I�t�ɂ���")] public bool DebugIsJumpDisable;
    [Header("�f�o�b�O�p�F�K�[�h�@�\���I�t�ɂ���")] public bool DebugIsGuardDisable;
    [Space(25)]
    [Header("���O")] public string Name;
    [Header("�X�L���̖��O")] public string SkillName;
    [Header("�K�E�Z�̖��O")] public string SpecialName;
    [Header("�̗�")] public float Hp;
    [Header("�d��")] public float Weight;
    [Header("��]���x")] public float RotationSpeed;
    [Header("�v�b�V����")] public float PushPower;
    [Header("�W�����v��")] public float JumpPower;
    [Header("�N���e�B�J���v�b�V����")] public float CriticalPushPower;
    [Header("�K�[�h�ϋv�l")] public float GuardDurability;
    [Header("�m�b�N�o�b�N�ϐ�"), Range(0, 1)] public float KnockbackResistance;
    [Header("�K�E�Z�̔����ɕK�v�ȃQ�[�W��")] public float GaugeAmountUntilSpecial;
    [Header("�v�b�V���̃N�[���^�C��")] public float PushCoolTime;
    [Header("�W�����v�̃N�[���^�C��")] public float JumpCooltime;
    [Header("�X�L���̃N�[���^�C��")] public float SkillCooltime;
    [Header("�K�E�Z�̃N�[���^�C��")] public float SpecialCooltime;
    [Header("�O�i�L�[")] public KeyCode ForwardKey;
    [Header("���i�L�[")] public KeyCode LeftKey;
    [Header("��i�L�[")] public KeyCode BackKey;
    [Header("�E�i�L�[")] public KeyCode RightKey;
    [Header("�v�b�V���L�[")] public KeyCode PushKey;
    [Header("�W�����v�L�[")] public KeyCode JumpKey;
    [Header("�K�[�h�L�[")] public KeyCode GuardKey;
}
