using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthTermPresentation
{
    /// <summary>
    /// プレイヤー名を保存する
    /// </summary>
    public static class PlayerNameData
    {
        public static string PlayerName { get; private set; }

        public static void SetPlayerName(string name)
        {
            PlayerName = name;
        }
    }
}