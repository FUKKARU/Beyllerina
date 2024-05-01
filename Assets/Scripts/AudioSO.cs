using System;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "SO/AudioSO", fileName = "AudioSO")]
public class AudioSO : ScriptableObject
{
    #region QOL向上処理
    // 保存してある場所のパス
    public const string PATH = "AudioSO";

    // 実体
    private static AudioSO _entity;
    public static AudioSO Entity
    {
        get
        {
            // 初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<AudioSO>(PATH);

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

    [Header("Audio Mixer")] public AudioMixer AudioMixer;
    [Header("【オーディオの種類ごとの情報】")] public AudioTypeTable[] Audioes;
    [Header("音量(db)を、いくつずつ変えるか")] public float VolumeChangeStep;
    [Header("音量テキストを何秒間表示するか")] public float VolumeTextShowDur;

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
    [Header("種類")] public AudioSO.AudioType Type;
    [Header("どのオーディオグループか")] public AudioMixerGroup Grouop;
    [Header("音量調節の変数名")] public string Param;
}