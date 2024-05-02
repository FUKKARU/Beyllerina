using UnityEngine;

namespace BaseSystem
{
    [CreateAssetMenu(menuName = "SO/BaseSystem/Status/BreakDancer", fileName = "BreakDancerStatusSO")]
    public class BreakDancerStatusSO : ScriptableObject
    {
        #region QOL向上処理
        // 保存してある場所のパス
        public const string PATH = "SO/BaseSystem/Status/BreakDancerStatusSO";

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

        public StatusTable StatusTable;
        [Header("スキル使用時に、回転方向を何秒で逆にするか")] public float SkillRotChangeDur;
        [Header("スキルの継続時間")] public float SkillDur;
        [Header("必殺技使用時に、プッシュ力を何倍にするか")] public float SpecialPushPowerCoefCoef;
        [Header("必殺技使用時に、回転速度を何倍にするか")] public float SpecialRotSpeedCoef;
        [Header("必殺技の継続時間")] public float SpecialDur;
    }
}
