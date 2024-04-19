using UnityEngine;

namespace BaseSystem
{
    [CreateAssetMenu(menuName = "SO/BaseSystem/Status/Enemy1", fileName = "Enemy1StatusSO")]
    public class Enemy1StatusSO : ScriptableObject
    {
        #region QOL向上処理
        // 保存してある場所のパス
        public const string PATH = "BaseSystem/SO/Status/Enemy1StatusSO";

        // 実体
        private static Enemy1StatusSO _entity;
        public static Enemy1StatusSO Entity
        {
            get
            {
                // 初アクセス時にロードする
                if (_entity == null)
                {
                    _entity = Resources.Load<Enemy1StatusSO>(PATH);

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
        [Header("スキル使用時何倍大きくなるか")] public float SkillSizeCoef;
        [Header("スキル使用時どこまで上昇するか")] public float SkillHeight;
        [Header("スキル使用時どのようなスピードで上昇するか")] public float SkillSpeed;
        [Header("スキルの巨大化を何秒継続するか")] public float a;
    }
}
