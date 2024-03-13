using UnityEngine;

[CreateAssetMenu(menuName = "SO/GameState", fileName = "GameStateSO")]
public class GameStateSO : ScriptableObject
{
    #region QOL向上処理
    // 保存してある場所のパス
    public const string PATH = "GameStateSO";

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

    [Header("解像度（横幅）")] public int ResolutionH;
    [Header("解像度（縦幅）")] public int ResolutionV;
    [Header("フルスクリーンにするか")] public bool IsFullScreen;
    [Header("Vsyncをオンにするか")] public bool IsVsyncOn;
    [Header("（Vsyncがオフの場合に）Unityが目標とするFPS")] public int TargetFrameRate;
}
