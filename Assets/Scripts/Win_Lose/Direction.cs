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

            const int CANVAS_WIDTH = 800;
            RectTransform resultUI = (BaseSystem.GameData.GameData.IsWin ? victoryUI : defeatUI).GetComponent<RectTransform>();
            float d1 = WinLoseSO.Entity.ResultUIDur;
            float t1 = 0;

            while (t1 < d1)
            {
                Vector3 pos = resultUI.localPosition;
                pos.x = -CANVAS_WIDTH / d1 * t1 + CANVAS_WIDTH;
                resultUI.localPosition = pos;

                yield return null;
            }

            Vector3 _pos = resultUI.localPosition;
            _pos.x = 0;
            resultUI.localPosition = _pos;
        }
    }
}