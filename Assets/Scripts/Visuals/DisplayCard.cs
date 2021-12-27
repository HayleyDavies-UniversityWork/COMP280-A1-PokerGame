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
        public float rotationSpeed = 1f;
        Card card;

        bool displayFront = false;
        public bool showToPlayer = false;
        private Quaternion startRotation;

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            this.GetComponent<Canvas>().enabled = false;
            startRotation = transform.rotation;
        }

        IEnumerator FlipCard(float finalRot)
        {
            Vector3 rotation = transform.localRotation.eulerAngles;
            rotation.y += rotationSpeed;
            transform.localRotation = Quaternion.Euler(rotation);


            if (rotation.y > 90 && rotation.y < 270)
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

            yield return new WaitForEndOfFrame();

            if (rotation.y < finalRot)
            {
                StartCoroutine(FlipCard(finalRot));
            }
        }

        public void SetCard(Card _card)
        {
            card = _card;
            cardDetails.text = _card.Text;
            cardDetails.color = Card.suitColor[_card.Suit];
            this.GetComponent<Canvas>().enabled = true;
            transform.rotation = startRotation;
            if (showToPlayer)
            {
                StartCoroutine(FlipCard(180));
            }
        }

        public void Setup()
        {
            transform.rotation = startRotation;
            StartCoroutine(FlipCard(0));
            this.GetComponent<Canvas>().enabled = false;
        }
    }

}