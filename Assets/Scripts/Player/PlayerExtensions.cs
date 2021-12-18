using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.Game
{
    using Utils;
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
                    Debugger.Log($"Added {card.cardValue}. Player: {player.number}. Hand Position: {i}.");
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
            Debugger.Log($"Player {player.number} added {player.money} to pot. New pot? {newPot}");
        }
    }
}