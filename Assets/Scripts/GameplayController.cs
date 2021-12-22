using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Poker.Game
{
    using Utils;
    using Display;
    using Players;

    enum PokerStage
    {
        PreFlop,
        Flop,
        Turn,
        River,
        Reset
    }

    public class GameplayController : MonoBehaviour
    {
        [Header("Table Settings")]
        public int currentBet;
        public Table pokerTable;
        [SerializeField] DisplayTable tableDisplay;
        [SerializeField] Button startGameButton;
        [SerializeField] Canvas tableCardsCanvas;
        GameSettings gameSettings;
        public List<DisplayCard> tableCards;


        [Header("Players")]
        public List<PlayerActions> allPlayers;
        public List<DisplayPlayer> playerDisplays;
        public Player finalPlayer;
        public int currentPlayerIndex;
        int dealerID = 0;
        // players to track directly internally
        Player host, lastPlayer;

        public Queue<Player> playerQueue;
        [Header("Other Table Options")]
        public DebugMode debugMode;
        PokerStage nextStage = PokerStage.PreFlop;

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
            host = new Player(0, gameSettings.buyIn, this);
            pokerTable = new Table(host);
            tableDisplay.pokerTable = pokerTable;
        }

        public void AddAI(int index)
        {
            pokerTable.AddPlayer(new Player(index, gameSettings.buyIn, this), true);
            if (pokerTable.playerList.Count >= 2)
            {
                startGameButton.interactable = true;
            }
        }

        public void StartGame()
        {
            nextStage = PokerStage.PreFlop;
            StartNextPhase();
        }

        public void NextPlayer()
        {
            IncrementCurrentPlayer();
            Player currentPlayer = pokerTable.playerList[currentPlayerIndex];
            Debugger.Log($"Player {currentPlayerIndex} turn");
            currentPlayer.actions.PlayerTurn(this, currentPlayer);
        }

        public void EndPlayerTurn(Player currentPlayer)
        {
            PlayerActions player = currentPlayer.actions;
            switch (player.option)
            {
                case PlayerOption.Bet:
                    lastPlayer = GetFinalPlayer();
                    break;
                case PlayerOption.Call:
                    break;
                case PlayerOption.Fold:
                    RemovePlayer(currentPlayer);
                    break;
            }
            if (currentPlayer == lastPlayer)
            {
                StartNextPhase();
            }
            else
            {
                NextPlayer();
            }
        }


        void RemovePlayer(Player currentPlayer)
        {
            DecrementCurrentPlayer();
            pokerTable.playerList.Remove(currentPlayer);
            if (pokerTable.playerList.Count == 1)
            {
                pokerTable.playerList[0].WinMoney();
            }
        }

        void StartNextPhase()
        {
            currentBet = 0;
            Debugger.Log($"Current Stage = {nextStage}");

            foreach (Player p in pokerTable.playerList)
            {
                p.actions.spendThisRound = 0;
            }

            switch (nextStage)
            {
                case PokerStage.PreFlop:
                    StagePreFlop();
                    nextStage = PokerStage.Flop;
                    break;
                case PokerStage.Flop:
                    StageFlop();
                    nextStage = PokerStage.Turn;
                    break;
                case PokerStage.Turn:
                    StageTurn();
                    nextStage = PokerStage.River;
                    break;
                case PokerStage.River:
                    StageRiver();
                    nextStage = PokerStage.Reset;
                    break;
                case PokerStage.Reset:
                    pokerTable.FindWinner();
                    StartGame();
                    break;
            }
        }

        void StagePreFlop()
        {
            currentBet = gameSettings.bigBlind;
            currentPlayerIndex = dealerID;
            dealerID++;
            DealCards();
            PlayBlinds();
            NextPlayer();
        }

        void PlayBlinds()
        {
            IncrementCurrentPlayer();
            pokerTable.playerList[currentPlayerIndex].TakeMoney(gameSettings.smallBlind);
            lastPlayer = GetFinalPlayer();
            IncrementCurrentPlayer();
            pokerTable.playerList[currentPlayerIndex].TakeMoney(gameSettings.bigBlind);
        }

        void StageFlop()
        {
            tableCardsCanvas.enabled = false;
            tableCardsCanvas.enabled = true;
            for (int i = 0; i < 3; i++)
            {
                Card card = pokerTable.deck.DealCard();
                tableCards[i].SetCard(card);
            }
            NextPlayer();
        }

        void StageTurn()
        {
            Card card = pokerTable.deck.DealCard();
            tableCards[3].SetCard(card);
            NextPlayer();
        }

        void StageRiver()
        {
            Card card = pokerTable.deck.DealCard();
            tableCards[4].SetCard(card);
            NextPlayer();
        }

        Player GetFinalPlayer()
        {
            if (currentPlayerIndex == 0)
            {
                return pokerTable.playerList[pokerTable.playerList.Count - 1];
            }
            return pokerTable.playerList[currentPlayerIndex - 1];
        }
        void IncrementCurrentPlayer()
        {
            currentPlayerIndex++;
            if (currentPlayerIndex == pokerTable.playerList.Count)
            {
                currentPlayerIndex = 0;
            }
            //Debugger.Error($"Current player: {currentPlayerIndex}");
        }

        void DecrementCurrentPlayer()
        {
            if (currentPlayerIndex == 0)
            {
                currentPlayerIndex = pokerTable.playerList.Count;
            }
            currentPlayerIndex--;
        }

        void DealCards()
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (Player p in pokerTable.playerList)
                {
                    Card card = pokerTable.deck.DealCard();
                    p.GiveCard(card);
                    p.display.handCards[i].SetCard(card);
                }
            }
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
