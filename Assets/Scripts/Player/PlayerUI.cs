using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Poker.Game.Players
{
    using Utils;
    public class PlayerUI : MonoBehaviour
    {
        private Canvas display;
        public PlayerActions playerActions;
        public Button betButton;
        public Button checkButton;
        public Button[] buttonUI;

        void Start()
        {
            display = GetComponent<Canvas>();

            buttonUI = GetComponentsInChildren<Button>();
        }

        void Update()
        {
            if (playerActions?.playerType != PlayerType.Player)
            {
                display.enabled = false;
            }

            if (playerActions != null)
            {
                betButton.interactable = playerActions.isTurn;
                checkButton.interactable = playerActions.isTurn;
            }
        }

        public void EnableUI()
        {
            if (playerActions.playerType != PlayerType.Player) return;

            int currentBet = playerActions.gameController.currentBet;
            int spendThisRound = playerActions.spendThisRound;
            if (currentBet == 0 || currentBet == spendThisRound)
            {
                checkButton.GetComponentInChildren<TextMeshProUGUI>().text = "Check";
            }
            else
            {
                Debugger.Log($"{currentBet} | {spendThisRound}");
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

        void DisableButton(Button button)
        {
            button.interactable = false;
        }

        void EnableButton(Button button)
        {
            button.interactable = true;
        }

        public void SetButtonOpacity(string buttonName, float opacity)
        {
            Button button = null;

            foreach (Button b in buttonUI)
            {
                if (b.name == buttonName)
                {
                    button = b;
                    break;
                }
            }

            if (button == null)
            {
                return;
            }

            Color color = button.image.color;

            color.a = opacity;

            button.image.color = color;
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
