using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.Game.Players
{
    using Display;

    public enum PlayerType
    {
        None,
        AI,
        Player,
        Network
    }

    [System.Serializable]
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

            controller = _controller;
            display = controller.playerDisplays[number];
            display.player = this;
            actions = controller.allPlayers[number];

            display.gameObject.SetActive(true);
        }

        public void ResetHand()
        {
            hand = new Card[2];
        }
    }
}