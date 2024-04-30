using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseSystem
{
    namespace GameData
    {
        public static class GameData
        {
            static byte playableRoundNum = 0;
            public static byte PlayableRoundNum
            {
                get
                {
                    return playableRoundNum;
                }

                set
                {
                    playableRoundNum = (byte)Mathf.Clamp(value, 0, GameSO.Entity.RoundNum);
                }
            }

            static byte unPlayableRoundNum = 0;
            public static byte UnPlayableRoundNum
            {
                get
                {
                    return unPlayableRoundNum;
                }

                set
                {
                    unPlayableRoundNum = (byte)Mathf.Clamp(value, 0, GameSO.Entity.RoundNum);
                }
            }
        }
    }
}