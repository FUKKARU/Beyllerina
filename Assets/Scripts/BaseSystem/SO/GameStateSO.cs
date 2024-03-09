using UnityEngine;

namespace BaseSystem
{
    [CreateAssetMenu(menuName = "SO/GameState", fileName = "GameStateSO")]
    public class GameStateSO : ScriptableObject
    {
        #region QOL向上処理
        // 保存してある場所のパス
        public const string PATH = "SO/GameStateSO";

        // 実体
        private static GameStateSO _entity;
        public static GameStateSO Entity
        {
            get
            {
                // 初アクセス時にロードする
                if (_entity == null)
                {
                    _entity = Resources.Load<GameStateSO>(PATH);

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

        [Header("Vsyncをオンにするか")] public bool IsVsyncOn;
        [Header("Unityが目標とするFPS")] public int TargetFrameRate;
    }
}
