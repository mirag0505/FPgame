using System;
using System.Collections.Generic;
using System.Linq;

namespace FPgame
{
    public static partial class Game
    {
        static Random r = new Random();
        static char[] symbols = { 'A', 'B', 'C', 'D', 'E', 'F' };

        public static BoardState InitializeGame()
        {
            int size = 8;
            var initialBoard = CreateRandomBoard(size);
            return new BoardState(initialBoard, 0);
        }

        public static Board CreateRandomBoard(int size)
        {
            var board = new Board(size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    board.cells[i, j] = new Element(symbols[r.Next(symbols.Length)]);
                }
            }
            return board;
        }

        public static Board CloneBoard(Board board)
        {
            Board b = new Board(board.size);
            for (int row = 0; row < board.size; row++)
                for (int col = 0; col < board.size; col++)
                    b.cells[row, col] = board.cells[row, col];
            return b;
        }
    }
}
