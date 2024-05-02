using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "SO/GameSO", fileName = "GameSO")]
public class GameSO : ScriptableObject
{
    #region QOL���㏈��
    // �ۑ����Ă���ꏊ�̃p�X
    public const string PATH = "SO/GameSO";

    // ����
    private static GameSO _entity;
    public static GameSO Entity
    {
        get
        {
            // ���A�N�Z�X���Ƀ��[�h����
            if (_entity == null)
            {
                _entity = Resources.Load<GameSO>(PATH);

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

    [Header("���E���h��")] public byte RoundNum;
    [FormerlySerializedAs("sceneName"), Header("�V�[����")] public SceneNameTable SceneName;
    [Header("Now Loading �̕��������̂��̂ɐ؂�ւ��܂ł̕b��")] public float NowLoadingTextDur;
}

[Serializable]
public class SceneNameTable
{
    [Header("�^�C�g��")] public string Title;
    [Header("�L�����I��")] public string CharacterSelect;
    [Header("��]")] public string Rotate;
    [Header("�Q�[��")] public string Game;
    [Header("����/�s�k")] public string WinLose;
}