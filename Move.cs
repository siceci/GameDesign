using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using static System.Console;

namespace GameDesign
{
    public class Move
    {
        public int col;
        public int row;


        public Move(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public Move()
        {

        }



    }

}

