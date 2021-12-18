using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.Game
{
    public class GameSettings
    {
        public int buyIn;
        public int smallBlind;
        public int bigBlind;
        public GameSettings(int _buyIn, int _minBet)
        {
            buyIn = _buyIn;
            bigBlind = _minBet;
            smallBlind = Mathf.RoundToInt(bigBlind / 2);
        }
    }
}
