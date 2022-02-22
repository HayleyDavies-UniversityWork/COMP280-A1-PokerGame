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
    public enum PlayerOption
    {
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

        public Button betButton;
        public Button checkButton;
        public TMP_InputField betAmount;

        Coroutine timer;

        private void Start()
        {
            if (playerType == PlayerType.Player)
            {
                playerUI = GetComponentInChildren<PlayerUI>();
                playerUI.playerActions = this;
            }

            gameController = GameplayController.singleton;
        }

        private void CreateOnlinePlayerInstance()
        {
            if (networkPlayer != null)
            {
                return;
            }

            if (!gameController.networkObject.IsOwner)
            {
                networkPlayer = GetComponentInChildren<NetworkPlayer>();
                return;
            }

            if (!gameController.networkObject.IsServer)
            {
                return;
            }

            GameObject networkPlayerObject = NetworkManager.Instance.InstantiatePlayer().gameObject;
            networkPlayer = networkPlayerObject.GetComponent<NetworkPlayer>();
            networkPlayer.playerActions = this;
            networkPlayer.networkObject.playerIndex = player.number;
            networkPlayer.networkObject.playerMoney = player.money;
        }

        public void PlayerTurn(GameplayController controller, Player currentPlayer)
        {
            isTurn = true;
            player = currentPlayer;
            CreateOnlinePlayerInstance();

            switch (playerType)
            {
                case PlayerType.AI:
                    Call();
                    break;
                case PlayerType.Player:
                    playerUI.EnableUI();
                    timer = StartCoroutine(PlayerTimeout(30));
                    if (isActionQueued)
                    {
                        queuedAction.Invoke();
                    }
                    break;
                case PlayerType.Network:
                    break;
            }
        }

        IEnumerator EndTurn()
        {
            switch (playerType)
            {
                case PlayerType.AI:
                    break;
                case PlayerType.Player:
                    playerUI.DisableUI();
                    if (timer != null)
                    {
                        StopCoroutine(timer);
                    }
                    break;
                case PlayerType.Network:
                    break;
            }

            yield return new WaitForSeconds(0.1f);
            isTurn = false;
            gameController.EndPlayerTurn(player);
        }

        public void Call()
        {
            option = PlayerOption.Call;
            int amount = gameController.currentBet - spendThisRound;
            player.TakeMoney(amount);

            networkPlayer.LocalAction((int)option, amount);

            StartCoroutine(EndTurn());
        }

        public void Bet()
        {
            int amount = Convert.ToInt32(betAmount.text) - spendThisRound;
            Bet(amount);
        }

        public void Bet(int amount)
        {
            if (amount > gameController.currentBet && amount <= player.money)
            {
                option = PlayerOption.Bet;
                player.TakeMoney(amount);
                gameController.currentBet = amount;

                networkPlayer.LocalAction((int)option, amount);

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

            networkPlayer.LocalAction((int)option, 0);

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
