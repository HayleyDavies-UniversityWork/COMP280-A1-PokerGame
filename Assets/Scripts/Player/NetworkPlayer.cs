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
            if (networkObject.IsOwner)
            {
                playerActions.isPlayer = true;
            }

            playerActions.playerNetwork = networkObject;

            if (!networkObject.IsOwner)
            {
                playerActions.player.money = networkObject.playerMoney;
                playerActions.player.money = networkObject.playerIndex;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (networkObject == null)
            {
                return;
            }
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
            throw new NotImplementedException();
        }
    }
}