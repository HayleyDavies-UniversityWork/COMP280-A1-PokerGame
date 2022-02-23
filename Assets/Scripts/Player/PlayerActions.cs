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

        Coroutine timer;

        private void Start()
        {

        }

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

            Debug.Log($"There are {networkPlayers.Length} network players in the scene.");
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
                    Call();
                    break;
                case PlayerType.Player:
                    playerUI.EnableUI();
                    timer = StartCoroutine(PlayerTimeout(30));
                    if (isActionQueued)
                    {
                        queuedAction.Invoke();
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

        IEnumerator EndTurn(int amount)
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
            if (amount > gameController.currentBet && amount <= player.money)
            {
                option = PlayerOption.Bet;
                player.TakeMoney(amount);
                gameController.currentBet = amount;

                StartCoroutine(EndTurn(amount));
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

            StartCoroutine(EndTurn(0));
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
