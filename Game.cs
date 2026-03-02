using System;
using System.Collections.Generic;
using System.Linq;

namespace FPgame
{
    public static partial class Game
    {
        internal static Random r = new Random();
        static char[] symbols = { 'A', 'B', 'C', 'D', 'E', 'F' };

        public static BoardState InitializeGame(int boardSize = 8)
        {
            return new BoardState(new Board(boardSize), 0)
                .Pipe(bs => FillEmptySpaces(bs, r))
                .Pipe(bs => ProcessCascade(bs, r));
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
