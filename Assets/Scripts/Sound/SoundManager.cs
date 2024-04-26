using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField, Tooltip("0:馬, 1:回復, 2:吹っ飛ばし, 3:プッシュヒット,")]
    AudioClip[] clips;
    [SerializeField] AudioSource seSource;
    [SerializeField] AudioSource bgmSource;

    public static SoundManager Instance{ get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 0:馬, 1:回復, 2:吹っ飛ばし, 3:プッシュヒット,
    /// </summary>
    public void PlaySE(int index)
    {
        if (index < 0 || clips.Length <= index)
        {
            Debug.Log("クリップが見つかりません。");
            return;
        }

        seSource.PlayOneShot(clips[index]);
    }

    public void PushSE(bool isPlayable)
    {

    }

    public void CounterSE(bool isPlayable)
    {

    }

    public void SkillSE(bool isPlayable)
    {

    }

    /// <summary>
    /// 必殺技SE
    /// </summary>
    /// <param name="index">0.強化状態に 1.衰弱状態に 2.必殺技が終了 3.剣がぶつかる</param>
    public void SpecialSE(int index)
    {
        switch (index)
        {
            case 0:
                break;

            case 1:
                break;

            case 2:
                break;

            case 3:
                break;
        }
    }
}
