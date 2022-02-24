using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.Game.AI
{
    using Players;
    using Utils;

    // the action the ai will take
    public struct AIAction
    {
        public PlayerOption option;
        public int money;
    }

    public class PlayerAI : MonoBehaviour
    {
        public PlayerActions playerActions;
        public HandValue handValue;
        public int bluffChance;
        public int minBluffChance = 1;
        public int maxBluffChance = 75;
        private int maxValue = 100;
        public float value;

        float betThreshold;


        // Start is called before the first frame update
        void Start()
        {
            // generate a random bluff chance and calculation thresholds
            bluffChance = Random.Range(minBluffChance, maxBluffChance);
            betThreshold = Random.Range(0.3f, 0.9f);
        }

        /// <summary>
        /// Calculate how good the AI's hand is
        /// </summary>
        /// <param name="handCards">the cards in the ai's hand</param>
        /// <param name="tableCards">the cards on the table</param>
        /// <param name="currentBid">the current bid</param>
        /// <param name="currentRound">the round we are currently on</param>
        /// <returns>an AIAction</returns>
        public AIAction CalculatePlay(Card[] handCards, Card[] tableCards, int currentBid, int currentRound)
        {
            // create a new action
            AIAction action;
            action.money = 0;

            // get how much money is availible to the ai
            int availibleMoney = playerActions.player.money;

            // how much money the ai has already put in
            int moneyInPot = playerActions.moneyInPot;

            // evaluate the hand
            handValue = HandEvaluation.EvaluateHand(tableCards, handCards);

            // work out the total value of the hand
            float totalValue = handValue.Total * ((int)handValue.Hand + 1);

            // if the current bid is 0 add the bluff chance
            if (currentBid == 0)
            {
                // add the bluff chance
                totalValue += bluffChance;
            }

            // if we aren't on the starting round, conserve money
            if (currentRound != 0)
            {
                totalValue -= ((currentBid / (availibleMoney + moneyInPot)) * 100) / currentRound;
            }

            // normalise the value (kinda)
            value = totalValue / maxValue;

            // log some information about the ai
            Debugger.Log($"AI {playerActions.player.number}'s value = {totalValue} | {value} | {betThreshold} | {bluffChance}");

            // calculate the max bit
            int maxBid = Mathf.RoundToInt(availibleMoney * value);

            // if the value is greater than the bet threshold, make a bet
            if (value >= betThreshold)
            {
                // set the action to bet
                action.option = PlayerOption.Bet;
                // normalize the value
                float normalizeValue = ((value - 0.3f) / (0.9f - 0.3f));
                // calculate bet amount
                action.money = Mathf.RoundToInt(availibleMoney * normalizeValue);
            }
            // if the current bid is less than the max bid for the ai
            else if (currentBid <= maxBid)
            {
                // call the bid
                action.option = PlayerOption.Call;
            }
            else
            {
                // fold
                action.option = PlayerOption.Fold;
            }

            // cap the money to the max availible
            if (action.money > availibleMoney)
            {
                action.money = availibleMoney;
            }

            // return the action
            return action;
        }
    }
}