using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.Game.AI
{
    using Players;
    using Utils;

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
        private int maxValue = 400;
        public float value;

        float betThreshold;
        float callThreshold;


        // Start is called before the first frame update
        void Start()
        {
            bluffChance = Random.Range(minBluffChance, maxBluffChance);
            betThreshold = Random.Range(0.5f, 0.9f);
            callThreshold = Random.Range(0.1f, 0.4f);
        }

        public AIAction CalculatePlay(Card[] handCards, Card[] tableCards, int currentBid, int currentRound)
        {
            AIAction action;
            int availibleMoney = playerActions.player.money;

            handValue = HandEvaluation.EvaluateHand(tableCards, handCards);

            float totalValue = handValue.Total * (int)handValue.Hand;

            totalValue += maxValue / bluffChance;

            if (currentRound != 0)
            {
                totalValue -= currentBid / currentRound;
            }

            value = totalValue / maxValue;

            if (value >= betThreshold)
            {
                action.option = PlayerOption.Bet;
                float normalizeValue = ((value - betThreshold) / (betThreshold - 1));
                action.money = Mathf.RoundToInt(availibleMoney * value);
            }
            else if (value >= callThreshold || currentBid <= 50)
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