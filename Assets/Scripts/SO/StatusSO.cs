using UnityEngine;

[System.Serializable]
public class StatusSO
{
    [Header("ステータスの初期値設定")]
    [Header("デバッグ用：ジャンプ機能をオフにする")] public bool DebugIsJumpDisable;
    [Header("デバッグ用：ガード機能をオフにする")] public bool DebugIsGuardDisable;
    [Space(25)]
    [Header("名前")] public string Name;
    [Header("スキルの名前")] public string SkillName;
    [Header("必殺技の名前")] public string SpecialName;
    [Header("体力")] public float Hp;
    [Header("重量")] public float Weight;
    [Header("回転速度")] public float RotationSpeed;
    [Header("プッシュ力")] public float PushPower;
    [Header("ジャンプ力")] public float JumpPower;
    [Header("クリティカルプッシュ力")] public float CriticalPushPower;
    [Header("ガード耐久値")] public float GuardDurability;
    [Header("ノックバック耐性"), Range(0, 1)] public float KnockbackResistance;
    [Header("必殺技の発動に必要なゲージ量")] public float GaugeAmountUntilSpecial;
    [Header("プッシュのクールタイム")] public float PushCoolTime;
    [Header("ジャンプのクールタイム")] public float JumpCooltime;
    [Header("スキルのクールタイム")] public float SkillCooltime;
    [Header("必殺技のクールタイム")] public float SpecialCooltime;
    [Header("前進キー")] public KeyCode ForwardKey;
    [Header("左進キー")] public KeyCode LeftKey;
    [Header("後進キー")] public KeyCode BackKey;
    [Header("右進キー")] public KeyCode RightKey;
    [Header("プッシュキー")] public KeyCode PushKey;
    [Header("ジャンプキー")] public KeyCode JumpKey;
    [Header("ガードキー")] public KeyCode GuardKey;
}
