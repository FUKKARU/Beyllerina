using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameStateSetter
{
    [RuntimeInitializeOnLoadMethod]
    static void RuntimeInitializeOnLoadMethods()
    {
        SetResolutionAndFullScreenMode(); // 解像度とフルスクリーンにするかどうかを設定
        SetVsyncAndTargetFrameRate(); // Vsync（とターゲットフレームレート）の設定
    }

    #region 詳細
    static void SetResolutionAndFullScreenMode()
    {
        Screen.SetResolution(GameStateSO.Entity.ResolutionH, GameStateSO.Entity.ResolutionV, GameStateSO.Entity.IsFullScreen);
    }

    static void SetVsyncAndTargetFrameRate()
    {
        if (GameStateSO.Entity.IsVsyncOn)
        {
            QualitySettings.vSyncCount = 1; // VSyncをONにする
        }
        else
        {
            QualitySettings.vSyncCount = 0; // VSyncをOFFにする
            Application.targetFrameRate = GameStateSO.Entity.TargetFrameRate; // ターゲットフレームレートの設定
        }
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
