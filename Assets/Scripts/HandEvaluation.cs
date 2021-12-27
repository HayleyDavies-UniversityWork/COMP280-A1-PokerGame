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
        FourKind
    }

    public struct HandValue
    {
        public Hands Hand;
        public int Total;
    }
    public static class HandEvaluation
    {
        public static HandValue EvaluateHand(Card[] tableCards, Card[] handCards)
        {
            HandValue handValue = new HandValue();
            List<Card> cards = new List<Card>(tableCards);
            cards.AddRange(handCards);
            cards = SortCardsByValue(cards);
            Dictionary<int, int> cardCounts = CountCards(cards);

            if ((handValue = FindRelated(cardCounts)).Hand != Hands.FourKind)
            {

            }

            return handValue;
        }

        static List<Card> SortCardsByValue(List<Card> cards)
        {
            List<Card> sortedCards = new List<Card>();

            foreach (Card c in cards)
            {
                if (sortedCards.Count == 0)
                {
                    sortedCards.Add(c);
                }
                else
                {
                    for (int i = 0; i < sortedCards.Count; i++)
                    {
                        if (c.Value < sortedCards[i].Value)
                        {
                            sortedCards.Insert(i, c);
                            break;
                        }
                        else if (i == sortedCards.Count)
                        {
                            sortedCards.Add(c);
                        }
                    }
                }
            }

            return sortedCards;
        }

        static Dictionary<int, int> CountCards(List<Card> cards)
        {
            Dictionary<int, int> cardCounts = new Dictionary<int, int>();
            foreach (Card c in cards)
            {
                if (cardCounts.ContainsKey(c.Value))
                {
                    cardCounts[c.Value]++;
                }
                else
                {
                    cardCounts.Add(c.Value, 1);
                }
            }

            return cardCounts;
        }

        static HandValue FindRelated(Dictionary<int, int> cardCounts)
        {
            HandValue handValue = new HandValue();
            int highestCount = 0;
            int handStrength = 0;
            bool hasFullHouse = false;
            bool hasTwoPair = false;

            foreach (KeyValuePair<int, int> i in cardCounts.Reverse())
            {
                if ((highestCount == 2 && i.Value == 3)
              || (highestCount == 3 && i.Value == 2) && !hasFullHouse)
                {

                    handStrength += i.Key * i.Value;
                    hasFullHouse = true;
                    break;
                }
                else if (i.Value > highestCount)
                {
                    highestCount = i.Value;
                    handStrength = i.Key * i.Value;
                }
                else if (highestCount == 2 && i.Value == 2 && !hasTwoPair)
                {
                    handStrength += i.Key * i.Value;
                    hasTwoPair = true;
                }
            }

            handValue.Total = handStrength;

            if (highestCount == 4)
            {
                handValue.Hand = Hands.FourKind;
            }
            else if (hasFullHouse)
            {
                handValue.Hand = Hands.FullHouse;
            }
            else if (highestCount == 3)
            {
                handValue.Hand = Hands.ThreeKind;
            }
            else if (hasTwoPair)
            {
                handValue.Hand = Hands.TwoPair;
            }
            else if (highestCount == 2)
            {
                handValue.Hand = Hands.OnePair;
            }

            return handValue;
        }
    }
}