using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SelectTeam
{
    public class time : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(CountAndChangeScene(GameManager.Instance.time));
        }

        IEnumerator CountAndChangeScene(float time)
        {
            yield return new WaitForSeconds(time);
            LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Game, true);
        }
    }
}
