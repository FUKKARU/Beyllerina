using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseSystem
{
    namespace GameData
    {
        public static class GameData
        {
            static byte roundNum = 1;
            public static byte RoundNum
            {
                get
                {
                    return roundNum;
                }

                set
                {
                    roundNum = (byte)Mathf.Clamp(value, 1, GameSO.Entity.RoundNum);
                }
            }

            public static float PlayableHp { get; set; } = 1;
        }
    }
}