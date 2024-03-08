using UnityEngine;

[System.Serializable]
public class StatusTable
{
    [Header("�X�e�[�^�X�̏����l�ݒ�")]
    [Space(25)]
    [Header("���O")] public string Name;
    [Header("�X�L���̖��O")] public string SkillName;
    [Header("�K�E�Z�̖��O")] public string SpecialName;
    [Header("�̗�")] public float Hp;
    [Header("�d��")] public float Weight;
    [Header("��]���x")] public float RotationSpeed;
    [Header("�v�b�V����")] public float PushPower;
    [Header("�m�b�N�o�b�N�ϐ�"), Range(0, 1)] public float KnockbackResistance;
    [Header("�K�E�Z�̔����ɕK�v�ȃQ�[�W��")] public float GaugeAmountUntilSpecial;
    [Header("�v�b�V���̃N�[���^�C��")] public float PushCoolTime;
    [Header("�X�L���̃N�[���^�C��")] public float SkillCooltime;
    [Header("�K�E�Z�̃N�[���^�C��")] public float SpecialCooltime;
    [Header("�v�b�V���L�[")] public KeyCode PushKey;
    [Header("�J�E���^�[�L�[")] public KeyCode CounterKey;
}
