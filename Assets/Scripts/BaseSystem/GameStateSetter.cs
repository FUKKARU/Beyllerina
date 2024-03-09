using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseSystem
{
    public class GameStateSetter : MonoBehaviour
    {
        void Awake()
        {
            if (!GameStateSO.Entity.IsVsyncOn)
            {
                QualitySettings.vSyncCount = 0; // VSyncをOFFにする
            }

            Application.targetFrameRate = GameStateSO.Entity.TargetFrameRate; // ターゲットフレームレートを設定
        }
    }
}
