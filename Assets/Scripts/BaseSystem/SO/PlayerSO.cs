using System;
using UnityEngine;

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

        [Header("ログを出すかどうか（WarningとErrorは除く）")] public bool IsShowNormalLog;
        [Header("地面として使うオブジェクトのレイヤー")] public LayerMask WhatIsGround;
        [Header("ベイのタグ")] public string BeyTagName;
        [Header("重力を何倍にするか")] public float GravityScale;
        [Header("プレイヤーの高さ")] public float PlayerHeight;
        [Header("中心への移動速度")] public float SpeedTowardCenter;
        [Space(25)]
        [Header("【挙動関係】")] public BehaviourTable BehaviourTable;
        [Header("【ダメージ処理関係】")] public DamageTable DamageTable;
    }

    #region Table
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
        [Header("ダメージ＝\r\n（基礎ダメージ）＊\r\n（ステータス補正値）＊\r\n（状態補正値）＊\r\n（ダメージ係数）")]
        [Space(50)]
        [Header("基礎ダメージ＝\r\n（｜自分の運動量｜＋｜相手の運動量｜）")]
        [Header("ステータス補正値＝\r\n（重量補正値）＊\r\n（回転速度補正値）＊\r\n（ノックバック耐性補正値）＊\r\n（体力補正値）")]
        [Header("状態補正値＝\r\n（IDLEなら1）、\r\n（PUSHなら0.5）、\r\n（COUNTERなら0）、\r\n（KNOCKBACKEDなら2）")]
        [Header("ダメージ係数：調整用")]
        [Space(50)]
        [Header("（※値をx、デフォルト値をd、上記の値をY,Xとする。）\r\n（※ k = (-2/d)log(2 - Y) ）\r\n（※ m = (-k)exp(k(X - d)) ）\r\n重量補正値＝\r\n回転速度補正値＝\r\nノックバック耐性補正値＝\r\n（x < d なら -exp(k(x - d)) + 2）、\r\n（そうでないなら m(x - d) + 1）")]
        [Header("体力補正値＝\r\n（歳差運動をしているなら0.9）、\r\n（そうでないなら1）")]
        [Space(50)]

        [Header("何もしない定数")] public int Tmp;
    }
    #endregion
}
