using UnityEngine;
using UnityEngine.SceneManagement;

public class time : MonoBehaviour
{
    private float step_time;    // �o�ߎ��ԃJ�E���g�p

    // Use this for initialization
    void Start()
    {
        step_time = 0.0f;       // �o�ߎ��ԏ�����
    }

    // Update is called once per frame
    void Update()
    {
        // �o�ߎ��Ԃ��J�E���g
        step_time += Time.deltaTime;

        // 10�b��ɉ�ʑJ�ځiGame�ֈړ��j
        if (step_time >= 10.0f)
        {
            SceneManager.LoadScene("Game");
        }
    }
}
