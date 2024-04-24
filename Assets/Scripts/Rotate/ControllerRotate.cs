using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SelectTeam
{
    public class ControllerRotate : MonoBehaviour
    {
        [SerializeField] GameObject stickSphere;
        [SerializeField] GameObject stickSphereCenter;
        [SerializeField] TextMeshProUGUI text;
        bool isFirstInput = true;
        float preDeg = 0; // 前フレームの角度
        float increaseSpeed = 1;

        void Update()
        {
            Vector2 rot = IA.InputGetter.Instance.ValueRotate;
            float deg = Mathf.Atan2(rot.y, rot.x) * Mathf.Rad2Deg;

            if (isFirstInput)
            {
                isFirstInput = false;
            }
            else
            {
                float deltaDeg = deg - preDeg;
                increaseSpeed += Mathf.Abs(deltaDeg) * 0.025f / 360;
            }

            preDeg = deg;

            Vector3 targetPos = rot;
            targetPos.z = -0.3f;
            stickSphere.transform.localPosition = targetPos;

            text.text = $"× {increaseSpeed.ToString("f2")}";
        }

        private void OnDisable()
        {
            RotData.RotateNumber = increaseSpeed;
        }
    }
}
