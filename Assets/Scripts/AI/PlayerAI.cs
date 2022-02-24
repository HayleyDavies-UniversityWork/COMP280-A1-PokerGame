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

        public int moneyInvested;
        public int bluffChance;
        public int minBluffChance = 1;
        public int maxBluffChance = 75;
        private int maxValue = 100;
        public float value;

        float betThreshold;
        float callThreshold;


        // Start is called before the first frame update
        void Start()
        {
            // generate a random bluff chance and calculation thresholds
            bluffChance = Random.Range(minBluffChance, maxBluffChance);
            betThreshold = Random.Range(0.5f, 0.9f);
            callThreshold = Random.Range(0.1f, 0.4f);
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

            // get how much money is availible to the ai
            int availibleMoney = playerActions.player.money;

            // how much money the ai has already put in
            int moneyInPot = playerActions.moneyInPot;

            // evaluate the hand
            handValue = HandEvaluation.EvaluateHand(tableCards, handCards);

            // work out the total value of the hand
            float totalValue = handValue.Total * ((int)handValue.Hand + 1);

            if (currentBid == 0)
            {
                // add the bluff chance
                totalValue += bluffChance;
            }

            // if we aren't on the starting round 
            if (currentRound != 0)
            {
                totalValue -= ((currentBid / (availibleMoney + moneyInPot)) * 100) / currentRound;
            }

            value = totalValue / maxValue;

            Debug.LogWarning($"AI {playerActions.player.number}'s value = {totalValue} | {value} | {betThreshold} | {callThreshold} | {bluffChance}");

            int maxBid = Mathf.RoundToInt(availibleMoney * value);

            if (value >= betThreshold)
            {
                action.option = PlayerOption.Bet;
                float normalizeValue = ((value - betThreshold) / (betThreshold - 1));
                action.money = maxBid;
            }
            else if (value >= callThreshold || currentBid <= maxBid)
            {
                action.option = PlayerOption.Call;
                action.money = 0;
            }
            else
            {
                action.option = PlayerOption.Fold;
                action.money = 0;
            }

            if (action.money > availibleMoney)
            {
                action.money = availibleMoney;
            }

            return action;
        }
    }
}