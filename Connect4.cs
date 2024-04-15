using System.IO;
using System.Collections.Generic;
using System;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Text.Json;

using static System.Console;

namespace GameDesign
{
    public class Connect4Game : Game
    {
        private Connect4Game c4;
        public Connect4Game C4
        {
            get
            {
                return c4;
            }
            set
            {
                this.c4 = value;
            }
        }
        string symbol1;
        string symbol2;
        List<string> symbolList;

        Player activePlayer;
        Player inactivePlayer;
        Board connect4Board;
        
        string activeSymbol;
        string inactiveSymbol;

        bool isUndo = false;
        bool isRedo = false;


        string input;

        string helpMessage = "Welcome to Connect Four Game!\nWe are here to help you to know the rules!\n"
                             + "Here is the rule:\nEach player forms an unbroken chain of four pieces horizontally, vertically, or diagonally.\n"
                             + "Who forms the unbroken chain forst, who wins the game.\n";

        public Connect4Game(Player p1, Player p2):base(p1, p2)
        {
            GAME = this;
            this.board = new Board(6, 7);
            symbolList = new List<string>() { "O", "X", " " };
            symbol1 = symbolList[0];
            symbol2 = symbolList[1];
            WriteLine(" ");
            getStart();
            activePlayer = player1;
            inactivePlayer = player2;
            WriteLine(" ");
            Run();
        }

        public override void Run()
        {
            int count = 0;
            while (!board.IsTableFilled())
            {
                if (count < 2)
                {
                    PlayertTypeMove();
                    ++count;
                }
                else
                {

                    if (activePlayer is HumanPlayer)
                    {
                        chooseRedoUndo();
                        ++count;

                        while (isUndo == true || isRedo == true)
                        {
                            bool flag = chooseRedoUndo();
                            if (flag == true)
                            {
                                chooseRedoUndo();

                            }
                            else
                            {
                                break;
                            }
                            ++count;
                        }
                    }
                    else if (activePlayer is ComputerPlayer)
                    {
                        PlayertTypeMove();
                        ++count;
                    }

                }

                if (count > 6)
                {
                    if (checkWin(symbol1))
                    {
                        WriteLine("{0} wins!", player1);
                        break;

                    }
                    else if (checkWin(symbol2))
                    {
                        WriteLine("{0} wins!", player2);
                        break;
                    }
                }

                switchPlayer();
            }

        }

        public void PlayertTypeMove()
        {
            if (player1 is HumanPlayer && player1.Equals(activePlayer))
            {
                WriteLine($"Player1: ");
                MakeMove(symbol1);
                activePlayer.playerMoveList = player1.playerMoveList;
            }
            else if (player2 is HumanPlayer && player2.Equals(activePlayer))
            {
                WriteLine($"Player2: ");
                MakeMove(symbol2);
                activePlayer.playerMoveList = player2.playerMoveList;
            }
            else if (player2 is ComputerPlayer && player2.Equals(activePlayer))
            {
                WriteLine($"Player2: ");
                SysMakeMove(symbol2);
                activePlayer.playerMoveList = player2.playerMoveList;
            }

        }

        public override void getStart()
        {
            WriteLine(helpMessage);

            WriteLine("1. Start a new game");
            WriteLine("2. Load game");
            string choice = ReadLine();

            if (choice == "2")
            {
                GAME.LoadGameProgress("gameprogress.json", this);
            }
            else
            {
                connect4Board = new Board(6, 7);
                modelChoose();
            }
        }

        // Human Player makes move
        public List<Move> MakeMove(string symbol)
        {
            int col = 0;
            int row = board.Row; // Start from the bottom-most row
            Write("Please enter the column you want to put your move (number 1 - 7), or enter Q to save and quit >> ");
            input = ReadLine().ToUpper();

            if (input == "Q")
            {
                SaveAndExit();
            }
            else if (input.Length == 1 && Char.IsDigit(input[0])
                && int.Parse(input) >= 1 && int.Parse(input) <= 7)
            {
                col = int.Parse(input);
                while (row > 0 && board.table[row][col] != null) // Find the available position from the bottom
                {
                    row--;
                }

                if (row == 0)
                {
                    WriteLine("The column is full, please choose another one!");
                                return MakeMove(symbol);
                }

                activePlayer.playerMoveList.Add(new Move(row, col));
                
            }
            else
            {
                WriteLine("Invalid entry, please re-enter your move!");
                return MakeMove(symbol);
            }

            if (this.board.table[row][col] == null)
            {
                this.board.table[row][col] = symbol;
                getSymbol();
                this.board.GenerateTable();
            }
            else
            {
                WriteLine("Your move is invalid, please try again!");
                return MakeMove(symbol);
            }
            return activePlayer.playerMoveList;
        }

