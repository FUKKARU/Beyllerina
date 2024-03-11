using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    //�J�E���g�_�E��
    public float countdown = 10.0f;
    
    // Update is called once per frame
    void Update()
    {
        //���Ԃ��J�E���g�_�E������
        countdown -= Time.deltaTime;

        //���Ԃ�\������
        text.text = countdown.ToString("f1") + "�b";

    }
}
