using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "SO/GameSO", fileName = "GameSO")]
public class GameSO : ScriptableObject
{
    #region QOL向上処理
    // 保存してある場所のパス
    public const string PATH = "SO/GameSO";

    // 実体
    private static GameSO _entity;
    public static GameSO Entity
    {
        get
        {
            // 初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<GameSO>(PATH);

                // ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError(PATH + " not found");
                }
            }

            return _entity;
        }
    }
    #endregion

    [Header("ラウンド数")] public byte RoundNum;
    [FormerlySerializedAs("sceneName"), Header("シーン名")] public SceneNameTable SceneName;
    [Header("Now Loading の文字が次のものに切り替わるまでの秒数")] public float NowLoadingTextDur;
}

[Serializable]
public class SceneNameTable
{
    [Header("タイトル")] public string Title;
    [Header("キャラ選択")] public string CharacterSelect;
    [Header("回転")] public string Rotate;
    [Header("ゲーム")] public string Game;
    [Header("勝利/敗北")] public string WinLose;
}