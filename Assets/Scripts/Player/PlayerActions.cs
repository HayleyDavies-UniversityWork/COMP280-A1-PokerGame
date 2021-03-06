using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

namespace Poker.Game.Players
{
    using Utils;
    using AI;
    public enum PlayerOption
    {
        None,
        Bet,
        Call,
        Fold
    }

    public class PlayerActions : MonoBehaviour
    {
        public Player player;
        public NetworkPlayer networkPlayer;

        public UnityAction queuedAction;
        public bool isActionQueued = false;
        public bool isTurn;
        public PlayerType playerType;
        public bool isOut;
        public int moneyInPot;
        public int spendThisRound;

        public GameplayController gameController;

        public PlayerOption option;

        public PlayerUI playerUI;
        public TMP_InputField betAmount;

        public PlayerAI playerAI;

        Coroutine timer;

        public void Initalize(Player playerReference, PlayerType typeOfPlayer)
        {
            gameController = GameplayController.singleton;
            player = playerReference;
            playerType = typeOfPlayer;

            playerUI = GetComponentInChildren<PlayerUI>();
            playerUI.playerActions = this;
            betAmount = playerUI.GetComponentInChildren<TMP_InputField>();

            if (playerType != PlayerType.Network)
            {
                CreateOnlinePlayerInstance();
            }

            NetworkPlayer[] networkPlayers = GameObject.FindObjectsOfType<NetworkPlayer>();

            Debugger.Log($"There are {networkPlayers.Length} network players in the scene.");

            if (playerType == PlayerType.AI)
            {
                playerAI = gameObject.AddComponent<PlayerAI>();
                playerAI.playerActions = this;
            }
        }

        private void CreateOnlinePlayerInstance()
        {
            networkPlayer = GetComponentInChildren<NetworkPlayer>();
            if (networkPlayer != null)
            {
                return;
            }

            GameObject networkPlayerObject = NetworkManager.Instance.InstantiatePlayer().gameObject;
            networkPlayer = networkPlayerObject.GetComponent<NetworkPlayer>();
            networkPlayer.SetPlayerActions(this);
        }

        public void PlayerTurn(GameplayController controller, Player currentPlayer)
        {
            isTurn = true;
            player = currentPlayer;

            switch (playerType)
            {
                case PlayerType.AI:
                    if (player.money == 0)
                    {
                        Call();
                    }
                    else
                    {
                        HandlePlayerAI();
                    }
                    break;
                case PlayerType.Player:
                    playerUI.EnableUI();
                    timer = StartCoroutine(PlayerTimeout(30));
                    if (isActionQueued)
                    {
                        queuedAction.Invoke();
                        queuedAction = null;
                        isActionQueued = false;
                    }
                    if (player.money == 0)
                    {
                        Call();
                    }
                    break;
                case PlayerType.Network:
                    break;
            }
        }

        /// <summary>
        /// Handle the player's UI
        /// </summary>
        void HandlePlayerAI()
        {
            // get the poker table and current bet
            Table pokerTable = gameController.pokerTable;
            int currentBet = gameController.currentBet - spendThisRound;

            // calculate an action
            AIAction action = playerAI.CalculatePlay(player.hand, pokerTable.cards, currentBet, (int)gameController.nextStage);

            // log what the ai has chosen to do
            Debugger.Log($"Player {player.number} has chosen to {action.option} spending {action.money}");

            // switch the action's option
            switch (action.option)
            {
                // bet
                case PlayerOption.Bet:
                    Bet(action.money);
                    break;
                // call
                case PlayerOption.Call:
                    Call();
                    break;
                // fold
                case PlayerOption.Fold:
                    Fold();
                    break;
            }
        }

        IEnumerator EndTurn(int amount)
        {
            switch (playerType)
            {
                case PlayerType.AI:
                    yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 3f));

                    networkPlayer.LocalAction((int)option, amount);
                    break;
                case PlayerType.Player:
                    playerUI.DisableUI();
                    if (timer != null)
                    {
                        StopCoroutine(timer);
                    }

                    networkPlayer.LocalAction((int)option, amount);
                    break;
                case PlayerType.Network:
                    break;
            }

            isTurn = false;
            yield return new WaitForSeconds(0.1f);
            gameController.EndPlayerTurn(player, option);
            option = PlayerOption.None;
        }

        public void Call()
        {
            if (isTurn)
            {
                option = PlayerOption.Call;
                int amount = gameController.currentBet - spendThisRound;
                player.TakeMoney(amount);

                StartCoroutine(EndTurn(amount));
            }
        }

        public void Bet()
        {
            int amount = Convert.ToInt32(betAmount.text) - spendThisRound;
            Bet(amount);
        }

        public void Bet(int amount)
        {
            if (amount < gameController.currentBet)
            {
                amount = gameController.currentBet;
            }

            if (amount > player.money)
            {
                amount = player.money;
            }

            option = PlayerOption.Bet;
            player.TakeMoney(amount);
            gameController.currentBet = amount;

            StartCoroutine(EndTurn(amount));
        }

        public void CallAny()
        {
            if (!isTurn)
            {
                QueueAction(CallAny, "Call Any");
            }
            else
            {
                playerUI.SetButtonOpacity("Call Any", 1f);
                Call();
            }
        }

        public void CheckFold()
        {
            if (!isTurn)
            {
                QueueAction(CheckFold, "Check/Fold");
            }
            else
            {
                playerUI.SetButtonOpacity("Check/Fold", 1f);
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
                QueueAction(Fold, "Fold");
            }
            else
            {
                playerUI.SetButtonOpacity("Fold", 1f);
                isOut = true;
            }

            if (playerType != PlayerType.Player)
            {
                player.display.Folded();
            }

            StartCoroutine(EndTurn(0));
        }

        void QueueAction(UnityAction actionToQueue, string buttonName)
        {
            float opacity = 1f;
            if (actionToQueue == queuedAction)
            {
                queuedAction = null;
                isActionQueued = false;
            }
            else
            {
                queuedAction = actionToQueue;
                isActionQueued = true;
                opacity = 0.5f;
            }
            playerUI.SetButtonOpacity(buttonName, opacity);
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