        public override string getSymbol()
        {
            if (activePlayer is HumanPlayer && player1.Equals(activePlayer))
            {
                foreach (string p in symbolList)
                {
                    if (symbol1 == p)
                    {
                        activePlayer.symbolTrack.Add(p);
                    }

                }

            }
            else if (activePlayer is HumanPlayer && player2.Equals(activePlayer))
            {
                foreach (string p in symbolList)
                {
                    if (symbol2 == p)
                    {
                        activePlayer.symbolTrack.Add(p);
                    }
                }

            }
            else if (activePlayer is ComputerPlayer && player2.Equals(activePlayer))
            {
                foreach (string p in symbolList)
                {
                    if (symbol2 == p)
                    {
                        activePlayer.symbolTrack.Add(p);
                    }
                }

            }
            return activePlayer.symbolTrack[activePlayer.symbolTrack.Count - 1];
        }

        public bool chooseRedoUndo()
        {
            const string UNDO = "U";
            const string REDO = "R";
            const string NEW = "N";

            Write("If you want to undo your move, please enter U, if you want to redo your move, please enter R, if you want to put a new move, enter N: ");
            string input = ReadLine().ToUpper();
            if (input == NEW)
            {
                PlayertTypeMove();
                isRedo = false;
                isUndo = false;
            }
            else if (input == "Q")
            {
                SaveAndExit();
            }
            else if (input == UNDO)
            {
                activePlayer.UndoMove(this.board);
                inactivePlayer.UndoMove(this.board);
                this.board.GenerateTable();
                isUndo = true;
                return true;
            }
            else if (input == REDO)
            {

                // Call RedoMove for the active player
                activeSymbol = activePlayer.symbolTrack[activePlayer.symbolTrack.Count - 1];
                activePlayer.RedoMove(this.board, GAME, activeSymbol);

                // Pass the inactive player's symbol to the inactive player's RedoMove method
                inactiveSymbol = inactivePlayer.symbolTrack[inactivePlayer.symbolTrack.Count - 1];
                inactivePlayer.RedoMove(this.board, GAME, inactiveSymbol);



                board.GenerateTable();
                isRedo = true;
                return true;
            }
            else
            {
                WriteLine("Invalid input, system does not understand your operation!");
                WriteLine("Please try again!");
                chooseRedoUndo();
            }
            return false;
        }

        public override Player switchPlayer()
        {
            if (activePlayer.ID == player1.ID)
            {
                if (player2 is HumanPlayer)
                {
                    activePlayer = player2;
                    inactivePlayer = player1;
                }
                else if (player2 is ComputerPlayer)
                {
                    activePlayer = player2;
                    inactivePlayer = player1;
                }
            }
            else
            {
                activePlayer = player1;
                inactivePlayer = player2;

            }

            return activePlayer;
        }

        // Computer Player makes move
        public List<Move> SysMakeMove(string symbol)
        {
            int col = 0;
            int row = this.board.Row; // Start from the bottom-most row
            Random random = new Random();
            col = random.Next(1, 8); // Random number between 1 and 7
            while (row > 0 && this.board.table[row][col] != null) // Find the available position from the bottom
            {
                row--;
            }

            while (row == 0) // If the chosen column is full, choose another column
            {
                col = random.Next(1, 8);
                row = this.board.Row;
                while (row > 0 && this.board.table[row][col] != null)
                {
                    row--;
                }
            }

            activePlayer.playerMoveList.Add(new Move(row, col));
            board.table[row][col] = symbol;
            getSymbol();
            this.board.GenerateTable();
            WriteLine($"Computer Player put a move in {Convert.ToChar('A' + row - 1)}{col}!");

            return activePlayer.playerMoveList;
        }

        public bool checkWin(string symbol)
        {
            if (checkHorizontalWin(symbol) || CheckVerticalWin(symbol)
                || CheckDiagonalWin(symbol))
            {
                return true;
            }
            return false;
        }

        public bool checkHorizontalWin(string symbol)
        {
            for (int i = 1; i < this.board.table.Length; ++i)
            {
                for (int j = 1; j <= this.board.table[i].Length - 4; ++j)
                {
                    if (this.board.table[i][j] == symbol &&
                        this.board.table[i][j + 1] == symbol &&
                        this.board.table[i][j + 2] == symbol &&
                        this.board.table[i][j + 3] == symbol)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckVerticalWin(string symbol)
        {
            for (int i = 1; i <= this.board.table.Length - 4; ++i)
            {
                for (int j = 1; j < board.table[i].Length; ++j)
                {
                    if (this.board.table[i][j] == symbol &&
                        this.board.table[i + 1][j] == symbol &&
                        this.board.table[i + 2][j] == symbol &&
                        this.board.table[i + 3][j] == symbol)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckDiagonalWin(string symbol)
        {
            for (int i = 1; i <= this.board.table.Length - 4; ++i)
            {
                for (int j = 1; j <= this.board.table[i].Length - 4; ++j)
                {
                    // check diagonal \
                    if (this.board.table[i][j] == symbol &&
                        this.board.table[i + 1][j + 1] == symbol &&
                        this.board.table[i + 2][j + 2] == symbol &&
                        this.board.table[i + 3][j + 3] == symbol)
                    {
                        return true;
                    }
                    // check diagonal /
                    if (this.board.table[i + 3][j] == symbol &&
                        this.board.table[i + 2][j + 1] == symbol &&
                        this.board.table[i + 1][j + 2] == symbol &&
                        this.board.table[i][j + 3] == symbol)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}

