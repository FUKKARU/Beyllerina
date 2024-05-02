using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseSystem
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField, Tooltip("0:�n, 1:��, 2:������΂�, 3:�v�b�V��, 4:�q�b�g, 5:�J�E���^�[, 6:�K�E�Z�i�`���[�W�j, 7:�q�b�g(�K�E�Z), 8:�X�L���`���[�W, 9:�X�L���A�N�V����, 10�`:��")]
        AudioClip[] clips;
        [SerializeField, Tooltip("0:�m�[�}�����E���h, 1:�ŏI���E���h")] AudioClip[] bgmClips;
        [SerializeField] AudioSource seSource;
        [SerializeField] AudioSource bgmSource;

        bool isSpecial;

        public static SoundManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            isSpecial = false;
        }

        private void Start()
        {
            if ((GameData.GameData.PlayableRoundNum >= GameSO.Entity.RoundNum - 1) || (GameData.GameData.UnPlayableRoundNum >= GameSO.Entity.RoundNum - 1))
            {
                PlayBGM(true);
            }
            else
            {
                PlayBGM(false);
            }
        }

        /// <summary>
        /// 0:�n, 1:��, 2:������΂�, 3:�v�b�V��, 4:�q�b�g, 5:�J�E���^�[, 6:�K�E�Z�i�`���[�W�j, 7:�q�b�g(�K�E�Z), 8:�X�L���`���[�W, 9:�X�L���A�N�V����, 10�`:��
        /// </summary>
        public void PlaySE(int index)
        {
            if (index < 0 || clips.Length <= index)
            {
                Debug.Log("�N���b�v��������܂���B");
                return;
            }

            if (clips[index] != null) seSource.PlayOneShot(clips[index]);
        }

        public void HitSE()
        {
            if (isSpecial)
            {
                seSource.PlayOneShot(clips[7]);
            }
            else
            {
                seSource.PlayOneShot(clips[4]);
            }
        }

        public void Special(bool which)
        {
            isSpecial = which;
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
        /// �K�E�ZSE
        /// </summary>
        /// <param name="index">0.������Ԃ� 1.�����Ԃ� 2.�K�E�Z���I�� 3.�����Ԃ���</param>
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

        public void PlayBGM(bool isReach)
        {
            AudioClip clip = isReach ? bgmClips[1] : bgmClips[0];
            bgmSource.clip = clip;
            bgmSource.volume = isReach ? 0.125f : 0.25f;
            bgmSource.Play();
        }

        public void PlayBGM(int index)
        {
            bgmSource.Stop();
            bgmSource.clip = bgmClips[index];
            bgmSource.Play();
        }
    }
}