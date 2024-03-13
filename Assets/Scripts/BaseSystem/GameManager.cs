using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BaseSystem
{
    public class GameManager : MonoBehaviour
    {
        #region staticかつシングルトンにする
        public static GameManager Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        [Header("BeysとTextsの要素の順番を一致させること！")]
        [Header("アタッチする")] public GameObject[] Beys;
        [Header("アタッチする")] public TextMeshProUGUI[] Texts;

        [NonSerialized] public bool IsGameEnded = false; // ゲームが終了しているかどうか
    }
}
