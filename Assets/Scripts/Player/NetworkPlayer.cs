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
        public PlayerActions playerActions;
        // Start is called before the first frame update
        void Start()
        {
            GameObject player = GameObject.Find($"Player {networkObject.playerIndex + 1}");

            transform.parent = player.transform;

            playerActions = player.GetComponent<PlayerActions>();
        }

        // Update is called once per frame
        void Update()
        {
            if (networkObject == null)
            {
                return;
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
    }
}