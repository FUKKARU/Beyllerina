using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    public GameObject[] Beys;
    public TextMeshProUGUI[] Texts;

    void Start()
    {
        // 確認ログを表示
        Debug.Log("<color=cyan>確認して下さい：各プレイヤーのPlayerMove.cs内の、typeが正しいかどうか</color>");
        Debug.Log("<color=cyan>確認して下さい：GameManager.cs内の、BeysとTextsの要素の順番が一致しているかどうか</color>");
    }
}
