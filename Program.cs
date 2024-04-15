
using static System.Console;
using System;
using System.Numerics;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.Intrinsics.X86;


namespace GameDesign
{
    class Program
    {
        public static void Main(string[] args)
        {
            Player p1 = new Player("1");
            Player p2 = new Player("2");

            Connect4Game connect4 = new Connect4Game(p1, p2);

            SOSGame sOS = new SOSGame(p1, p2);

        }
    }
}

