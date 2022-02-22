using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

namespace Poker.Game
{
    public class GameLogic : MonoBehaviour
    {
        public GameObject[] gameControllers;
        // Start is called before the first frame update
        void Start()
        {
            if (NetworkManager.Instance.IsServer)
            {
                NetworkManager.Instance.InstantiateGameplayController();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}