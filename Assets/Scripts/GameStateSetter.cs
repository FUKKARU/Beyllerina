using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
