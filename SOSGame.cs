using System.IO;
using System.Collections.Generic;
using System;
using System.Text.Json;
using static System.Console;

namespace GameDesign
{
    public class SOSGame : Game
    {
        private SOSGame sos; 
        public SOSGame SOS
        {
            get
            {
                return sos;
            }
            set
            {
                this.sos = value;
            }
        }
        string activeSymbol;
        List<string> symbolList;

        Board sosBoard;
        Player activePlayer;
        Player inactivePlayer;

        bool isUndo = false;
        bool isRedo = false;

        int player1SOSCount;
        int player2SOSCount;

        string input1;
        string input2;
        string symbolToPlace;

        string helpMessage = "Welcome to SOS Game!\nWe are here to help you to know the rules!\n\n"
                             + "Here is the rule:\nTwo players take turns to add either an S or an O (no requirement to use the same letter each turn) on a board with at least 3x3 squares in size.\nIf a player makes the sequence SOS vertically, horizontally or diagonally they get a point and also take another turn."
                             + "Once the grid has been filled up, the winner is the player who made the most SOSs.\n";

        public SOSGame(Player p1, Player p2) : base(p1, p2)
        {
            player1SOSCount = 0;
            player2SOSCount = 0;
            GAME = this;
            this.board = new Board(4,4);
            symbolList = new List<string>() { "S", "O", " " };
            getStart();
            activePlayer = player1;
            inactivePlayer = player2;
            WriteLine(" ");
            Run();
        }

