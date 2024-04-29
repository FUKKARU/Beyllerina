using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseSystem
{
    [CreateAssetMenu(menuName = "SO/BaseSystem/Player", fileName = "PlayerSO")]
    public class PlayerSO : ScriptableObject
    {
        #region QOL向上処理
        // 保存してある場所のパス
        public const string PATH = "BaseSystem/SO/PlayerSO";

        // 実体
        private static PlayerSO _entity;
        public static PlayerSO Entity
        {
            get
            {
                // 初アクセス時にロードする
                if (_entity == null)
                {
                    _entity = Resources.Load<PlayerSO>(PATH);

                    // ロード出来なかった場合はエラーログを表示
                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [Header("【デバッグ関係】")] public DebugTable Dbg;
        [Space(25)]
        [Header("地面として使うオブジェクトのレイヤー")] public LayerMask WhatIsGround;
        [Header("ベイのタグ")] public string BeyTagName;
        [Header("重力を何倍にするか")] public float GravityScale;
        [Header("プレイヤーの高さ")] public float PlayerHeight;
        [Header("中心への移動速度")] public float SpeedTowardCenter;
        [Header("カメラの'theta'の範囲(min, max)")] public Vector2 CameraThetaRange;
        [Header("カメラの'angle'の範囲(min, max)")] public Vector2 CameraAngleRange;
        [Header("カメラが動く軌跡の、円の半径")] public float CameraRadius;
        [Header("カメラがステージを1周するのにかかる時間[s]")] public float CameraDuration;
        [Header("自分のy座標に対する、名前UIのy座標(offset)")] public float NameUIOffset;
        [Header("勝利した瞬間から何秒後にHPを回復するか")] public float UntilHpRecoverDur;
        [Header("勝利時に、最大HP - 現在HPの何%分回復するか")] public float HpRecoverRatio;
        [Header("HPを回復してから何秒後にシーン遷移するか")] public float FromHpRecoverDur;
        [Header("K.O. の文字を何秒で画面の中央に持ってくるか")] public float KODur;
        [Header("K.O.になってから、何秒でクリック可能にするか")] public float OnKOClickDur;
        [Space(25)]
        [Header("【挙動関係】")] public BehaviourTable BehaviourTable;
        [Header("【ライトの演出関係】")] public LightDirectionTable LightDirectionTable;
        [Header("【ダメージ処理関係】")] public DamageTable DamageTable;
    }

    #region Table
    [Serializable]
    public class DebugTable
    {
        [Header("ログを出す（WarningとErrorは除く）")] public bool IsShowNormalLog;
        [Header("プレイアブルの受けるダメージを100倍にする")] public bool P_DamageMul;
        [Header("アンプレイアブルの受けるダメージを100倍にする")] public bool U_DamageMul;
        [Header("プレイアブルの受けるダメージを0倍にする")] public bool P_DamageImmune;
        [Header("アンプレイアブルの受けるダメージを0倍にする")] public bool U_DamageImmune;
        [Header("【取扱注意】\r\nプッシュ、カウンター、スキル、必殺技の\r\nクールタイムを1秒にし、\r\n必殺技の発動に必要なポイントを1にする。")] public bool IsInfinityAction;
    }

    [Serializable]
    public class BehaviourTable
    {
        [Header("ベイの中心軸（transform.up）を、\n地面の法線ベクトルに合わせる速度")] public float PlayerMainAxisChangeSpeed;
        [Header("回転軸の回転速度（°/秒）")] public float AxisRotateSpeed;
        [Header("回転軸の傾きを変更する間隔（秒）")] public float AxisSlopeChangeInterval;
        [Header("ベイの回転速度を何倍まで許容するか（min,max）")] public Vector2 RotationSpeedCoefRange;
        [Header("歳差運動を始める境界のHPを、最大HPの何倍にするか")] public float AxisSlopeStartHpCoef;
        [Header("歳差運動において、中心軸から何度傾けるか（min,max）")] public Vector2 AxisSlopRange;
        [Header("Rigidbody.drag（＝抵抗）を\r\nノックバック耐性の何倍にするか")] public float DragCoef;
        [Header("KNOCKBACKEDの処理において、\r\n自分と敵の運動量の大きさの和の、\n何倍の力を瞬間的に受けるか")] public float PowerCoefOnKnockbacked;
        [Header("KNOCKBACKEDの処理において、\r\n瞬間的に受ける力の最小値（＝最低保証）")] public float MinPowerOnKnockbacked;
        [Header("カウンター時に、自分のベイのノックバック耐性を何倍にするか")] public float KnockbackResistanceCoefOnCounter;
        [Header("プッシュ失敗後、何秒でIDLE状態に戻すか")] public float Duration2IdleOnPushFailed;
        [Header("カウンター失敗後、何秒でIDLE状態に戻すか")] public float Duration2IdleOnCounterFailed;
        [Header("Knockbacked状態から、何秒でIDLE状態に戻すか")] public float Duration2IdleWhenKnockbacked;
        [Header("クールタイムの処理を何秒ごとに行うか")] public float CooltimeBehaviourInterval;
        [Header("クールタイムが空けていないとき、Gaugeの透明度"), Range(0, 255)] public byte GaugeAOnCooltime;
        [Header("何秒おきにポイントを貯めるか")] public float PointDur;
        [Header("〇秒おきに貯めるポイントの量")] public int PointAmount;
        [Header("ボーナスポイントの量\r\n（要素数で、体力の判定境界を識別する。体力が少ない→多いの順）")] public int[] BonusPoint;
        [Header("Clampする r （半径）（min/max）")] public Vector2 ClampR;
        [Header("Clampする y （高さ）（min/max）")] public Vector2 ClampY;
        [Header("落下判定位置")] public float FallJudgeY;
        [Header("落下修正位置")] public float FallResetY;
        [Header("Playable respawn point tag")] public string P_RePosTag;
        [Header("UnPlayable respawn point tag")] public string U_RePosTag;
    }

    [Serializable]
    public class LightDirectionTable
    {
        [Header("ライトのデフォルトの色")] public Color LightNormalColor;
        [Header("ランタンのデフォルトのintensity")] public float LampNormalIntensity;
        [Header("必殺技発動時に、ライトを何色にするか")] public Color LightSpecialColor;
        [Header("必殺技発動時に、ランタンのintensityをいくらにするか")] public float LampSpecialIntensity;
    }

    [Serializable]
    public class DamageTable
    {
        [Header("ダメージを食らえる間隔（＝無敵時間）")] public float DamagableDuration;
        [Header("食らえるダメージの最小値")] public float MinDamage;
        [Header("食らえるダメージの最大値")] public float MaxDamage;
        
        [Header("状態補正値\r\n（IDLE/PUSH/COUNTER/KNOCKBACKED）")] public float[] StateAdjustValue;
        [Header("ダメージ係数")] public float DamageCoef;
        [Header("重量/回転速度/ノックバック耐性補正値で、\r\nx = d/2 の時の値")] public float MrkAdjustValueY;
        [Header("重量/回転速度/ノックバック耐性補正値で、\r\nx >= d の時と傾きが等しくなる x( < d) が、\r\n d の何倍かどうか")] public float MrkAdjustValueX;
        [Header("体力補正値（歳差運動をしている/していない）")] public float[] HpAdjustValue;

        [Header("ダメージ計算の詳細")] public DamageTableDescription DamageDescriptionTable;
    }

    [Serializable]
    public class DamageTableDescription
    {
        [Header("ダメージ＝\r\n（基礎ダメージ）＊\r\n（ステータス補正値）＊\r\n（状態補正値）＊\r\n（ダメージ係数） * \r\n（汎用ダメージ係数）")]
        [Space(50)]
        [Header("基礎ダメージ＝\r\n（｜自分の運動量｜＋｜相手の運動量｜）")]
        [Header("ステータス補正値＝\r\n（重量補正値）＊\r\n（回転速度補正値）＊\r\n（ノックバック耐性補正値）＊\r\n（体力補正値）")]
        [Header("状態補正値＝\r\n（IDLEなら1）、\r\n（PUSHなら0.5）、\r\n（COUNTERなら0）、\r\n（KNOCKBACKEDなら2）")]
        [Header("ダメージ係数：調整用")]
        [Header("汎用ダメージ係数：各ベイごとの調整用。内部的に計算される")]
        [Space(50)]
        [Header("（※値をx、デフォルト値をd、上記の値をY,Xとする。）\r\n（※ k = (-2/d)log(2 - Y) ）\r\n（※ m = (-k)exp(k(X - d)) ）\r\n重量補正値＝\r\n回転速度補正値＝\r\nノックバック耐性補正値＝\r\n（x < d なら -exp(k(x - d)) + 2）、\r\n（そうでないなら m(x - d) + 1）")]
        [Header("体力補正値＝\r\n（歳差運動をしているなら0.9）、\r\n（そうでないなら1）")]
        [Space(50)]

        [Header("何もしない定数")] public int Tmp;
    }
    #endregion
}
