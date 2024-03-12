using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameStateSetter
{
    [RuntimeInitializeOnLoadMethod]
    static void RuntimeInitializeOnLoadMethods()
    {
        #region【設定】
        SetVsync(); // Vsyncの設定
        SetTargetFrameRate(); // ターゲットフレームレートの設定
        #endregion

#if UNITY_EDITOR
        #region【LogError】
        CheckVsync(); // Vsyncの設定が成功したどうかを確認
        CheckTargetFrameRate(); // ターゲットフレームレートの設定が成功したかどうかを確認
        #endregion

        #region【LogWarning】
        #endregion

        #region【Log】
        ShowConfirmation(); // 確認メッセージを表示
        #endregion
#endif
    }

    #region メソッド達の詳細
    static void SetVsync()
    {
        if (!GameStateSO.Entity.IsVsyncOn)
        {
            QualitySettings.vSyncCount = 0; // VSyncをOFFにする
        }
    }

    static void SetTargetFrameRate()
    {
        Application.targetFrameRate = GameStateSO.Entity.TargetFrameRate;
    }

    static void CheckVsync()
    {
        if (!GameStateSO.Entity.IsVsyncOn && QualitySettings.vSyncCount != 0)
        {
            Debug.LogError("<color=red>Vsyncがオフになっていません</color>");
        }
        else if (GameStateSO.Entity.IsVsyncOn && QualitySettings.vSyncCount == 0)
        {
            Debug.LogError("<color=red>Vsyncがオンになっていません</color>");
        }
    }

    static void CheckTargetFrameRate()
    {
        if (Application.targetFrameRate != GameStateSO.Entity.TargetFrameRate)
        {
            Debug.LogError("<color=red>ターゲットフレームレートが設定できていません</color>");
        }
    }

    static void ShowConfirmation()
    {
        Debug.Log("<color=cyan>確認してください：BaseSystem：各プレイヤーのPlayerMove.cs内の、typeが正しいかどうか</color>");
        Debug.Log("<color=cyan>確認してください：BaseSystem：GameManager.cs内の、BeysとTextsの要素の順番が一致しているかどうか</color>");
    }
    #endregion
}

// SOのバグを告知
[InitializeOnLoad]
public class AnnounceSOBug
{
    static bool errorShown = false; // エラーメッセージを表示したかどうかのフラグ

    static AnnounceSOBug()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode && !errorShown)
        {
            // 選択されているアセットを取得
            Object[] selectedAssets = Selection.objects;

            // 選択されたアセットをチェック
            foreach (Object selectedAsset in selectedAssets)
            {
                // 選択されているアセットが ScriptableObject であるかを確認
                if (selectedAsset is ScriptableObject)
                {
                    Debug.LogWarning("<color=yellow>Scriptable Object を選択した状態でゲームを実行すると、エラーが出る場合があります。\r\nこれは現状Unity2022以降で発生しているバグであり、自分が確認する限りでは実害はありません。</color>");
                    Debug.LogWarning("<color=yellow>参考記事：https://www.create-forever.games/unity2022-3-value-cannot-be-null-_unity_self/</color>");
                    errorShown = true;
                    break;
                }
            }
        }
    }
}
