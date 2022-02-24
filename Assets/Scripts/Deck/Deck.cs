using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace Poker.Game
{
    using Utils;
    public class Deck
    {
        // a queue for the shuffled cards
        public Queue<Card> shuffledCards;
        // a list of all the possible card combinations
        List<Card> allCards;

        System.Random prng;

        public string deckValue;

        private char separator = '-';


        /// <summary>
        /// a collection which stores cards
        /// </summary>
        public Deck(int seed)
        {
            // get all the cards
            allCards = GetAllCards();

            prng = new System.Random(seed);
            // and shuffle them
            shuffledCards = Shuffle(this);
            deckValue = "";

            int i = 0;
            foreach (Card c in shuffledCards)
            {
                int index = allCards.IndexOf(c);
                if (i != 0)
                {
                    deckValue += $"{separator}";
                }
                deckValue += $"{index}";
                i++;
            }

            Debug.LogError(deckValue);
        }

        public Deck(string deckString)
        {
            allCards = GetAllCards();

            shuffledCards = SyncDeck(deckString);
        }

        public Queue<Card> SyncDeck(string deckString)
        {
            Queue<Card> deck = new Queue<Card>();
            string[] cardOrder = deckString.Split(separator);
            foreach (string s in cardOrder)
            {
                int cardIndex = Int32.Parse(s);
                deck.Enqueue(allCards[cardIndex]);
            }

            return deck;
        }

        /// <summary>
        /// shuffle all of the cards
        /// </summary>
        /// <param name="deck">the deck to shuffle</param>
        /// <returns>a queue of the shuffled deck</returns>
        public Queue<Card> Shuffle(Deck deck)
        {
            //  start profiling this script
            Profiler.BeginSample("Deck.Shuffle");

            // start a new stopwatch
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            // define and create a new shuffle
            Queue<Card> shuffle = new Queue<Card>();

            // create a new shuffle
            shuffle = new Queue<Card>();

            // create a new deck of all cards
            List<Card> cardsInDeck = new List<Card>(deck.allCards);

            // get the length of the deck
            int cardsLength = deck.allCards.Count;

            // for all the values in the deck
            for (int i = 0; i < cardsLength; i++)
            {
                // get a random index of the values remaining
                int index = prng.Next(0, cardsLength - i);

                // get the card at that index
                Card card = cardsInDeck[index];

                // if the card doesn't already exist
                if (!shuffle.Contains(card))
                {
                    // add that card to the deck
                    shuffle.Enqueue(card);
                }
                // otherwise log that it failed
                else
                {
                    Debug.Log("ERROR");
                }

                // remove the card from the original deck
                cardsInDeck.RemoveAt(index);
            }

            // stop the stopwatch
            watch.Stop();
            // log how much time this took by the watch
            Debug.LogError($"Shuffle {watch.ElapsedMilliseconds}");

            // end the profiler sample
            Profiler.EndSample();

            // return the shuffled deck
            return shuffle;
        }

        /// <summary>
        /// shuffle all of the cards
        /// </summary>
        /// <param name="deck">the deck to shuffle</param>
        /// <returns>a queue of the shuffled deck</returns>
        public Queue<Card> ShuffleDeck(Deck deck)
        {
            //  start profiling this script
            Profiler.BeginSample("Deck.ShuffleDeck");

            // start a new stopwatch
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            // define and create a new shuffle
            Queue<Card> shuffle = new Queue<Card>();

            // create a new shuffle
            shuffle = new Queue<Card>();

            // create a new deck of all cards
            List<Card> cardsInDeck = new List<Card>(deck.allCards);

            // get the length of the deck
            int cardsLength = deck.allCards.Count;

            // while the shuffled deck's count is less than a normal deck
            while (shuffle.Count < cardsLength)
            {
                // get a random index of the values remaining
                int index = prng.Next(0, cardsLength);

                // get the card at that index
                Card card = cardsInDeck[index];

                // if the card doesn't already exist
                if (!shuffle.Contains(card))
                {
                    // add that card to the deck
                    shuffle.Enqueue(card);
                }
                // otherwise log that it failed
                else
                {
                    Debug.Log("ERROR");
                }
            }

            // stop the stopwatch
            watch.Stop();
            // log how much time this took by the watch
            Debug.LogError($"Shuffle {watch.ElapsedMilliseconds}");

            // end the profiler sample
            Profiler.EndSample();

            // return the shuffled deck
            return shuffle;
        }

        /// <summary>
        /// generate the list of all cards
        /// </summary>
        /// <returns>all the potential cards</returns>
        List<Card> GetAllCards()
        {
            // create a new list of cards
            List<Card> cardsInDeck = new List<Card>();
            // for all suit and number combinations
            foreach (string i in Card.numbers.Keys)
            {
                foreach (string j in Card.suits.Keys)
                {
                    // create a new card and add it to the deck
                    Card card = new Card(i, j);
                    cardsInDeck.Add(card);
                }
            }

            return cardsInDeck;
        }
    }

    public static class DeckExtensions
    {
        /// <summary>
        /// output the values on the deck
        /// </summary>
        /// <param name="deck">the deck to output</param>
        /// <returns>the string for the cards in the deck</returns>
        public static void Output(this Deck deck)
        {
            string output = "Deck:";

            foreach (Card i in deck.shuffledCards)
            {
                output += $"\n{i.Text}";
            }

            Debugger.Log(output);
        }

        public static Card DealCard(this Deck deck)
        {
            Card card = deck.shuffledCards.Dequeue();
            return card;
        }
    }

}