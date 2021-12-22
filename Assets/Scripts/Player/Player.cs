using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.Game.Players
{
    using Display;
    public class Player
    {
        public Card[] hand;
        public int number;
        public int money;
        public Table table;
        public bool isTurn = false;

        GameplayController controller;
        public DisplayPlayer display;
        public PlayerActions actions;

        public Player(int _number, int _money, GameplayController _controller)
        {
            number = _number;
            money = _money;
            hand = new Card[2];

            controller = _controller;
            display = controller.playerDisplays[number];
            actions = controller.allPlayers[number];

            display.gameObject.SetActive(true);
        }
    }
}