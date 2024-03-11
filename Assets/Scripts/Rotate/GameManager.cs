using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SelectTeam
{
    public class GameManager : MonoBehaviour
    {
        #region static‚©‚ÂƒVƒ“ƒOƒ‹ƒgƒ“‚É‚·‚é
        public static GameManager Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion
        public bool isUsingMouse = true;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}

