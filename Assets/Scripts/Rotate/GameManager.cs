using System;
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

        public bool IsUsingMouse = true;
        [NonSerialized] public float time = 10f;

        void Start()
        {

        }

        void Update()
        {

        }
    }

}

