using System.Collections;
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
            SceneManager.LoadScene("Game");
        }
    }
}
