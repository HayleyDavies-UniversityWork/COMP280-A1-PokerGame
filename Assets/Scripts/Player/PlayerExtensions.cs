using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.Game.Players
{
    using Utils;
    using Display;
    public static class PlayerExtensions
    {
        /// <summary>
        /// put a card into the player's hand
        /// </summary>
        /// <param name="player">the player to add the card to</param>
        /// <param name="card">the card to add</param>
        public static void GiveCard(this Player player, Card card)
        {
            for (int i = 0; i < player.hand.Length; i++)
            {
                if (player.hand[i] == null)
                {
                    player.hand[i] = card;
                    Debugger.Log($"Added {card.Text}. Player: {player.number}. Hand Position: {i}.");
                    return;
                }
            }
            Debugger.Warn($"Trying to overfill hand. Player: {player.number}");
        }

        /// <summary>
        /// take money from the player
        /// </summary>
        /// <param name="player">the player to take the money from</param>
        /// <param name="amount">the amount of money to take</param>
        public static void TakeMoney(this Player player, int amount)
        {
            bool newPot = false;
            if (amount >= player.money)
            {
                amount = player.money;
                newPot = true;
            }
            player.table.AddToPot(amount, player, newPot);
            player.money -= amount;
            player.actions.spendThisRound += amount;
            Debugger.Log($"Player {player.number} added {amount} to pot. New pot? {newPot}");
        }

        public static void WinMoney(this Player player)
        {
            int winAmount = 0;
            foreach (int i in player.table.pots)
            {
                player.money += i;
                winAmount += i;
            }

            player.table.pots = new List<int>();
            Debugger.Log($"Player {player.number} won {winAmount}");
        }

        public static void Setup(this Player player)
        {
            player.hand = new Card[2];
            player.actions.isOut = false;
            foreach (DisplayCard dc in player.display.handCards)
            {
                dc.Setup();
            }
        }
    }
}