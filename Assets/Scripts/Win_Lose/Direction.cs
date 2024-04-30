using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Win_Lose
{
    public class Direction : MonoBehaviour
    {
        GameObject cam;

        Vector3 startPos, endPos;
        Quaternion startRot, endRot;

        float time = 0;

        void Start()
        {
            cam = Camera.main.gameObject;

            startPos = WinLoseSO.Entity.CameraDir.StartPosition;
            endPos = WinLoseSO.Entity.CameraDir.EndPosition;
            startRot = Quaternion.Euler(WinLoseSO.Entity.CameraDir.StartRotation);
            endRot = Quaternion.Euler(WinLoseSO.Entity.CameraDir.EndRotation);

            cam.transform.position = startPos;
            cam.transform.rotation = startRot;
        }

        void Update()
        {
            float d = WinLoseSO.Entity.CameraDir.Duration;

            if (time < d)
            {
                float t = time / d;

                cam.transform.position = Vector3.Slerp(startPos, endPos, t);
                cam.transform.rotation = Quaternion.Slerp(startRot, endRot, t);

                time += Time.deltaTime;
            }
            else
            {
                cam.transform.position = endPos;
                cam.transform.rotation = endRot;
            }
        }
    }
}