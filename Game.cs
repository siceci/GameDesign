using System;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using static System.Console;

namespace GameDesign
{

    public abstract class Game
    {
        public string helpMessage;
        public string[][] table;
        public Player player1;
        public Player player2;
        public Player activePlayer;
        public Board board;
        public Piece piece;
        public Move move;
        public static string model;
        public List<Player> playerList;


        private Game game;
        public Game GAME
        {
            get
            {
                return game;
            }

            set
            {
                this.game = value;
            }
        }

        public Game(Player player1, Player player2)
        {
            this.player1 = player1;
            this.player2 = player2;
            this.playerList = new List<Player>();
        }

        public virtual void getStart() { }


        public virtual void
            Run() { }

        public virtual string getSymbol() { return activePlayer.symbolTrack[activePlayer.symbolTrack.Count - 1]; }

        public virtual Player switchPlayer() { return null; }


        public void SaveAndExit()
        {
            SaveGameProgress("gameprogress.json");
            Environment.Exit(0);

        }

        public void LoadGameProgress(string fileName, Game gameInstance)
        {
            if (File.Exists(fileName))
            {
                string json = File.ReadAllText(fileName);
                GameProgress gameProgress = JsonSerializer.Deserialize<GameProgress>(json);
                if (gameProgress != null && gameProgress.Table != null)
                {
                    board.table = gameProgress.Table;
                    board.GenerateTable();
                    if (!string.IsNullOrEmpty(gameProgress.SelectedModel))
                    {
                        model = (gameProgress.SelectedModel == "Human VS Human") ? "1" : "2";

                        if (model == "1")
                        {
                            gameInstance.player1 = new HumanPlayer("1");
                            gameInstance.player2 = new HumanPlayer("2");
                            gameInstance.playerList = new List<Player> { gameInstance.player1, gameInstance.player2 };
                        }
                        else if (model == "2")
                        {
                            gameInstance.player1 = new HumanPlayer("1");
                            gameInstance.player2 = new ComputerPlayer("2");
                            gameInstance.playerList = new List<Player> { gameInstance.player1, gameInstance.player2 };
                        }
                    }
                }
                else
                {
                    WriteLine("Error: Loaded game progress is invalid or incomplete.\n");
                }
            }

        }

        public void SaveGameProgress(string fileName)
        {

            GameProgress gameProgress = new GameProgress
            {
                Table = board.table,
                SelectedModel = (model == "1") ? "Human VS Human" : "Human VS Computer"
            };

            string json = JsonSerializer.Serialize(gameProgress);
            File.WriteAllText(fileName, json);
        }


        public List<Player> modelChoose()
        {
            const string MODEL1 = "Human VS Human";
            const string MODEL2 = "Human VS Computer";

            Write("If you want to play with Human, please enter 1, if you want to play with Computer, please enter 2 >> ");
            model = ReadLine();
            bool flag = true;
            do
            {
                //Human plays with Human
                if (model == "1")
                {
                    flag = true;
                    player1 = new HumanPlayer("1");
                    player2 = new HumanPlayer("2");
                    playerList = new List<Player> { player1, player2 };
                    WriteLine("You are in the Model of {0}!", MODEL1);
                    WriteLine("Player list: {0}, {1}\n", player1.ToString(), player2.ToString());
                }

                //Huamn plays with Computer
                else if (model == "2")
                {
                    flag = true;
                    player1 = new HumanPlayer("1");
                    player2 = new ComputerPlayer("2");
                    playerList = new List<Player> { player1, player2 };
                    WriteLine("You are in the Model of {0}!", MODEL2);
                    WriteLine("Player list: {0}, {1}\n", player1.ToString(), player2.ToString());
                }
                else
                {
                    WriteLine("Please enter a valid value (1 or 2)!\n");
                    model = ReadLine();
                    flag = false;
                }
            } while (!flag);

            return playerList;
        }

        public virtual List<Move> MakeMove(string symbol)
        {
            return activePlayer.playerMoveList;

        }
        public virtual List<Move> MakeMove()
        {
            return activePlayer.playerMoveList;

        }

        public virtual List<Move> SysMakeMove()
        {
            return activePlayer.playerMoveList;

        }


    }



    public class GameProgress
    {
        public string[][] Table { get; set; }
        public string SelectedModel { get; set; }
    }

}

