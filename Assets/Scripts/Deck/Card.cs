using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.Game
{
    public class Card
    {
        // an array to store the card numbers
        public static string[] numbers = {
        "A",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
        "10",
        "J",
        "Q",
        "K"
    };

        // an array to store the card suits
        public static string[] suits = {
        "♠",
        "♣",
        "♥",
        "♦"
    };

        public string cardValue;
        public Card(string _cardValue)
        {
            cardValue = _cardValue;
        }
    }

}