using System;
using UnityEngine;

namespace BaseSystem
{
    [Serializable]
    public class StatusTable
    {
        [Header("プレイアブルかどうか")] public bool IsPlayable;
        [Space(25)]
        [Header("【名前】")] public StatusTableName StatusTableName;
        [Header("【プレイアブル】")] public StatusTablePlayable StatusTablePlayable;
        [Header("【アンプレイアブル】")] public StatusTableUnPlayable StatusTableUnPlayable;
        [Header("【ステータス情報（初期値）】")] public StatusTableInitStatus StatusTableInitStatus;
    }

    [Serializable]
    public class StatusTableName
    {
        [Header("名前")] public string Name;
        [Header("スキルの名前")] public string[] SkillNames;
        [Header("必殺技の名前")] public string SpecialName;
    }

    [Serializable]
    public class StatusTablePlayable
    {
        [Header("使うスキルの数")] public int SkillNum;
        [Header("HPバーの減少速度")] public float HpBarChangeSpeed;
    }

    [Serializable]
    public class StatusTableUnPlayable
    {
        [Header("使うスキルの数（ランダム）")] public int SkillNum;
        [Header("プッシュ→スキル→プッシュ→…")]
        [Header("プッシュ→スキルの間隔")] public float Push2SkillInterval;
        [Header("プッシュ→スキルの間隔の誤差（±）")] public float Push2SkillIntervalOffset;
        [Header("スキル→プッシュの間隔")] public float Skill2PushInterval;
        [Header("スキル→プッシュの間隔の誤差（±）")] public float Skill2PushIntervalOffset;
        [Header("HPバーの減少速度")] public float HpBarChangeSpeed;
    }

    [Serializable]
    public class StatusTableInitStatus
    {
        [Header("体力")] public float Hp;
        [Header("重量")] public float Weight;
        [Header("回転速度")] public float RotationSpeed;
        [Header("プッシュ力")] public float PushPower;
        [Header("ノックバック耐性"), Range(0, 1)] public float KnockbackResistance;
        [Header("必殺技の発動に必要なポイント量")] public int SpecialPoint;
        [Header("プッシュのクールタイム")] public float PushCoolTime;
        [Header("カウンターのクールタイム")] public float CounterCoolTime;
        [Header("スキルのクールタイム")] public float[] SkillCooltimes;
        [Header("必殺技のクールタイム")] public float SpecialCooltime;
    }
}
