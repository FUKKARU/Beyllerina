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
}
