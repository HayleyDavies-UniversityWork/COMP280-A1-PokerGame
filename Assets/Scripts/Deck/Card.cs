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

        public static Dictionary<string, Color> suitColor = new Dictionary<string, Color> {
            {"♠", Color.black},
            {"♣", Color.black},
            {"♥", Color.red},
            {"♦", Color.red}
        };

        public string cardValue;
        public string cardNumber;
        public string cardSuit;
        public Card(string _cardNumber, string _cardSuit)
        {
            cardNumber = _cardNumber;
            cardSuit = _cardSuit;
            cardValue = _cardNumber + _cardSuit;
        }
    }

}