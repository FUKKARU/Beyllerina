using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Status/BreakDancer", fileName = "BreakDancerStatusSO")]
public class BreakDancerStatusSO : ScriptableObject
{
    #region QOL向上処理
    // 保存してある場所のパス
    public const string PATH = "SO/Status/BreakDancerStatusSO";

    // 実体
    private static BreakDancerStatusSO _entity;
    public static BreakDancerStatusSO Entity
    {
        get
        {
            // 初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<BreakDancerStatusSO>(PATH);

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

    public StatusSO Status;
}
