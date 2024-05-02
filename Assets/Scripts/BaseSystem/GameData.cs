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

            static bool isWin = true;
            public static bool IsWin
            {
                get
                {
                    return isWin;
                }

                set
                {
                    isWin = value;
                }
            }

            static int counterNum = 0;
            public static int CounterNum
            {
                get
                {
                    return counterNum;
                }

                set
                {
                    if (value >= 0)
                    {
                        counterNum = value;
                    }
                    else
                    {
                        counterNum = 0;
                    }
                }
            }

            static byte specialPoint = 0;
            public static byte SpecialPoint
            {
                get
                {
                    return specialPoint;
                }

                set
                {
                    specialPoint = value;
                }
            }



            static float directionMoveSpeedCoef = 1;
            public static float DirectionMoveSpeedCoef
            {
                get
                {
                    return directionMoveSpeedCoef;
                }

                set
                {
                    if (value < 0)
                    {
                        directionMoveSpeedCoef = 0;
                    }
                    else
                    {
                        directionMoveSpeedCoef = value;
                    }
                }
            }
        }
    }
}