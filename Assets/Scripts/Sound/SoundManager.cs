using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField, Tooltip("0:�n, 1:��, 2:������΂�, 3:�v�b�V���q�b�g,")]
    AudioClip[] clips;
    [SerializeField] AudioSource seSource;
    [SerializeField] AudioSource bgmSource;

    public static SoundManager Instance{ get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 0:�n, 1:��, 2:������΂�, 3:�v�b�V���q�b�g,
    /// </summary>
    public void PlaySE(int index)
    {
        if (index < 0 || clips.Length <= index)
        {
            Debug.Log("�N���b�v��������܂���B");
            return;
        }

        seSource.PlayOneShot(clips[index]);
    }
}
