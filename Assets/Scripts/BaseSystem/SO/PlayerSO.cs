using UnityEngine;

namespace BaseSystem
{
    [CreateAssetMenu(menuName = "SO/Player", fileName = "PlayerSO")]
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
        [Header("ベイの中心軸（transform.up）を、\n地面の法線ベクトルに合わせる速度")] public float PlayerMainAxisChangeSpeed;
        [Header("回転軸の回転速度（°/秒）")] public float AxisRotateSpeed;
        [Header("回転軸の傾きを変更する間隔（秒）")] public float AxisSlopeChangeInterval;
        [Header("ベイの回転速度を何倍まで許容するか（min,max）")] public Vector2 RotationSpeedCoefRange;
        [Header("歳差運動を始める境界のHPを、最大HPの何倍にするか")] public float AxisSlopeStartHpCoef;
        [Header("歳差運動において、中心軸から何度傾けるか（min,max）")] public Vector2 AxisSlopRange;
        [Header("Rigidbody.drag（＝抵抗）を\r\nノックバック耐性の何倍にするか")] public float DragCoef;
        [Header("KNOCKBACKEDの処理において、\r\n自分と敵の運動量の大きさの和の、\n何倍の力を瞬間的に受けるか")] public float PowerCoefOnKnockbacked;
        [Header("KNOCKBACKEDの処理において、\r\n瞬間的に受ける力の最小値（＝最低保証）")] public float MinPowerOnKnockbacked;
        [Header("ダメージを食らえる間隔（＝無敵時間）")] public float DamagableDuration;
        [Header("ぶつかった時、相手の速度の何倍のダメージを食らうか")] public float DamageCoef;
        [Header("プッシュ時、受けるダメージを何倍にするか")] public float DamageCoefOnPush;
        [Header("ノックバックされた時、受けるダメージを何倍にするか")] public float DamageCoefOnKnockbacked;
        [Header("カウンター時に、自分のベイのノックバック耐性を何倍にするか")] public float KnockbackResistanceCoefOnCounter;
        [Header("プッシュ失敗後、何秒でIDLE状態に戻すか")] public float Duration2IdleOnPushFailed;
        [Header("カウンター失敗後、何秒でIDLE状態に戻すか")] public float Duration2IdleOnCounterFailed;
        [Header("Knockbacked状態から、何秒でIDLE状態に戻すか")] public float Duration2IdleWhenKnockbacked;
    }
}
