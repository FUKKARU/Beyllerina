using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Win_Lose
{
    [CreateAssetMenu(menuName = "SO/Win_Lose/WinLoseSO", fileName = "WinLoseSO")]
    public class WinLoseSO : ScriptableObject
    {
        #region QOL向上処理
        // 保存してある場所のパス
        public const string PATH = "Win_Lose/WinLoseSO";

        // 実体
        private static WinLoseSO _entity;
        public static WinLoseSO Entity
        {
            get
            {
                // 初アクセス時にロードする
                if (_entity == null)
                {
                    _entity = Resources.Load<WinLoseSO>(PATH);

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

        [Header("【カメラの演出関係】")] public CameraDirectionTable CameraDir;
    }

    [Serializable]
    public class CameraDirectionTable
    {
        [Header("開始位置")] public Vector3 StartPosition;
        [FormerlySerializedAs("EndPositon"), Header("終了位置")] public Vector3 EndPosition;
        [Header("開始回転")] public Vector3 StartRotation;
        [Header("終了回転")] public Vector3 EndRotation;
        [Header("時間")] public float Duration;
    }
}