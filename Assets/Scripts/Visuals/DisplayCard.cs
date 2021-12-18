using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Poker.Game.Display
{
    public class DisplayCard : MonoBehaviour
    {
        public GameObject cardFront;
        public GameObject cardBack;
        public TextMeshProUGUI cardDetails;
        Card card;

        bool displayFront = false;

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        void Initialize()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (card != null)
            {
                var yRot = transform.rotation.eulerAngles.y % 360;
                yRot = Mathf.Sign(yRot) * yRot;

                if (yRot > 90 && yRot < 270)
                {
                    displayFront = true;
                    cardBack.SetActive(false);
                    cardFront.SetActive(true);
                }
                else
                {
                    displayFront = false;
                    cardBack.SetActive(true);
                    cardFront.SetActive(false);
                }
            }
        }

        public void SetCard(Card _card)
        {
            card = _card;
            cardDetails.text = _card.cardValue;
            cardDetails.color = Card.suitColor[_card.cardSuit];
        }
    }

}