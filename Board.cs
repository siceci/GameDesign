using System;
using System.Text.Json;
using static System.Console;

namespace GameDesign
{
    public class Board
    {
        public int Col { get; set; }
        public int Row { get; set; }
        public Piece piece;
        public Move move;

        public string[][] table;

        public Board()
        {
            //table = new string[][] { };
            //Interaction();
            InitializeTable();
            GenerateTable();
        }

        public Board(int row, int col)
        {
            this.Row = row;
            this.Col = col;
            //table = new string[][] { };
            InitializeTable();
            GenerateTable();
        }


       /* public void Interaction()
        {
            const int MAX = 9;
            const int MIN = 3;

            WriteLine("Please enter your borad size");
            Write("Row number(3 <= Row <= 9): ");
            Row = Convert.ToInt32(ReadLine());
            while (Row > MAX || Row < MIN)
            {
                Write("Invalid size! Please re-enter your row number>> ");
                Row = Convert.ToInt32(ReadLine());
            }
            Write("Column number(3 <= Column <= 9) >> ");
            Col = Convert.ToInt32(ReadLine());
            while (Col > MAX || Col < MIN)
            {
                Write("Invalid size! Please re-enter your row number>> ");
                Col = Convert.ToInt32(ReadLine());
            }
        }
*/


        private void InitializeTable()
        {

            table = new string[Row + 1][];
            table[0] = new string[Col + 1];
            table[0][0] = " ";

            for (int i = 1; i <= Row; i++)
            {
                table[i] = new string[Col + 1];
                table[i][0] = ((char)('A' + i - 1)).ToString();
            }

            for (int j = 1; j <= Col; j++)
            {
                table[0][j] = j.ToString();
            }
        }

        public void GenerateTable()
        {
            int rows = table.Length;
            int cols = table.First().Length;
            var header = $"┌─{string.Join("", Enumerable.Repeat("──┬─", cols - 1))}──┐";
            var middle = $"├─{string.Join("", Enumerable.Repeat("──┼─", cols - 1))}──┤";
            var footer = $"└─{string.Join("", Enumerable.Repeat("──┴─", cols - 1))}──┘";

            WriteLine(header);
            for (int i = 0; i < rows; ++i)
            {
                foreach (var cell in table[i])
                    Write($"│  {cell ?? " "}");
                Console.WriteLine("│");
                if (i < rows - 1)
                    WriteLine(middle);
            }
            WriteLine(footer);
        }

        public bool IsTableFilled()
        {
            foreach (var row in table)
            {
                foreach (var cell in row)
                {
                    if (cell == " ")
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

