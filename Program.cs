using System;

namespace FPgame
{
    class Program
    {
        static void Main(string[] args)
        {
           BoardState bs = Game.InitializeGame();
           while(true) { 
               Game.Draw(bs.Board);
               bs = Game.ReadMove(bs);
               bs = Game.ProcessCascade(bs);
           }
        }
    }
}
