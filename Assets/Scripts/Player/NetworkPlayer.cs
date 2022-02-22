using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using BeardedManStudios.Forge.Networking.Generated;

namespace Poker.Game.Players
{
    using BeardedManStudios.Forge.Networking;
    using Utils;
    public class NetworkPlayer : PlayerBehavior
    {
        public Player player;
        public PlayerActions playerActions;
        public int playerIndex;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (networkObject == null)
            {
                Debug.LogWarning("I can't find my network object");
                return;
            }
            playerIndex = networkObject.playerIndex;

            if (transform.parent == null)
            {
                player = GameplayController.singleton.pokerTable.playerList[networkObject.playerIndex];

                if (player == null)
                {
                    return;
                }

                playerActions = player.actions;
                playerActions.networkPlayer = this;

                transform.parent = playerActions.transform;
            }
        }

        public void SetPlayerActions(PlayerActions actions)
        {
            playerActions = actions;
            player = actions.player;

            transform.parent = playerActions.transform;

            if (networkObject.IsOwner)
            {
                networkObject.playerMoney = player.money;
                networkObject.playerIndex = player.number;
                networkObject.SendRpc(RPC_RECIEVE_IN_SCENE, Receivers.OthersBuffered);
            }
        }

        public void LocalAction(int action, int money)
        {
            networkObject.SendRpc(RPC_SEND_PLAYER_ACTION, Receivers.OthersBuffered, action, money);
        }

        public override void RecieveCard(RpcArgs args)
        {
            throw new NotImplementedException();
        }

        public override void JoinTable(RpcArgs args)
        {
            throw new NotImplementedException();
        }

        public override void SendPlayerAction(RpcArgs args)
        {
            // Run code for when the player recieves an action
            if (!networkObject.IsOwner)
            {
                PlayerOption action = (PlayerOption)args.GetNext<int>();
                switch (action)
                {
                    case PlayerOption.Bet:
                        playerActions.Bet(args.GetNext<int>());
                        break;
                    case PlayerOption.Call:
                        playerActions.Call();
                        break;
                    case PlayerOption.Fold:
                        playerActions.Fold();
                        break;
                }
                playerActions.player.money = networkObject.playerMoney;
                playerActions.player.number = networkObject.playerIndex;
            }
        }

        public override void RecieveInScene(RpcArgs args)
        {
            player = GameplayController.singleton.pokerTable.playerList[networkObject.playerIndex];

            playerActions = player.actions;
            playerActions.networkPlayer = this;

            transform.parent = playerActions.transform;
        }
    }
}