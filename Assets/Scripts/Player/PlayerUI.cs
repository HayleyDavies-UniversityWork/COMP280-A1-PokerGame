using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Poker.Game.Players
{
    public class PlayerUI : MonoBehaviour
    {
        private Canvas display;
        public PlayerActions playerActions;
        public Button checkButton;
        public Button betButton;
        public Button[] buttonUI;

        void Start()
        {
            display = GetComponent<Canvas>();

            DisableCanvas();

            buttonUI = GetComponentsInChildren<Button>();
        }

        public void EnableUI()
        {
            if (playerActions.playerType != PlayerType.Player) return;

            display.enabled = true;

            int currentBet = playerActions.gameController.currentBet;
            int spendThisRound = playerActions.spendThisRound;
            if (currentBet == 0 || currentBet == spendThisRound)
            {
                checkButton.GetComponentInChildren<TextMeshProUGUI>().text = "Check";
            }
            else
            {
                Debug.Log($"{currentBet} | {spendThisRound}");
                checkButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Call ${currentBet - spendThisRound}";
            }
            EnableButton(checkButton);
            EnableButton(betButton);
            foreach (Button b in buttonUI)
            {
                EnableButton(b);
            }
        }

        public void DisableUI()
        {
            if (playerActions.playerType != PlayerType.Player) return;

            if (playerActions.isOut)
            {
                foreach (Button b in buttonUI)
                {
                    DisableButton(b);
                }
            }
        }

        public void DisableCanvas()
        {
            display.enabled = false;
        }

        void DisableButton(Button button)
        {
            button.interactable = false;
        }

        void EnableButton(Button button)
        {
            button.interactable = true;
        }

        public void Call()
        {
            playerActions.Call();
        }

        public void Bet()
        {
            playerActions.Bet();
        }

        public void CallAny()
        {
            playerActions.CallAny();
        }

        public void CheckFold()
        {
            playerActions.CheckFold();
        }

        public void Fold()
        {
            playerActions.Fold();
        }
    }
}
