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

        /// <summary>
        /// Initialize the GameplayController
        /// </summary>
        void Initialize()
        {
            // create a new gameplay controller and add a player
            pokerTable = new Table();
            thisPlayer = AddPlayer(PlayerType.Player);

            // if we aren't the server, return
            if (!networkObject.IsServer)
            {
                return;
            }

            // set the table display's pokerTable to this pokerTable
            tableDisplay.pokerTable = pokerTable;

            // set the networkObject's seed
            networkObject.deckSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

            // setup the pokerTable based on the seed
            pokerTable.Setup(networkObject.deckSeed);

            // share the deck's string to all connections
            networkObject.SendRpc(RPC_UPDATE_DECK_STRING, Receivers.OthersBuffered, pokerTable.deck.deckValue);

            // ensure all the AI buttons are enabled
            foreach (Button b in addAIButtons)
            {
                b.interactable = true;
            }
        }

        /// <summary>
        /// Update the deck string
        /// </summary>
        /// <param name="args">deckString</param>
        public override void UpdateDeckString(RpcArgs args)
        {
            // get the deck string
            string deckString = args.GetNext<string>();

            // setup the pokertable based on that string
            pokerTable.Setup(deckString);

            // set the table display's pokerTable to this pokerTable
            tableDisplay.pokerTable = pokerTable;
        }

        public Player AddPlayer(PlayerType playerType)
        {
            int index = networkObject.totalPlayers;
            return AddPlayer(index, playerType);
        }

        /// <summary>
        /// Add a player
        /// </summary>
        /// <param name="index">index that the player should be in</param>
        /// <param name="playerType">the type of player that is being added</param>
        /// <returns>the player</returns>
        public Player AddPlayer(int index, PlayerType playerType)
        {
            // create a new player with default settings
            Player player = new Player(index, gameSettings.buyIn, this);

            // check if we are the host
            bool isHost = networkObject.IsServer && pokerTable == null && index == 0;

            // add a player to the table
            pokerTable.AddPlayer(player, playerType);

            // if there are at least two players, and we are the server, enable the start game button
            if (pokerTable.playerList.Count >= 2 && NetworkManager.Instance.IsServer)
            {
                startGameButton.interactable = true;
            }

            // if the player isn't from the network
            if (playerType != PlayerType.Network)
            {
                // increase the amount of players
                networkObject.totalPlayers++;

                // send an rpc to add a new player
                networkObject.SendRpc(RPC_ADD_NETWORK_PLAYER, Receivers.OthersBuffered, index, isHost);
            }

            // return the player
            return player;
        }

        /// <summary>
        /// Add a player from the network
        /// </summary>
        /// <param name="args"></param>
        public override void AddNetworkPlayer(RpcArgs args)
        {
            // get the args
            int index = args.GetNext<int>();
            bool isHost = args.GetNext<bool>();

            // loop until the game settings is set
            while (gameSettings == null)
            {
                // INEFFICIENT BUT IF WE DON'T DO THIS CHECK
                // ERRORS WILL OCCUR LATER DOWN THE LINE
            }

            // if the player is the host
            if (isHost)
            {
                // set the host
                host = new Player(index, gameSettings.buyIn, this);
            }
            else
            {
                // add a player with type network
                AddPlayer(index, PlayerType.Network);
            }
        }

        /// <summary>
        /// Start the game
        /// </summary>
        public void StartGame()
        {
            // if we are the owner, send an RPC to start the game for all clients
            if (networkObject.IsOwner)
            {
                networkObject.SendRpc(RPC_START_GAME, Receivers.OthersBuffered);
            }

            // clear the winner text
            tableDisplay.ClearWinner();

            // create a new list of folded players
            foldedPlayers = new Player[pokerTable.playerList.Count];

            // set the singleton to itself to ensure its the only one that exists in the scene
            singleton = this;

            // disable the main menu canvas
            mainMenuCanvas.enabled = false;

            // enable the table canvas
            tableCanvas.enabled = true;

            // set the stage to pre flop
            nextStage = PokerStage.PreFlop;

            // start the next phase
            StartNextPhase();
        }

        /// <summary>
        /// Start the game (via the network)
        /// </summary>
        /// <param name="args"></param>
        public override void StartGame(RpcArgs args)
        {
            // start the game
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

        /// <summary>
        /// Start the next phase
        /// </summary>
        void StartNextPhase()
        {
            // set the current bet to 0
            currentBet = 0;
            // log the current stage
            Debugger.Log($"Current Stage = {nextStage}");

            // set the players spent this round to 0
            foreach (Player p in pokerTable.playerList)
            {
                p.actions.spendThisRound = 0;
            }

            // check what stage it is and run the program accordingly
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
                    // if we own this object, sent an RPC to restart ALL games
                    if (networkObject.IsOwner)
                    {
                        networkObject.SendRpc(RPC_RESTART_GAME, Receivers.AllBuffered);
                    }
                    break;
            }
        }

        /// <summary>
        /// Restart the game from the netowkr
        /// </summary>
        /// <param name="args"></param>
        public override void RestartGame(RpcArgs args)
        {
            // start the coroutine to restart th game
            StartCoroutine(RestartGame());
        }

        private void FindWinners()
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
        }

        void ResetPlayerLists()
        {
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
        }

        void ResetPlayers()
        {

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
        }

        void ResetTableCards()
        {
            foreach (DisplayCard c in tableCards)
            {
                c.Setup();
            }
        }


        /// <summary>
        /// Restart the game.
        /// </summary>
        /// <returns></returns>
        IEnumerator RestartGame()
        {
            // find the winners
            FindWinners();

            // reset the player lists
            ResetPlayerLists();

            // wait for 5 seconds
            yield return new WaitForSeconds(5);

            // reset the players
            ResetPlayers();

            // reset the table cards
            ResetTableCards();

            // if we are the server
            if (networkObject.IsServer)
            {
                // the table with a new deck seed
                networkObject.deckSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                pokerTable.Setup(networkObject.deckSeed);

                // share the deck's string to all connections
                networkObject.SendRpc(RPC_UPDATE_DECK_STRING, Receivers.OthersBuffered, pokerTable.deck.deckValue);

                // start the game
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
