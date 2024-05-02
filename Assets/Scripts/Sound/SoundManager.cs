using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseSystem
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField, Tooltip("0:馬, 1:回復, 2:吹っ飛ばし, 3:プッシュ, 4:ヒット, 5:カウンター, 6:必殺技（チャージ）, 7:ヒット(必殺技), 8:スキルチャージ, 9:スキルアクション, 10～:声")]
        AudioClip[] clips;
        [SerializeField, Tooltip("0:ノーマルラウンド, 1:最終ラウンド")] AudioClip[] bgmClips;
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
        /// 0:馬, 1:回復, 2:吹っ飛ばし, 3:プッシュ, 4:ヒット, 5:カウンター, 6:必殺技（チャージ）, 7:ヒット(必殺技), 8:スキルチャージ, 9:スキルアクション, 10～:声
        /// </summary>
        public void PlaySE(int index)
        {
            if (index < 0 || clips.Length <= index)
            {
                Debug.Log("クリップが見つかりません。");
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