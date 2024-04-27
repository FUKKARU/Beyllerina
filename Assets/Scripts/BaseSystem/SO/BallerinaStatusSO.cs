using UnityEngine;

namespace BaseSystem
{
    [CreateAssetMenu(menuName = "SO/BaseSystem/Status/Ballerina", fileName = "BallerinaStatusSO")]
    public class BallerinaStatusSO : ScriptableObject
    {
        #region QOL向上処理
        // 保存してある場所のパス
        public const string PATH = "BaseSystem/SO/Status/BallerinaStatusSO";

        // 実体
        private static BallerinaStatusSO _entity;
        public static BallerinaStatusSO Entity
        {
            get
            {
                // 初アクセス時にロードする
                if (_entity == null)
                {
                    _entity = Resources.Load<BallerinaStatusSO>(PATH);

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
        [Header("スキル発動時に何倍の力を加えるか")] public float SkillPushPowerCoef;
        [Header("スキル発動時の上昇時間")] public float SkillRiseDur;
        [Header("スキル発動時の上昇高度")] public float SkillRiseHeight;
        [Header("スキル発動時の下降時間")] public float SkillRushDur;
        [Header("スキル発動時の時間指数")] public int  SkillPow;
        [Header("必殺技発動時に与えるダメージを何倍にするか")] public float GenericDamageCoefCoef;
        [Header("必殺技の継続時間")] public float SpecialDur;
        [Header("衰弱時に、与えるダメージを何分の一にするか（分母）")] public float OnWeakGenericDamageCoefCoef;
        [Header("衰弱状態の継続時間")] public float WeakDur;
    }
}
