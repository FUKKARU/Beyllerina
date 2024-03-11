using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SelectTeam
{
    public class ControllerRotate : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        // Start is called before the first frame update
        public float degree02 = 0;
        void Start()
        {
            if (GameManager.Instance.isUsingMouse)
            {
                this.enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            float degree = Mathf.Atan2(v, h) * Mathf.Rad2Deg;

            if (degree < 0)
            {
                degree += 360;
                degree02 += (degree * 0 + 1) * 1 / 3;
            }

            text.text = degree02.ToString();
            SceneChange.rotatenumber = degree02;
    
        }
    }
}
