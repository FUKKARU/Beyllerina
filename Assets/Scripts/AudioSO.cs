using System;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "SO/AudioSO", fileName = "AudioSO")]
public class AudioSO : ScriptableObject
{
    #region QOL���㏈��
    // �ۑ����Ă���ꏊ�̃p�X
    public const string PATH = "AudioSO";

    // ����
    private static AudioSO _entity;
    public static AudioSO Entity
    {
        get
        {
            // ���A�N�Z�X���Ƀ��[�h����
            if (_entity == null)
            {
                _entity = Resources.Load<AudioSO>(PATH);

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

    [Header("Audio Mixer")] public AudioMixer AudioMixer;
    [Header("�y�I�[�f�B�I�̎�ނ��Ƃ̏��z")] public AudioTypeTable[] Audioes;
    [Header("����(db)���A�������ς��邩")] public float VolumeChangeStep;
    [Header("���ʃe�L�X�g�����b�ԕ\�����邩")] public float VolumeTextShowDur;

    public enum AudioType { BGM, SE, SYSTEM };
    public string GetAudioParam(AudioType type)
    {
        foreach (AudioTypeTable e in Audioes)
        {
            if (type == e.Type)
            {
                return e.Param;
            }
        }

        return null;
    }
}

[Serializable]
public class AudioTypeTable
{
    [Header("���")] public AudioSO.AudioType Type;
    [Header("�ǂ̃I�[�f�B�I�O���[�v��")] public AudioMixerGroup Grouop;
    [Header("���ʒ��߂̕ϐ���")] public string Param;
}