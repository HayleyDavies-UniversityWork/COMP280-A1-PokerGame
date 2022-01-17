using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Poker.Game.Players
{
    using Utils;
    public enum PlayerOption
    {
        Bet,
        Call,
        Fold
    }

    public class PlayerActions : MonoBehaviour
    {
        public Player player;

        public UnityAction queuedAction;
        public bool isActionQueued = false;
        public bool isTurn;
        public bool isPlayer;
        public bool isOut;
        public int moneyInPot;
        public int spendThisRound;

        public GameplayController gameController;

        public PlayerOption option;

        public PlayerUI playerUI;

        public Button betButton;
        public Button checkButton;
        public TMP_InputField betAmount;

        Coroutine timer;

        private void Start()
        {
            if (isPlayer)
            {
                playerUI = GetComponentInChildren<PlayerUI>();
                playerUI.playerActions = this;
            }
        }

        public void PlayerTurn(GameplayController controller, Player currentPlayer)
        {
            gameController = controller;
            isTurn = true;
            player = currentPlayer;

            if (isPlayer)
            {
                playerUI.EnableUI();
                timer = StartCoroutine(PlayerTimeout(30));
                if (isActionQueued)
                {
                    queuedAction.Invoke();
                }
            }
            else
            {
                Call();
            }
        }

        IEnumerator EndTurn()
        {
            if (isPlayer)
            {
                playerUI.DisableUI();
                if (timer != null)
                {
                    StopCoroutine(timer);
                }
            }
            yield return new WaitForSeconds(0.1f);
            isTurn = false;
            gameController.EndPlayerTurn(player);
        }

        public void Call()
        {
            option = PlayerOption.Call;
            player.TakeMoney(gameController.currentBet - spendThisRound);

            StartCoroutine(EndTurn());
        }

        public void Bet()
        {
            int amount = Convert.ToInt32(betAmount.text) - spendThisRound;
            if (amount > gameController.currentBet && amount <= player.money)
            {
                option = PlayerOption.Bet;
                player.TakeMoney(amount);
                gameController.currentBet = amount;

                StartCoroutine(EndTurn());
            }
        }

        public void CallAny()
        {
            if (!isTurn)
            {
                QueueAction(CallAny);
            }
            else
            {
                Call();
            }
        }

        public void CheckFold()
        {
            if (!isTurn)
            {
                QueueAction(CheckFold);
            }
            else
            {
                if (gameController.currentBet > 0)
                {
                    Fold();
                }
                else
                {
                    Call();
                }
            }
        }

        public void Fold()
        {
            option = PlayerOption.Fold;

            if (!isTurn)
            {
                QueueAction(Fold);
            }
            else
            {
                isOut = true;
            }

            StartCoroutine(EndTurn());
        }

        void QueueAction(UnityAction actionToQueue)
        {
            if (actionToQueue == queuedAction)
            {
                queuedAction = null;
                isActionQueued = false;
            }
            else
            {
                queuedAction = actionToQueue;
                isActionQueued = true;
            }
        }

        IEnumerator PlayerTimeout(int timeoutLength)
        {
            yield return new WaitForSeconds(1);
            if (timeoutLength > 0)
            {
                timer = StartCoroutine(PlayerTimeout(timeoutLength - 1));
            }
            else
            {
                Fold();
            }
        }
    }
}
