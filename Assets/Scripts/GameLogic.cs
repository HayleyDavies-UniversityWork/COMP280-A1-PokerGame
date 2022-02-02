using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;

namespace Poker.Game
{
    public class GameLogic : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            NetworkManager.Instance.InstantiateGameplayController();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}