using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BaseSystem
{
    public class GameManager : MonoBehaviour
    {
        #region static���V���O���g���ɂ���
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

        [Header("Beys��Texts�̗v�f�̏��Ԃ���v�����邱�ƁI")]
        [Header("�A�^�b�`����")] public GameObject[] Beys;
        [Header("�A�^�b�`����")] public TextMeshProUGUI[] Texts;

        [NonSerialized] public bool IsGameEnded = false; // �Q�[�����I�����Ă��邩�ǂ���
    }
}
