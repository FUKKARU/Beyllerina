using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SelectTeam
{
    public class CountDown : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        float countdown; //カウントダウン

        void Start()
        {
            countdown = GameManager.Instance.time;
        }

        void Update()
        {
            //時間をカウントダウンする
            countdown -= Time.deltaTime;

            //時間を表示する
            text.text = countdown.ToString("f1") + "秒";
        }
    }
}
