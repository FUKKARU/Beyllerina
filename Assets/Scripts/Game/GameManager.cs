using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    void Start()
    {
        // �m�F���O�̕\��
        ShowConfirmationLog();
    }

    // �m�F���O�̕\���̏ڍ�
    void ShowConfirmationLog()
    {
        if (PlayerSO.Entity.IsShowNormalLog)
        {
            Debug.Log("<color=cyan>�m�F���ĉ������F�e�v���C���[��PlayerMove.cs���́Atype�����������ǂ���</color>");
            Debug.Log("<color=cyan>�m�F���ĉ������FGameManager.cs���́ABeys��Texts�̗v�f�̏��Ԃ���v���Ă��邩�ǂ���</color>");
        }
    }
}
