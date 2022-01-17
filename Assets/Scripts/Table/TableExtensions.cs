using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.Game
{
    using Utils;
    using Players;
    public static class TableExtensions
    {
        /// <summary>
        /// add a card to the table
        /// </summary>
        /// <param name="table">the table to add the card to</param>
        /// <param name="card">the card to be added to the table</param>
        public static void PlayCard(this Table table, Card card)
        {
            for (int i = 0; i < table.cards.Length; i++)
            {
                if (table.cards[i] == null)
                {
                    table.cards[i] = card;
                    Debugger.Log($"Added {card.Text}. Table. Card Position: {i}.");
                }
            }
            Debugger.Warn($"Trying to overfill table.");
        }

        /// <summary>
        /// add a player to the table
        /// </summary>
        /// <param name="table">the table to add the player to</param>
        /// <param name="player">the player to be added</param>
        /// <param name="isAI">whether the player is or isn't an AI</param>
        public static void AddPlayer(this Table table, Player player, bool isAI)
        {
            table.playerList.Add(player);
            player.table = table;
            Debugger.Log($"Player {player.number} added to table.");
        }

        /// <summary>
        /// add a player to the table
        /// </summary>
        /// <param name="table">the table to add the player to</param>
        /// <param name="player">the player to be added</param>
        public static void AddPlayer(this Table table, Player player)
        {
            table.AddPlayer(player, false);
            player.table = table;
        }

        /// <summary>
        /// remove a player from the table
        /// </summary>
        /// <param name="table">the table to add the player to</param>
        /// <param name="player">the player to be removed</param>
        public static void RemovePlayer(this Table table, Player player)
        {
            table.playerList.Remove(player);
            Debugger.Log($"Player {player.number} removed from table.");
        }

        /// <summary>
        /// add money to the pots on the table
        /// </summary>
        /// <param name="table">the table the pot is on</param>
        /// <param name="amount">the amount to add</param>
        /// <param name="player">the player who added the amount</param>
        /// <param name="inNewPot">create a new pot?</param>
        public static void AddToPot(this Table table, int amount, Player player, bool inNewPot)
        {
            if (table.pots.Count == 0)
            {
                table.pots.Add(0);
            }

            if (inNewPot)
            {
                table.currentPot++;
                table.pots.Add(0);
            }

            table.pots[table.currentPot] += amount;
            Debugger.Log($"Player {player.number} added {amount} to pot {table.currentPot}");
        }


        /// <summary>
        /// add money to the pots on the table
        /// </summary>
        /// <param name="table">the table the pot is on</param>
        /// <param name="amount">the amount to add</param>
        /// <param name="player">the player who added the amount</param>
        /// <returns>a string for debugging</returns>
        public static void AddToPot(this Table table, int amount, Player player)
        {
            table.AddToPot(amount, player, false);
        }

        public static int GetTotalPot(this Table table)
        {
            int total = 0;
            foreach (int i in table.pots)
            {
                total += i;
            }
            return total;
        }

        public static List<Player> FindWinner(this Table table)
        {
            List<Player> winners = new List<Player>();
            int winningHandStrength = 0;
            int highCardStrength = 0;
            Hands winningHand = Hands.HighCard;
            Dictionary<Player, HandValue> handValues = new Dictionary<Player, HandValue>();
            foreach (Player p in table.playerList)
            {
                if (!p.actions.isOut)
                {
                    handValues.Add(p, HandEvaluation.EvaluateHand(table.cards, p.hand));

                    Debugger.Log($"Player {p.number} | Hand: {handValues[p].Hand} | Value: {handValues[p].Total} | High Card: {handValues[p].HighCardTotal}");

                    if ((int)handValues[p].Hand > (int)winningHand)
                    {
                        winners.Clear();
                        winners.Add(p);
                        winningHandStrength = handValues[p].Total;
                        highCardStrength = handValues[p].HighCardTotal;
                        winningHand = handValues[p].Hand;
                    }
                    else if (handValues[p].Hand == winningHand)
                    {
                        if (handValues[p].Total > winningHandStrength)
                        {
                            winners.Clear();
                            winners.Add(p);
                            winningHandStrength = handValues[p].Total;
                        }
                        else if (handValues[p].Total == winningHandStrength)
                        {
                            if (handValues[p].HighCardTotal > highCardStrength)
                            {
                                winners.Clear();
                                winners.Add(p);
                                highCardStrength = handValues[p].HighCardTotal;
                            }
                            else if (handValues[p].HighCardTotal == highCardStrength)
                            {
                                winners.Add(p);
                            }
                        }
                    }
                }
            }

            return winners;
        }

        public static void Setup(this Table table)
        {
            table.cards = new Card[5];
            table.deck = new Deck();

            table.pots = new List<int>() {
                0
            };

            table.currentPot = 0;
        }
    }
}