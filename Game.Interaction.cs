using System;
using System.Collections.Generic;
using System.Linq;

namespace FPgame
{
    public static partial class Game
    {
        public static void Draw(Board board)
        {
            Console.WriteLine("  0 1 2 3 4 5 6 7");
            for (int i = 0; i < 8; i++)
            {
                Console.Write(i + " ");
                for (int j = 0; j < 8; j++)
                {
                    Console.Write(board.cells[i, j].Symbol + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static BoardState ReadMove(BoardState bs)
        {
            Console.WriteLine(">");
            string input = Console.ReadLine();
            if (input == "q")
                Environment.Exit(0);

            Board board = CloneBoard(bs.Board);
            string[] coords = input.Split(' ');
            int x = int.Parse(coords[1]);
            int y = int.Parse(coords[0]);
            int x1 = int.Parse(coords[3]);
            int y1 = int.Parse(coords[2]);
            Element e = board.cells[x, y];
            board.cells[x, y] = board.cells[x1, y1];
            board.cells[x1, y1] = e;
            BoardState bb = new BoardState(board, bs.Score);
            return bb;
        }
    }
}
