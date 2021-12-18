using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Poker.Game
{
    using Utils;
    using Display;
    public class GameplayController : MonoBehaviour
    {
        Table pokerTable;
        Player host;
        GameSettings gameSettings;
        public int dealer;
        public int currentPlayerIndex;
        public DebugMode debugMode;
        public Queue<Player> playerQueue;

        public DisplayCard testCard;

        int currentBet;

        public List<DisplayCard> tableCards;

        [SerializeField] Button startGameButton;

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
            startGameButton.interactable = false;
        }

        void Initialize()
        {
            gameSettings = new GameSettings(1000, 50);
            Debugger.SetDebugMode(debugMode);
            host = new Player(0, gameSettings.buyIn);
            pokerTable = new Table(host);
        }

        public void AddAI(int index)
        {
            pokerTable.AddPlayer(new Player(index, gameSettings.buyIn), true);
            if (pokerTable.playerList.Count >= 2)
            {
                startGameButton.interactable = true;
            }
        }

        public void StartGame(int dealerID)
        {
            currentPlayerIndex = dealerID + 1;
            DealCards();
            PlayBlinds();
            dealerID++;

            currentBet = gameSettings.bigBlind;
            PlayFlop();
            PlayTurn();
            PlayRiver();
        }

        void PlayBlinds()
        {
            pokerTable.playerList[currentPlayerIndex].TakeMoney(gameSettings.smallBlind);
            IncrementCurrentPlayer();
            pokerTable.playerList[currentPlayerIndex].TakeMoney(gameSettings.bigBlind);
            IncrementCurrentPlayer();
        }

        void IncrementCurrentPlayer()
        {
            currentPlayerIndex++;
            if (currentPlayerIndex == pokerTable.playerList.Count)
            {
                currentPlayerIndex = 0;
            }
        }

        void DealCards()
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (Player p in pokerTable.playerList)
                {
                    Card card = pokerTable.deck.DealCard();
                    p.GiveCard(card);
                    testCard.SetCard(card);
                }
            }
        }

        void PlayFlop()
        {
            for (int i = 0; i < 3; i++)
            {
                Card card = pokerTable.deck.DealCard();
                tableCards[i].SetCard(card);
            }
        }

        void PlayTurn()
        {
            Card card = pokerTable.deck.DealCard();
            tableCards[3].SetCard(card);
        }

        void PlayRiver()
        {
            Card card = pokerTable.deck.DealCard();
            tableCards[4].SetCard(card);
        }

        void PlayerTurn(int playerID)
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Debugger.debugMode != debugMode)
            {
                Debugger.SetDebugMode(debugMode);
            }
        }
    }
}
