using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.Game
{
    public class Card
    {
        // an array to store the card numbers
        public static string[] numbers = {
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
            "K",
            "A"
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

        public string Text;
        public string Number;
        public string Suit;
        public int Value;
        public Card(string number, string suit)
        {
            Number = number;
            Suit = suit;
            Text = number + suit;
            Value = number.IndexOf(number);
        }
    }

}