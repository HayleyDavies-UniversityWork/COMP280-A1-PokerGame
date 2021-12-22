using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Poker.Game.Display
{
    public class DisplayTable : MonoBehaviour
    {
        public Table pokerTable;

        public TextMeshProUGUI potSize;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (pokerTable != null)
            {
                potSize.text = $"Pot:\n${pokerTable.GetTotalPot()}";
            }
        }
    }

}