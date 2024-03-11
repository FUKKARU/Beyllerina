using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SelectTeam
{
    public class CountDown : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        float countdown; //�J�E���g�_�E��

        void Start()
        {
            countdown = GameManager.Instance.time;
        }

        void Update()
        {
            //���Ԃ��J�E���g�_�E������
            countdown -= Time.deltaTime;

            //���Ԃ�\������
            text.text = countdown.ToString("f1") + "�b";
        }
    }
}
