using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    //カウントダウン
    public float countdown = 10.0f;
    
    // Update is called once per frame
    void Update()
    {
        //時間をカウントダウンする
        countdown -= Time.deltaTime;

        //時間を表示する
        text.text = countdown.ToString("f1") + "秒";

    }
}
