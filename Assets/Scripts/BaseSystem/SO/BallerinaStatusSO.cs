using UnityEngine;

namespace BaseSystem
{
    [CreateAssetMenu(menuName = "SO/Status/Ballerina", fileName = "BallerinaStatusSO")]
    public class BallerinaStatusSO : ScriptableObject
    {
        #region QOL���㏈��
        // �ۑ����Ă���ꏊ�̃p�X
        public const string PATH = "SO/Status/BallerinaStatusSO";

        // ����
        private static BallerinaStatusSO _entity;
        public static BallerinaStatusSO Entity
        {
            get
            {
                // ���A�N�Z�X���Ƀ��[�h����
                if (_entity == null)
                {
                    _entity = Resources.Load<BallerinaStatusSO>(PATH);

                    // ���[�h�o���Ȃ������ꍇ�̓G���[���O��\��
                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        public StatusTable Status;
    }
}