        public override void Run()
        {

            int count = 0;
            while (!this.board.IsTableFilled())
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
                        CheckWin();

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

                        }
                        ++count;
                    }
                    else if (activePlayer is ComputerPlayer)
                    {
                        PlayertTypeMove();
                        ++count;
                    }

                }


                if (count > 2)
                {
                    if (CheckWin())
                    {
                        WriteLine("{0} wins!", player1);
                        break;

                    }
                    else if (CheckWin())
                    {
                        WriteLine("{0} wins!", player2);
                        break;
                    }
                }
                switchPlayer();

            }

        }


        // need to change for sos game
        public void PlayertTypeMove()
        {
            if (player1 is HumanPlayer && player1.Equals(activePlayer))
            {
                // activePlayer choose a symbol from the symbolList in class Piece, using getSymbol(string sym)
                WriteLine($"Player1: ");
                MakeMove();
                activeSymbol = input2;
                if (activeSymbol == "S")
                {
                    activePlayer.playerMoveList = player1.playerMoveList;

                }
                else if (activeSymbol == "O")
                {
                    activePlayer.playerMoveList = player1.playerMoveList;

                }

            }
            else if (player2 is HumanPlayer && player2.Equals(activePlayer))
            {

                WriteLine($"Player2: ");
                MakeMove();
                activeSymbol = input2;
                if (activeSymbol == "S")
                {
                    activePlayer.playerMoveList = player2.playerMoveList;
                }
                else if (activeSymbol == "O")
                {
                    activePlayer.playerMoveList = player2.playerMoveList;
                }

            }
            else if (player2 is ComputerPlayer && player2.Equals(activePlayer))
            {

                WriteLine($"Player2: ");
                SysMakeMove();
                activePlayer.playerMoveList = player2.playerMoveList;
            }

        }

        public override void getStart()
        {
            WriteLine(helpMessage);

            WriteLine("1. Start a new game");
            WriteLine("2. Load game");
            string choice = ReadLine();
            WriteLine(" ");

            if (choice == "2")
            {
                GAME.LoadGameProgress("gameprogress.json", this);
            }
            else
            {
                sosBoard = new Board(4,4);
                modelChoose();
            }

        }


        // Human Player makes move
        public override List<Move> MakeMove()
        {
            int d1 = 0;
            int d2 = 0;

            Write("Please enter your move（i.e.A1 or 1A>> ");
            input1 = ReadLine().ToUpper();
            Write("Please choose which piece you want to put('S' or 'O'), or enter Q to quit>> ");
            input2 = ReadLine().ToUpper();
            getSymbol();
            foreach (string p in symbolList)
            {
                if (input2 == p)
                {
                    break;
                }

            }

            if (input2 == "Q")
            {
                SaveAndExit();
            }

            if (input1.Length == 2 && Char.IsDigit(input1[0]) && Char.IsLetter(input1[1]))
            {
                d1 = input1[1] - 'A' + 1;
                d2 = int.Parse(input1[0].ToString());
                activePlayer.playerMoveList.Add(new Move(d1, d2));


            }
            else if (input1.Length == 2 && Char.IsDigit(input1[1]) && Char.IsLetter(input1[0]))
            {
                d1 = input1[0] - 'A' + 1;
                d2 = int.Parse(input1[1].ToString());
                activePlayer.playerMoveList.Add(new Move(d1, d2));

            }
            else
            {
                WriteLine("Invalid enter, please re-enter your move!");
                MakeMove();

            }
            if (d1 >= 1 && d2 >= 1 && d1 <= board.Row && d2 <= board.Col && board.table[d1][d2] == null)

            {
                WriteLine("Your move is valid!");
                board.table[d1][d2] = input2;
                activePlayer.symbolTrack.Add(input2);
                board.GenerateTable();

            }
            else
            {
                WriteLine("Your move is invalid, please try again!");
                MakeMove();
            }
            return activePlayer.playerMoveList;
        }

        public override string getSymbol()

        {
            string symbolReturn = " ";
            if (activePlayer is HumanPlayer)
            {
                if (((HumanPlayer)activePlayer).symbolTrack.Count > 0)
                {
                    symbolReturn = activePlayer.symbolTrack[activePlayer.symbolTrack.Count - 1];
                }

            }
            else
            {
                if (((ComputerPlayer)activePlayer).symbolTrack.Count > 0)
                {
                    symbolReturn = activePlayer.symbolTrack[activePlayer.symbolTrack.Count - 1];
                }
            }
            return symbolReturn;
        }

        // Computer Player makes move
        public override List<Move> SysMakeMove()
        {
            List<Move> availableMoves = new List<Move>();

            // Find all available empty cells
            for (int i = 1; i < board.table.Length; ++i)
            {
                for (int j = 1; j < board.table[i].Length; ++j)
                {
                    if (board.table[i][j] == null)
                    {
                        availableMoves.Add(new Move(i, j));
                    }
                }
            }

            if (availableMoves.Count > 0)
            {
                Random random = new Random();
                int randomIndex = random.Next(availableMoves.Count);
                Move randomMove = availableMoves[randomIndex];
                int row = randomMove.row;
                int col = randomMove.col;
                symbolToPlace = random.Next(2) == 0 ? "S" : "O";

                activePlayer.playerMoveList.Add(new Move(row, col));

                board.table[row][col] = symbolToPlace;
                activePlayer.symbolTrack.Add(symbolToPlace);
                board.GenerateTable();
                WriteLine($"Computer Player put a {symbolToPlace} in {Convert.ToChar('A' + row - 1)}{col}!");
            }

            return activePlayer.playerMoveList;
        }


        public bool chooseRedoUndo()
        {
            const string UNDO = "U";
            const string REDO = "R";
            const string NEW = "N";

            Write("If you want to undo your move, please enter U, if you want to redo your move, please enter R,\n " +
                "if you want to put a new move, enter N: or or enter Q to quit");
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
                activePlayer.UndoMove(board);
                inactivePlayer.UndoMove(board);

                board.GenerateTable();
                isUndo = true;
                return true;
            }
            else if (input == REDO)
            {

                // Call RedoMove for the active player
                activePlayer.RedoMove(board, GAME, activePlayer.symbolTrack[activePlayer.symbolTrack.Count - 1]);

                // Pass the inactive player's symbol to the inactive player's RedoMove method
                inactivePlayer.RedoMove(board, GAME, inactivePlayer.symbolTrack[inactivePlayer.symbolTrack.Count - 1]);

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

        public bool CheckWin()
        {
            if (CheckHorizontalWin() || CheckVerticalWin() || CheckDiagonalWin())
            {
                return true;
            }
            return false;
        }

        public bool CheckHorizontalWin()
        {
            for (int i = 1; i <= this.board.Row; ++i)
            {
                for (int j = 1; j <= this.board.Col - 2; ++j)
                {
                    if (this.board.table[i][j] == "S" &&
                        this.board.table[i][j + 1] =="O" &&
                        this.board.table[i][j + 2] == "S")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckVerticalWin()
        {
            for (int i = 1; i <= this.board.Row - 2; ++i)
            {
                for (int j = 1; j <= this.board.Col; ++j)
                {
                    if (this.board.table[i][j] == "S" &&
                        this.board.table[i + 1][j] == "O" &&
                        this.board.table[i + 2][j] == "S")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckDiagonalWin()
        {
            for (int i = 1; i <= this.board.Row - 2; ++i)
            {
                for (int j = 1; j <= this.board.Col - 2; ++j)
                {
                    // Check diagonal \
                    if (board.table[i][j] == "S" &&
                        this.board.table[i + 1][j + 1] == "O" &&
                        this.board.table[i + 2][j + 2] == "S")
                    {
                        return true;
                    }

                    // Check diagonal /
                    if (this.board.table[i][j + 2] == "S" &&
                        this.board.table[i + 1][j + 1] == "O" &&
                        this.board.table[i + 2][j] == "S")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}