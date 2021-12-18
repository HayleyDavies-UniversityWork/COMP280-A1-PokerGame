using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Poker.Game.Utils
{
    public enum DebugMode
    {
        All,
        Warn,
        Error,
        None
    }

    public static class Debugger
    {
        public static DebugMode debugMode = DebugMode.Warn;

        public static void SetDebugMode(DebugMode mode)
        {
            debugMode = mode;
        }

        public static void Log(string debugMessage)
        {
            if (debugMode == DebugMode.All)
            {
                Debug.Log(debugMessage);
            }
        }

        public static void Warn(string debugMessage)
        {
            if (debugMode <= DebugMode.Warn)
            {
                Debug.LogWarning(debugMessage);
            }
        }

        public static void Error(string debugMessage)
        {
            if (debugMode <= DebugMode.Error)
            {
                Debug.LogError(debugMessage);
            }
        }
    }
}