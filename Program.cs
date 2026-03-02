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
                   .Pipe(b => Game.Draw(b.Board))
                   .Pipe(Game.ReadMove)
                   .Pipe(Game.ProcessCascade);
           }
        }
    }
}
