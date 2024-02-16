using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Player", fileName = "PlayerSO")]
public class PlayerSO : ScriptableObject
{
    #region QOL向上処理
    // 保存してある場所のパス
    public const string PATH = "SO/PlayerSO";

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

    [Header("地面として使うオブジェクトのレイヤー")] public LayerMask WhatIsGround;
    [Header("重力を何倍にするか")] public float GravityScale;
    [Header("プレイヤーの高さ")] public float PlayerHeight;
    [Header("中心への移動速度")] public float SpeedTowardCenter;
    [Header("ベイの中心軸（transform.up）を、\n地面の法線ベクトルに合わせる速度")] public float PlayerMainAxisChangeSpeed;
    [Header("回転軸の回転速度（°/秒）")] public float AxisRotateSpeed;
    [Header("回転軸の傾きを変更する間隔（秒）")] public float AxisSlopeChangeInterval;
    [Header("ベイの回転速度を何倍まで許容するか（min,max）")] public Vector2 RotationSpeedCoefRange;
    [Header("歳差運動を始める境界のHPを、最大HPの何倍にするか")] public float AxisSlopeStartHpCoef;
    [Header("歳差運動において、中心軸から何度傾けるか（min,max）")] public Vector2 AxisSlopRange;
    [Header("Rigidbody.drag（＝抵抗）を\nノックバック耐性の何倍にするか")] public float DragCoef;
    [Header("ぶつかった時、相手の速度の何倍のダメージを食らうか")] public float DamageCoef;
    [Header("プッシュ中、受けるダメージを何倍にするか")] public float DamageCoefOnPush;
    [Header("ガード時に、ベイのノックバック耐性を何倍にするか")] public float KnockbackResistanceCoefOnGuard;
}
