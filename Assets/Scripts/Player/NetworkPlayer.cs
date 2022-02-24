using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            playerIndex = networkObject.playerIndex;
        }

        // Update is called once per frame
        void Update()
        {
            if (networkObject == null)
            {
                Debugger.Warn("I can't find my network object");
                return;
            }
        }

        /// <summary>
        /// Set the player actions.
        /// </summary>
        /// <param name="actions">The PlayerActions to set on this object.</param>
        public void SetPlayerActions(PlayerActions actions)
        {
            // set the player settings
            playerActions = actions;
            player = actions.player;

            // set this object's parent to be the playerActions' GameObject
            transform.parent = playerActions.transform;

            // apply the player's money and index to the network object
            networkObject.playerMoney = player.money;
            networkObject.playerIndex = player.number;

            // send an RPC to other connections to say that this player has joined
            networkObject.SendRpc(RPC_JOIN_TABLE, Receivers.OthersBuffered);
        }

        /// <summary>
        /// Set the player actions.
        /// </summary>
        public void SetPlayerActions()
        {
            // set the player to the correct player in the list
            player = GameplayController.singleton.pokerTable.playerList[networkObject.playerIndex];

            // if the player is unset, return
            if (player == null)
            {
                return;
            }

            // set the players money and index from the network
            player.money = networkObject.playerMoney;
            player.number = networkObject.playerIndex;

            // set the player actions and netowrk player
            playerActions = player.actions;
            playerActions.networkPlayer = this;

            // set this object's parent to be the playerActions' GameObject
            transform.parent = playerActions.transform;
        }

        /// <summary>
        /// Perform a local action.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="money">The money to spend on this action.</param>
        public void LocalAction(int action, int money)
        {
            // if this isn't the owner of the network object, return
            if (!networkObject.IsOwner)
            {
                return;
            }

            // send an RPC to other connections for the action taken locally
            networkObject.SendRpc(RPC_NETWORK_ACTION, Receivers.OthersBuffered, action, money);

            // set the network object's money to be the local money
            networkObject.playerMoney = playerActions.player.money;
        }

        /// <summary>
        /// Make the player join the table correctly.
        /// </summary>
        public override void JoinTable(RpcArgs args)
        {
            // set the player actions - setting the player up on the network
            SetPlayerActions();
        }

        /// <summary>
        /// Recieve player's action from the network.
        /// </summary>
        /// <param name="args">action and money</param>
        public override void NetworkAction(RpcArgs args)
        {
            // if the player actions isn't set, set it
            if (playerActions == null)
            {
                SetPlayerActions();
            }

            // if the player isn't a network player, return
            if (playerActions.playerType != PlayerType.Network)
            {
                return;
            }

            // get the player action from the args
            PlayerOption action = (PlayerOption)args.GetNext<int>();

            // check the action
            switch (action)
            {
                // if betting, get the money spent and bet it
                case PlayerOption.Bet:
                    playerActions.Bet(args.GetNext<int>());
                    break;
                // if calling, call
                case PlayerOption.Call:
                    playerActions.Call();
                    break;
                // if folding, fold
                case PlayerOption.Fold:
                    playerActions.Fold();
                    break;
            }

            // sync the player
            SyncPlayer();
        }

        /// <summary>
        /// Update the local player's money and index from the network
        /// </summary>
        public void SyncPlayer()
        {
            playerActions.player.money = networkObject.playerMoney;
            playerActions.player.number = networkObject.playerIndex;
        }
    }
}