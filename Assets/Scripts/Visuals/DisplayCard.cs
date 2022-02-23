using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Poker.Game.Display
{
    using Players;
    public class DisplayCard : MonoBehaviour
    {
        public GameObject cardFront;
        public GameObject cardBack;
        public TextMeshProUGUI cardDetails;
        public float rotationSpeed = 1f;
        Card card;
        public PlayerType playerType = PlayerType.None;
        bool displayFront = false;
        private Quaternion startRotation;
        public bool showToPlayer = false;

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            startRotation = transform.rotation;
            this.GetComponent<Canvas>().enabled = false;
        }

        public IEnumerator FlipCard(float finalRot)
        {
            Vector3 rotation = transform.localRotation.eulerAngles;
            rotation.y += rotationSpeed;
            transform.localRotation = Quaternion.Euler(rotation);

            if (rotation.y > 90f && rotation.y < 270f)
            {
                displayFront = true;
            }
            else
            {
                displayFront = false;
            }
            cardBack.SetActive(!displayFront);
            cardFront.SetActive(displayFront);

            yield return new WaitForEndOfFrame();

            if (rotation.y < finalRot)
            {
                StartCoroutine(FlipCard(finalRot));
            }
        }

        public void SetCard(Card _card, PlayerType playerTypeReference)
        {
            playerType = playerTypeReference;

            if (playerType == PlayerType.Player || playerType == PlayerType.None)
            {
                showToPlayer = true;
            }
            else
            {
                showToPlayer = false;
            }

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

        public void SetCard(Card _card)
        {
            SetCard(_card, PlayerType.None);
        }

        public void Setup()
        {
            transform.rotation = startRotation;
            StartCoroutine(FlipCard(0));
            this.GetComponent<Canvas>().enabled = false;
            cardBack.SetActive(true);
            cardFront.SetActive(false);
        }
    }

}