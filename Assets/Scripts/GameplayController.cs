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

    public enum PokerStage
    {
        PreFlop,
        Flop,
        Turn,
        River,
        Reset
    }

    public class GameplayController : GameplayControllerBehavior
    {
        [Header("Static Variables")]
        public static GameplayController singleton;

        [Header("Canvases")]
        public Canvas mainMenuCanvas;
        public Canvas tableCanvas;

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
        public Player[] foldedPlayers;
        public List<Player> playerList;

        [Header("Other Table Options")]
        public DebugMode debugMode;
        public PokerStage nextStage = PokerStage.PreFlop;
        public int deckSeed;

        // Start is called before the first frame update
        void Start()
        {
            Debugger.SetDebugMode(debugMode);
            gameSettings = new GameSettings(1000, 50);
            singleton = this;

            if (networkObject == null)
            {
                return;
            }

            Initialize();

            startGameButton.interactable = false;
        }

        void Initialize()
        {
            pokerTable = new Table();
            thisPlayer = AddPlayer(PlayerType.Player);

            if (!networkObject.IsServer)
            {
                return;
            }

            tableDisplay.pokerTable = pokerTable;
            networkObject.deckSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            pokerTable.Setup(networkObject.deckSeed);
            networkObject.SendRpc(RPC_GET_DECK_STRING, Receivers.OthersBuffered, pokerTable.deck.deckValue);

            // Server only functions
            if (NetworkManager.Instance.IsServer)
            {
                foreach (Button b in addAIButtons)
                {
                    b.interactable = true;
                }
            }
        }

        public override void GetDeckString(RpcArgs args)
        {
            string deckString = args.GetNext<string>();
            pokerTable.Setup(deckString);

            tableDisplay.pokerTable = pokerTable;
        }

        public Player AddPlayer(PlayerType playerType)
        {
            int index = networkObject.totalPlayers;
            return AddPlayer(index, playerType);
        }

        public Player AddPlayer(int index, PlayerType playerType)
        {
            Player player = new Player(index, gameSettings.buyIn, this);
            bool isHost = networkObject.IsServer && pokerTable == null;

            pokerTable.AddPlayer(player, playerType);

            networkObject.totalPlayers++;

            if (pokerTable.playerList.Count >= 2 &&
            NetworkManager.Instance.IsServer)
            {
                startGameButton.interactable = true;
            }

            if (playerType != PlayerType.Network)
            {
                networkObject.SendRpc(RPC_ADD_PLAYER, Receivers.OthersBuffered, index, (int)playerType, isHost);
            }

            return player;
        }

        public override void AddPlayer(RpcArgs args)
        {
            int index = args.GetNext<int>();
            PlayerType playerType = (PlayerType)args.GetNext<int>();
            bool isHost = args.GetNext<bool>();

            while (gameSettings == null)
            {
                // WAIT UNTIL ITS SET
            }

            if (isHost)
            {
                host = new Player(index, gameSettings.buyIn, this);
            }
            else
            {
                AddPlayer(index, PlayerType.Network);
            }
        }

        public void StartGame()
        {
            if (networkObject.IsOwner)
            {
                networkObject.SendRpc(RPC_START_GAME, Receivers.OthersBuffered);
            }
            tableDisplay.DisplayWinner();
            foldedPlayers = new Player[pokerTable.playerList.Count];
            singleton = this;
            mainMenuCanvas.enabled = false;
            tableCanvas.enabled = true;
            nextStage = PokerStage.PreFlop;
            StartNextPhase();
        }

        public override void StartGame(RpcArgs args)
        {
            StartGame();
        }

        public void AddAI()
        {
            AddPlayer(PlayerType.AI);
        }

        public void LeaveGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void NextPlayer()
        {
            IncrementCurrentPlayer();
            Player currentPlayer = pokerTable.playerList[currentPlayerIndex];
            Debugger.Log($"Player {currentPlayer.number} turn");
            currentPlayer.actions.PlayerTurn(this, currentPlayer);
        }

        public void EndPlayerTurn(Player currentPlayer, PlayerOption option)
        {
            PlayerActions player = currentPlayer.actions;
            switch (option)
            {
                case PlayerOption.Bet:
                    lastPlayer = GetFinalPlayer();
                    break;
                case PlayerOption.Call:
                    break;
                case PlayerOption.Fold:
                    foldedPlayers[currentPlayer.number] = currentPlayer;
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
                    if (networkObject.IsOwner)
                    {
                        networkObject.SendRpc(RPC_RESTART_GAME, Receivers.AllBuffered);
                    }
                    break;
            }
        }

        public override void RestartGame(RpcArgs args)
        {
            StartCoroutine(RestartGame());
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

            string winnerText = "Winners:";
            foreach (Player p in pokerTable.playerList)
            {
                if (winners.Contains(p))
                {
                    p.money += totalPot / winners.Count;
                    Debugger.Log($"Player {p.number} wins {totalPot / winners.Count}");
                    winnerText += $"\nPlayer {p.number}: ${totalPot / winners.Count}";
                }
            }

            tableDisplay.DisplayWinner(winnerText);

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

            foreach (Player p in foldedPlayers)
            {
                if (p != null)
                {
                    pokerTable.playerList.Insert(p.number, p);
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

            if (networkObject.IsServer)
            {

                networkObject.deckSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                pokerTable.Setup(networkObject.deckSeed);
                networkObject.SendRpc(RPC_GET_DECK_STRING, Receivers.OthersBuffered, pokerTable.deck.deckValue);

                StartGame();
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
            foreach (Player p in pokerTable.playerList)
            {
                p.ResetHand();
            }

            for (int i = 0; i < 2; i++)
            {
                foreach (Player p in pokerTable.playerList)
                {
                    Card card = pokerTable.deck.DealCard();
                    p.GiveCard(card);
                    p.display.handCards[i].SetCard(card, p.actions.playerType);
                }
            }
        }

        public override void SetDeckSeed(RpcArgs args)
        {
            networkObject.deckSeed = args.GetNext<int>();
            deckSeed = networkObject.deckSeed;
        }

        // Update is called once per frame
        void Update()
        {
            if (Debugger.debugMode != debugMode)
            {
                Debugger.SetDebugMode(debugMode);
            }

            playerList = new List<Player>(pokerTable.playerList);
        }
    }
}
