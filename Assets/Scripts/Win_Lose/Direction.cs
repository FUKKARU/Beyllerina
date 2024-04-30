using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Win_Lose
{
    public class Direction : MonoBehaviour
    {
        [SerializeField] GameObject playable;
        [SerializeField] GameObject unPlayable;
        [SerializeField] GameObject victoryUI;
        [SerializeField] GameObject defeatUI;

        GameObject cam;

        Vector3 startPos, endPos;
        Quaternion startRot, endRot;

        void Start()
        {
            if (BaseSystem.GameData.GameData.IsWin)
            {
                unPlayable.SetActive(false);
            }
            else
            {
                playable.SetActive(false);
            }

            cam = Camera.main.gameObject;

            startPos = WinLoseSO.Entity.CameraDir.StartPosition;
            endPos = WinLoseSO.Entity.CameraDir.EndPosition;
            startRot = Quaternion.Euler(WinLoseSO.Entity.CameraDir.StartRotation);
            endRot = Quaternion.Euler(WinLoseSO.Entity.CameraDir.EndRotation);

            cam.transform.position = startPos;
            cam.transform.rotation = startRot;

            StartCoroutine(CameraDirection());
        }

        IEnumerator CameraDirection()
        {
            float d0 = WinLoseSO.Entity.CameraDir.Duration;
            float t0 = 0;

            while (t0 < d0)
            {
                float _t0 = t0 / d0;

                cam.transform.position = Vector3.Slerp(startPos, endPos, _t0);
                cam.transform.rotation = Quaternion.Slerp(startRot, endRot, _t0);

                t0 += Time.deltaTime;

                yield return null;
            }

            cam.transform.position = endPos;
            cam.transform.rotation = endRot;

            RectTransform resultUI = ((BaseSystem.GameData.GameData.IsWin) ? victoryUI : defeatUI).GetComponent<RectTransform>();
            float d1 = WinLoseSO.Entity.ResultUIDur;
            float t1 = 0;

            while (t1 < d1)
            {
                //resultUI.

                yield return null;
            }
        }
    }
}