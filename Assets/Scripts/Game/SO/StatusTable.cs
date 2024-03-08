using UnityEngine;

[System.Serializable]
public class StatusTable
{
    [Header("ステータスの初期値設定")]
    [Space(25)]
    [Header("名前")] public string Name;
    [Header("スキルの名前")] public string SkillName;
    [Header("必殺技の名前")] public string SpecialName;
    [Header("体力")] public float Hp;
    [Header("重量")] public float Weight;
    [Header("回転速度")] public float RotationSpeed;
    [Header("プッシュ力")] public float PushPower;
    [Header("ノックバック耐性"), Range(0, 1)] public float KnockbackResistance;
    [Header("必殺技の発動に必要なゲージ量")] public float GaugeAmountUntilSpecial;
    [Header("プッシュのクールタイム")] public float PushCoolTime;
    [Header("スキルのクールタイム")] public float SkillCooltime;
    [Header("必殺技のクールタイム")] public float SpecialCooltime;
    [Header("プッシュキー")] public KeyCode PushKey;
    [Header("カウンターキー")] public KeyCode CounterKey;
}
