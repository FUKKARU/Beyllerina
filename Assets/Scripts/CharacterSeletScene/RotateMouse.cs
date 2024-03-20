using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RotateMouse : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    Vector2 mPos;
    Vector3 screenSizeHalf;
    float rad;
    float previousRad;
    float tan = 0f;
    float RotateNumber = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
        screenSizeHalf.x = Screen.width / 2f;
        screenSizeHalf.y = Screen.height / 2f;
        screenSizeHalf.z = 0f;

        
        mPos = Input.mousePosition - screenSizeHalf;
        previousRad = Mathf.Atan2(mPos.x, mPos.y);
    }

    // Update is called once per frame
    void Update()
    {
        // �^�񒆂�(0,0,0)�ɂȂ�悤�Ƀ}�E�X�̈ʒu���擾
        mPos = Input.mousePosition - screenSizeHalf;

        float rad = Mathf.Atan2(mPos.x, mPos.y); // ������ƃ}�E�X�ʒu�̂Ȃ��p
        float dRad = rad - previousRad; // �O�̃t���[���̊p�x�Ƃ̍�

        tan += Mathf.Tan(dRad); //�^���W�F���g // * mPos.magnitude;
        text.text = tan + "";

        previousRad = rad; // ���̃t���[���̊p�x��ۑ�

        if (tan * RotateNumber == 3f)
        {

        }
    }
}
