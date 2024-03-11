using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SelectTeam
{
    public class Mouse : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;

        Vector2 mPos;
        Vector3 screenSizeHalf;
        float rad;
        float previousRad;
        float tan = 0f;

        // Start is called before the first frame update
        void Start()
        {
            if (!GameManager.Instance.isUsingMouse)
            {
                this.enabled = false;
            }
            // 画面の縦横の半分 
            screenSizeHalf.x = Screen.width / 2f;
            screenSizeHalf.y = Screen.height / 2f;
            screenSizeHalf.z = 0f;

            // マウスの初期位置
            mPos = Input.mousePosition - screenSizeHalf;
            previousRad = Mathf.Atan2(mPos.x, mPos.y);
        }

        // Update is called once per frame
        void Update()
        {
            // 真ん中が(0,0,0)になるようにマウスの位置を取得
            mPos = Input.mousePosition - screenSizeHalf;

            float rad = Mathf.Atan2(mPos.x, mPos.y); // 上向きとマウス位置のなす角
            float dRad = rad - previousRad; // 前のフレームの角度との差

            tan += Mathf.Tan(dRad); //タンジェント // * mPos.magnitude;
            text.text = tan + "";
            SceneChange.rotatenumber = tan ;

            previousRad = rad; // 今のフレームの角度を保存
        }
    }

}
