using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.Game
{
    using Players;
    public class Table
    {
        public List<Player> playerList;
        public Card[] cards;
        public Deck deck;

        public List<int> pots;
        public Dictionary<Player, int> totalInPots;
        public int currentPot;

        public Table(Player host)
        {
            deck = new Deck();

            pots = new List<int>() {
                0
            };

            currentPot = 0;

            playerList = new List<Player>();
            this.AddPlayer(host);
        }
    }
}