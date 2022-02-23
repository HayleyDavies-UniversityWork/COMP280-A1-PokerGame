using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Poker.Game.Utils
{
    public enum Hands
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeKind,
        Straight,
        Flush,
        FullHouse,
        FourKind,
        StraightFlush,
        RoyalFlush
    }

    public struct HandValue
    {
        public Hands Hand;
        public int Total;

        public int HighCardTotal;
    }
    public static class HandEvaluation
    {
        public static HandValue EvaluateHand(Card[] tableCards, Card[] handCards)
        {
            HandValue handValue = new HandValue();
            HandValue tempHandValue = new HandValue();
            List<Card> cards = new List<Card>(handCards).Concat(tableCards).ToList();
            cards = SortCardsByValue(cards);

            List<Card> straightCards;

            tempHandValue = FindRelated(cards);

            if (HasStraight(cards, out straightCards, out handValue))
            {
                if (HasFlush(straightCards, out handValue))
                {
                    handValue.Hand = Hands.StraightFlush;
                    if (straightCards[0].Value == (int)Card.NumberEnum.Ten && straightCards.Last().Value == (int)Card.NumberEnum.Ace)
                    {
                        handValue.Hand = Hands.RoyalFlush;
                    }
                }
            }

            if (handValue.Hand < tempHandValue.Hand)
            {
                handValue = tempHandValue;
            }

            if (HasFlush(cards, out tempHandValue))
            {
                if (tempHandValue.Hand > handValue.Hand)
                {
                    handValue = tempHandValue;
                }
            }

            return handValue;
        }

        static List<Card> SortCardsByValue(List<Card> cards)
        {

            List<Card> sortedCards = new List<Card>();

            int totalValidCards = cards.Count;
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i] == null)
                {
                    totalValidCards = i;
                    break;
                }
            }

            cards.RemoveRange(totalValidCards, cards.Count - totalValidCards);

            foreach (Card c in cards.OrderBy(c => c.Value))
            {
                sortedCards.Add(c);
            }

            return sortedCards;
        }

        static Dictionary<int, int> CountCards(List<Card> cards)
        {
            cards.Reverse();
            Dictionary<int, int> cardCounts = new Dictionary<int, int>();
            foreach (Card c in cards)
            {
                if (cardCounts.ContainsKey(c.Value))
                {
                    cardCounts[c.Value] = cardCounts[c.Value] + 1;
                }
                else
                {
                    cardCounts.Add(c.Value, 1);
                }
            }

            return cardCounts;
        }

        static HandValue FindRelated(List<Card> cards)
        {
            Dictionary<int, int> cardCounts = CountCards(cards);

            List<int> cardValues = new List<int>();
            foreach (Card c in cards)
            {
                cardValues.Insert(0, c.Value);
            }

            List<int> highCardValues = cardValues;

            HandValue handValue = new HandValue();

            int highestCount = 0;
            int handStrength = 0;

            bool hasFullHouse = false;
            bool hasTwoPair = false;

            foreach (KeyValuePair<int, int> card in cardCounts)
            {
                if (((highestCount == 2 && card.Value == 3)
              || (highestCount == 3 && card.Value == 2)) && !hasFullHouse)
                {
                    handStrength += card.Key * card.Value;
                    hasFullHouse = true;
                    break;
                }
                else if (card.Value > highestCount)
                {
                    highestCount = card.Value;
                    handStrength = card.Key * card.Value;
                    highCardValues = cardValues;
                    if (highCardValues.Count >= 5)
                    {
                        for (int j = 0; j < card.Value; j++)
                        {
                            highCardValues.Remove(card.Key);
                        }
                    }
                }
                else if (highestCount == 2 && card.Value == 2 && !hasTwoPair)
                {
                    handStrength += card.Key * card.Value;
                    hasTwoPair = true;
                    for (int j = 0; j < card.Value; j++)
                    {
                        highCardValues.Remove(card.Key);
                    }
                }
            }

            int highCardCount = 5 - highestCount;

            if (highCardCount > cards.Count)
            {
                highCardCount = cards.Count;
            }

            if (highestCount == 4)
            {
                handValue.Hand = Hands.FourKind;
            }
            else if (hasFullHouse)
            {
                handValue.Hand = Hands.FullHouse;
                highCardCount = 0;
            }
            else if (highestCount == 3)
            {
                handValue.Hand = Hands.ThreeKind;
            }
            else if (hasTwoPair)
            {
                handValue.Hand = Hands.TwoPair;
                highCardCount = 1;
            }
            else if (highestCount == 2)
            {
                handValue.Hand = Hands.OnePair;
            }
            else
            {
                handValue.Hand = Hands.HighCard;
            }

            for (int i = 0; i < highCardCount; i++)
            {
                handValue.HighCardTotal += highCardValues[i];
            }

            handValue.Total = handStrength;

            return handValue;
        }

        static bool HasFlush(List<Card> cards, out HandValue handValue)
        {
            handValue = new HandValue();
            List<int> suitCounts = new List<int> { 0, 0, 0, 0 };
            Dictionary<int, List<int>> suitsOfCards = new Dictionary<int, List<int>>();
            foreach (Card c in cards)
            {
                int suitEnumAsInt = (int)c.SuitValue;
                suitCounts[suitEnumAsInt]++;
                if (!suitsOfCards.ContainsKey(suitEnumAsInt))
                {
                    suitsOfCards.Add(suitEnumAsInt, new List<int>());
                }
                suitsOfCards[suitEnumAsInt].Add(c.Value);
            }

            for (int i = 0; i < suitCounts.Count; i++)
            {
                if (suitCounts[i] >= 5)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        handValue.Total += suitsOfCards[i][j];
                    }
                    handValue.Hand = Hands.Flush;
                    return true;
                }
            }

            return false;
        }

        static bool HasStraight(List<Card> cards, out List<Card> straightCards, out HandValue handValue)
        {
            handValue = new HandValue();
            straightCards = new List<Card>();
            List<int> uniqueCardValues = new List<int>();
            int handStrength = 0;


            int lastValue = cards[0].Value;
            if (cards[0].Value == (int)Card.NumberEnum.Two &&
            cards.Last().Value == (int)Card.NumberEnum.Ace)
            {
                straightCards.Add(cards.Last());
                uniqueCardValues.Add(cards.Last().Value);
            }

            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];
                if (card.Value == lastValue || card.Value == lastValue + 1)
                {
                    straightCards.Add(card);
                    lastValue = card.Value;
                    if (card.Value == lastValue + 1)
                    {
                        uniqueCardValues.Add(card.Value);
                    }
                }
                else if (uniqueCardValues.Count < 5 && cards.Count() - i >= 5)
                {
                    straightCards = new List<Card>();
                    uniqueCardValues = new List<int>();
                    straightCards.Add(card);
                }
                else
                {
                    break;
                }
            }

            if (straightCards.Count >= 5)
            {
                handStrength = uniqueCardValues.Sum();
                if (straightCards[0].Value == (int)Card.NumberEnum.Ace)
                {
                    handStrength -= ((int)Card.NumberEnum.Ace - 1);
                }
                handValue.Total = handStrength;
                handValue.Hand = Hands.Straight;
                return true;
            }

            return false;
        }

    }
}