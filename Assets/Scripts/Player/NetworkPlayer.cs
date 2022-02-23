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
        public bool IsOwner;
        // Start is called before the first frame update
        void Start()
        {
            playerIndex = networkObject.playerIndex;

            IsOwner = networkObject.IsOwner;
        }

        // Update is called once per frame
        void Update()
        {
            if (networkObject == null)
            {
                Debug.LogWarning("I can't find my network object");
                return;
            }
        }

        public void SetPlayerActions(PlayerActions actions)
        {
            playerActions = actions;
            player = actions.player;

            transform.parent = playerActions.transform;

            networkObject.playerMoney = player.money;
            networkObject.playerIndex = player.number;
            networkObject.SendRpc(RPC_JOIN_TABLE, Receivers.OthersBuffered);
        }

        public void SetPlayerActions()
        {
            player = GameplayController.singleton.pokerTable.playerList[networkObject.playerIndex];

            if (player == null)
            {
                return;
            }

            player.money = networkObject.playerMoney;
            player.number = networkObject.playerIndex;

            playerActions = player.actions;
            playerActions.networkPlayer = this;

            transform.parent = playerActions.transform;
        }

        public void LocalAction(int action, int money)
        {
            if (!networkObject.IsOwner)
            {
                return;
            }

            networkObject.SendRpc(RPC_SEND_PLAYER_ACTION, Receivers.OthersBuffered, action, money);
            networkObject.playerMoney = playerActions.player.money;
        }

        public override void RecieveCard(RpcArgs args)
        {
            throw new NotImplementedException();
        }

        public override void JoinTable(RpcArgs args)
        {
            SetPlayerActions();
        }

        public override void SendPlayerAction(RpcArgs args)
        {
            if (playerActions == null)
            {
                SetPlayerActions();
            }

            if (playerActions.playerType != PlayerType.Network)
            {
                return;
            }

            PlayerOption action = (PlayerOption)args.GetNext<int>();
            Debug.LogError($"Player {networkObject.playerIndex} has taken turn of {action}");
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

            SyncPlayer();
        }

        public void SyncPlayer()
        {
            playerActions.player.money = networkObject.playerMoney;
            playerActions.player.number = networkObject.playerIndex;
        }

        public override void RecieveInScene(RpcArgs args)
        {
        }
    }
}