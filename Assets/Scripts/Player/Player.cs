using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.Game
{
    public class Player
    {
        public Card[] hand;
        public int number;
        public int money;
        public Table table;

        public Player(int _number, int _money)
        {
            number = _number;
            money = _money;
            hand = new Card[2];
        }
    }
}