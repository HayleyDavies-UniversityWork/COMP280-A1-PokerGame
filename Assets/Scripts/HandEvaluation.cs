using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            if ((handValue = FourKind(cards)).Hand != Hands.FourKind)
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

        static HandValue FourKind(List<Card> cards)
        {
            HandValue handValue = new HandValue();
            List<int> blacklist = new List<int>();

            for (int i = 0; i < cards.Count; i++)
            {
                int cardCount = 0;
                if (!blacklist.Contains(i))
                {
                    for (int j = i; j < cards.Count; j++)
                    {
                        if (cards[i].Value == cards[j].Value)
                        {
                            cardCount++;
                            blacklist.Add(j);
                        }
                    }
                    if (cardCount == 4)
                    {
                        handValue.Hand = Hands.FourKind;
                        handValue.Total = cards[i].Value * 4;
                        return handValue;
                    }
                }
            }
            return handValue;
        }
    }
}