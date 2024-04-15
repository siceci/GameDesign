using System;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using static System.Console;

namespace GameDesign
{
    public class HumanPlayer : Player
    {

        public HumanPlayer(string id) : base(id)
        {
          
        }


        public override Move GetMove()
        {
            return base.GetMove();
        }

        public override List<Move> UndoMove(Board board)
        {

            if (playerMoveList.Count > 0)
            {
                int index = playerMoveList.Count - 1;
                Move lastMove = playerMoveList[index];
                redoMovelist.Add(lastMove);
                playerMoveList.RemoveAt(index);
                board.table[lastMove.row][lastMove.col] = " ";
                WriteLine("Undo move successfully!");
                return redoMovelist;
            }
            WriteLine("Undo move unsuccessfully!");
            return null;
        }

        public new void RedoMove(Board board, Game game, string sym) { }


        public override string ToString()
        {
            return "Human player";
        }

    }

    public class ComputerPlayer : Player
    {
        public ComputerPlayer(string id) : base(id)
        {
            
        }

        public new Move GetMove()
        {
            return new Move();
        }


        public override List<Move> UndoMove(Board board)
        {

            if (playerMoveList.Count > 0)
            {
                int index = playerMoveList.Count - 1;
                Move lastMove = playerMoveList[index];
                redoMovelist.Add(lastMove);
                playerMoveList.RemoveAt(index);
                board.table[lastMove.row][lastMove.col] = " ";
                WriteLine("Undo move successfully!");
                return redoMovelist;
            }
            WriteLine("Undo move unsuccessfully!");
            return null;
        }

        public new void RedoMove(Board board, Game game, string sym) { }

        public override string ToString()
        {
            return "Computer player";
        }

    }

    public class Player
    {
        public string ID { get; set; }
        public Move move;

        public List<Player> playerList;
        public List<string> symbolTrack;

        public List<Move> playerMoveList = new List<Move>();
        public List<Move> redoMovelist = new List<Move>();

        public Player(string id)
        {
            this.ID = id;
            this.playerMoveList = new List<Move>();
            this.symbolTrack = new List<string>();
        }


        public virtual Move GetMove()
        {
            if (playerMoveList != null && playerMoveList.Count > 0)
            {
                int moveIndex = playerMoveList.Count - 1;
                Move lastMove = playerMoveList[moveIndex];
                return lastMove;
            }

            return null;
        }


        public override bool Equals(Object obj)
        {
            bool equal;
            Player player = (Player)obj;

            if (this.ID == player.ID)
            {
                equal = true;
            }
            else
            {
                equal = false;
            }
            return equal;
        }

        public virtual bool validMoveCheck()
        {
            return true;
        }
        public virtual List<Move> UndoMove(Board board) { return null; }

        public virtual void RedoMove(Board board, Game GAME, string symbol)
        {
            // Check if redoMovelist has moves
            if (redoMovelist.Count > 0)
            {
                int index = redoMovelist.Count - 1;
                Move redoMove = redoMovelist[index];

                // Check if board is initialized
                if (board == null)
                {
                    WriteLine("board is null!");
                }
                else
                {
                    // Check if redoMove is within bounds
                    if (redoMove.row >= 0 && redoMove.row < board.table.Length &&
                        redoMove.col >= 0 && redoMove.col < board.table[0].Length)
                    {
                        // Use the symbol passed as an argument
                        board.table[redoMove.row][redoMove.col] = symbol;
                        WriteLine("Redo your move successfully!");
                    }
                    else
                    {
                        WriteLine("Invalid redo move!");
                    }
                }
            }
            else
            {
                WriteLine("Redo your move unsuccessfully!");
            }
        }



    }

}

