using System;

namespace FPgame
{
    class Program
    {
        static void Main(string[] args)
        {
           BoardState bs = Game.InitializeGame();
           while(true) 
           { 
               bs = bs
                   .Draw()
                   .Move()
                   .Cascade();
           }
        }
    }
}
