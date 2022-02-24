using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Poker.Game.Display
{
    using Players;
    public class DisplayPlayer : MonoBehaviour
    {
        public List<DisplayCard> handCards;
        public TextMeshProUGUI bank;
        public Player player;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (player != null)
            {
                bank.text = $"${player.money}";
            }
        }

        public void Folded()
        {
            foreach (DisplayCard dc in handCards)
            {
                dc.ChangeCardColor(Color.gray);
            }
        }
    }
}