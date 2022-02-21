using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;

namespace Poker.Game
{
    using Utils;
    using Display;
    using Players;
    using BeardedManStudios.Forge.Networking;

    enum PokerStage
    {
        PreFlop,
        Flop,
        Turn,
        River,
        Reset
    }

    public class GameplayController : GameplayControllerBehavior
    {
        public static GameplayController singleton;

        [Header("Table Settings")]
        public int currentBet;
        public Table pokerTable;
        [SerializeField] DisplayTable tableDisplay;
        [SerializeField] Button startGameButton;
        [SerializeField] Canvas tableCardsCanvas;
        GameSettings gameSettings;
        public List<Button> addAIButtons;
        public List<DisplayCard> tableCards;


        [Header("Players")]
        public List<PlayerActions> allPlayers;
        public List<DisplayPlayer> playerDisplays;
        public Player finalPlayer;
        public int currentPlayerIndex;
        int dealerID = 0;
        // players to track directly internally
        Player host, lastPlayer, thisPlayer;
        public Queue<Player> playerQueue;
        public List<Player> foldedPlayers = new List<Player>();

        [Header("Other Table Options")]
        public DebugMode debugMode;
        PokerStage nextStage = PokerStage.PreFlop;

        // Start is called before the first frame update
        void Start()
        {
            Debugger.SetDebugMode(debugMode);
            gameSettings = new GameSettings(1000, 50);
            if (singleton == null)
            {
                singleton = this;
            }
            else
            {
                gameObject.SetActive(false);
                return;
            }

            if (networkObject == null)
            {
                return;
            }

            if (networkObject.IsServer)
            {
                Initialize();
            }
            startGameButton.interactable = false;
        }

        void Initialize()
        {
            thisPlayer = AddPlayer(false);
            pokerTable.Setup(networkObject.deckSeed);
            tableDisplay.pokerTable = pokerTable;
            foreach (Button b in addAIButtons)
            {
                b.interactable = true;
            }
        }

        public Player AddPlayer(bool isAI)
        {
            int index = networkObject.totalPlayers;
            return AddPlayer(isAI, index, false);
        }

        public Player AddPlayer(bool isAI, int index, bool fromNetwork)
        {
            Player player = new Player(index, gameSettings.buyIn, this);
            bool isHost = networkObject.IsServer && pokerTable == null;

            if (isHost)
            {
                pokerTable = new Table(player);
            }
            else
            {
                pokerTable.AddPlayer(player, isAI);
            }
            networkObject.totalPlayers++;

            Debug.Log($"{pokerTable.playerList}");

            if (pokerTable.playerList.Count >= 2)
            {
                startGameButton.interactable = true;
            }

            if (!fromNetwork)
            {
                networkObject.SendRpc(RPC_ADD_PLAYER, Receivers.OthersBuffered, index, isAI, isHost);
            }

            return player;
        }

        public override void AddPlayer(RpcArgs args)
        {
            int index = args.GetNext<int>();
            bool isAI = args.GetNext<bool>();
            bool isHost = args.GetNext<bool>();

            while (gameSettings == null)
            {
                // WAIT UNTIL ITS SET
            }

            if (isHost)
            {
                host = new Player(index, gameSettings.buyIn, this);
                pokerTable = new Table(host);
                Initialize();
            }
            else
            {
                AddPlayer(isAI, index, true);
            }
        }

        public void AddAI()
        {
            AddPlayer(true);
        }

        public void StartGame()
        {
            nextStage = PokerStage.PreFlop;
            StartNextPhase();
        }

        public void LeaveGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
                    foldedPlayers.Add(currentPlayer);
                    RemovePlayer(currentPlayer);
                    lastPlayer = GetNextPlayer();
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
                nextStage = PokerStage.Reset;
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
                    StartCoroutine(RestartGame());
                    break;
            }
        }

        IEnumerator RestartGame()
        {
            List<Player> winners = new List<Player>();
            if (pokerTable.playerList.Count == 1)
            {
                winners.Add(pokerTable.playerList[0]);
            }
            else
            {
                winners = pokerTable.FindWinner();
            }
            int totalPot = pokerTable.GetTotalPot();

            foreach (Player p in pokerTable.playerList)
            {
                if (winners.Contains(p))
                {
                    p.money += totalPot / winners.Count;
                    Debugger.Log($"Player {p.number} wins {totalPot / winners.Count}");
                }
            }

            foreach (Player p in foldedPlayers)
            {
                pokerTable.playerList.Insert(p.number, p);
            }

            foreach (Player p in pokerTable.playerList)
            {
                foreach (DisplayCard c in p.display.handCards)
                {
                    if (c.showToPlayer == false)
                    {
                        StartCoroutine(c.FlipCard(180));
                    }
                }
            }

            yield return new WaitForSeconds(5);

            foreach (Player p in pokerTable.playerList)
            {
                p.Setup();
                foreach (DisplayCard c in p.display.handCards)
                {
                    if (c.showToPlayer == false)
                    {
                        StartCoroutine(c.FlipCard(0));
                    }
                }
            }

            foreach (DisplayCard c in tableCards)
            {
                c.Setup();
            }

            pokerTable.Setup(networkObject.deckSeed);

            foldedPlayers = new List<Player>();

            StartGame();
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
                pokerTable.cards[i] = card;
            }
            NextPlayer();
        }

        void StageTurn()
        {
            Card card = pokerTable.deck.DealCard();
            tableCards[3].SetCard(card);
            pokerTable.cards[3] = card;
            NextPlayer();
        }

        void StageRiver()
        {
            Card card = pokerTable.deck.DealCard();
            tableCards[4].SetCard(card);
            pokerTable.cards[4] = card;
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

        Player GetNextPlayer()
        {
            int playerIndex = currentPlayerIndex + 1;
            if (playerIndex >= pokerTable.playerList.Count)
            {
                playerIndex = 0;
            }
            return pokerTable.playerList[playerIndex];
        }

        void IncrementCurrentPlayer()
        {
            currentPlayerIndex++;
            if (currentPlayerIndex >= pokerTable.playerList.Count)
            {
                currentPlayerIndex = 0;
            }
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

        public override void SetDeckSeed(RpcArgs args)
        {
            throw new System.NotImplementedException();
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
