using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.Game
{
    public class Card
    {
        public enum SuitEnum
        {
            Spades,
            Clubs,
            Hearts,
            Diamonds
        }

        public enum NumberEnum
        {
            NULL,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King,
            Ace
        }

        // an array to store the card numbers
        public static Dictionary<string, NumberEnum> numbers = new Dictionary<string, NumberEnum>() {
            {"2", NumberEnum.Two},
            {"3", NumberEnum.Three},
            {"4", NumberEnum.Four},
            {"5", NumberEnum.Five},
            {"6", NumberEnum.Six},
            {"7", NumberEnum.Seven},
            {"8", NumberEnum.Eight},
            {"9", NumberEnum.Nine},
            {"10", NumberEnum.Ten},
            {"J", NumberEnum.Jack},
            {"Q", NumberEnum.Queen},
            {"K", NumberEnum.King},
            {"A", NumberEnum.Ace}
        };

        // an array to store the card suits
        public static Dictionary<string, SuitEnum> suits = new Dictionary<string, SuitEnum>{
            {"♠", SuitEnum.Spades},
            {"♣", SuitEnum.Clubs},
            {"♥", SuitEnum.Hearts},
            {"♦", SuitEnum.Diamonds}
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
        public SuitEnum SuitValue;
        public int Value;
        public Card(string number, string suit)
        {


            Number = number;
            Suit = suit;
            SuitValue = suits[suit];
            Text = number + suit;
            Value = (int)numbers[number];
        }
    }

}