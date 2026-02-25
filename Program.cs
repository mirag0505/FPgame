using System;
using System.Collections.Generic;
using System.Linq;

namespace FPgame
{
    public struct Element
    {
        public int Symbol;
        public static readonly int EMPTY = 0;

        public Element(int symbol)
        {
            Symbol = symbol;
        }

        public override string ToString()
        {
            return Symbol == EMPTY ? "." : Symbol.ToString();
        }
    }

    public class Board
    {
        public int size;
        public Element[,] cells;

        public Board(int size)
        {
            this.size = size;
            cells = new Element[size, size];
        }
        
        // Copy constructor for immutability simulation
        public Board(Board other)
        {
            size = other.size;
            cells = (Element[,])other.cells.Clone();
        }
    }

    public class BoardState
    {
        public Board Board { get; }
        public int Score { get; }

        public BoardState(Board board, int score)
        {
            Board = board;
            Score = score;
        }
    }

    public enum MatchDirection { Horizontal, Vertical }

    public struct Match
    {
        public readonly MatchDirection Direction;
        public readonly int Row;
        public readonly int Col;
        public readonly int Length;

        public Match(MatchDirection direction, int row, int col, int length)
        {
            Direction = direction;
            Row = row;
            Col = col;
            Length = length;
        }
    }

    class Program
    {
        private static readonly Random r = new Random();
        private static readonly int[] symbols = { 1, 2, 3, 4, 5 }; // Example symbols

        static void Main(string[] args)
        {
            // Initial setup
            int size = 8;
            var initialBoard = new Board(size);
            
            // Fill initial board
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    initialBoard.cells[i, j] = new Element(symbols[r.Next(symbols.Length)]);
                }
            }

            var initialState = new BoardState(initialBoard, 0);

            Console.WriteLine("Initial Board:");
            PrintBoard(initialState.Board);

            var finalState = ProcessCascade(initialState);

            Console.WriteLine("\nFinal Board:");
            PrintBoard(finalState.Board);
            Console.WriteLine($"Final Score: {finalState.Score}");
        }

        private static void PrintBoard(Board board)
        {
            for (int i = 0; i < board.size; i++)
            {
                for (int j = 0; j < board.size; j++)
                {
                    Console.Write(board.cells[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        // --- Core Logic from Tasks.md ---

        public static BoardState ProcessCascade(BoardState currentState)
        {
            // 1. Находим комбинации
            var matches = FindMatches(currentState.Board);

            // 2. Если не найдено ни одной, завершаем работу (базовый случай рекурсии)
            if (matches.Count == 0)
            {
                return currentState;
            }

            // 3. Удаляем комбинации, считаем статистику/бонусы
            // Note: RemoveMatches handles gravity application as per Tasks.md
            var stateAfterRemoval = RemoveMatches(currentState, matches);

            // 4. Заполняем пустые клетки
            var stateAfterFilling = FillEmptySpaces(stateAfterRemoval);

            // 5. Возвращаем рекурсивный вызов с новым состоянием
            return ProcessCascade(stateAfterFilling);
        }

        public static List<Match> FindMatches(Board board)
        {
            var matches = new List<Match>();

            // Горизонтальные комбинации
            for (int row = 0; row < board.size; row++)
            {
                int startCol = 0;
                for (int col = 1; col < board.size; col++)
                {
                    if (board.cells[row, startCol].Symbol == Element.EMPTY)
                    {
                        startCol = col;
                        continue;
                    }

                    if (board.cells[row, col].Symbol == Element.EMPTY)
                    {
                        AddMatchIfValid(matches, row, startCol, col - startCol, MatchDirection.Horizontal);
                        startCol = col + 1;
                        continue;
                    }

                    if (board.cells[row, col].Symbol != board.cells[row, startCol].Symbol)
                    {
                        AddMatchIfValid(matches, row, startCol, col - startCol, MatchDirection.Horizontal);
                        startCol = col;
                    }
                    else if (col == board.size - 1)
                    {
                        AddMatchIfValid(matches, row, startCol, col - startCol + 1, MatchDirection.Horizontal);
                    }
                }
            }

            // Вертикальные комбинации
            for (int col = 0; col < board.size; col++)
            {
                int startRow = 0;
                for (int row = 1; row < board.size; row++)
                {
                    if (board.cells[startRow, col].Symbol == Element.EMPTY)
                    {
                        startRow = row;
                        continue;
                    }

                    if (board.cells[row, col].Symbol == Element.EMPTY)
                    {
                        AddMatchIfValid(matches, startRow, col, row - startRow, MatchDirection.Vertical);
                        startRow = row + 1;
                        continue;
                    }

                    if (board.cells[row, col].Symbol != board.cells[startRow, col].Symbol)
                    {
                        AddMatchIfValid(matches, startRow, col, row - startRow, MatchDirection.Vertical);
                        startRow = row;
                    }
                    else if (row == board.size - 1)
                    {
                        AddMatchIfValid(matches, startRow, col, row - startRow + 1, MatchDirection.Vertical);
                    }
                }
            }

            return matches;
        }

        private static void AddMatchIfValid(List<Match> matches, int row, int col, int length, MatchDirection direction)
        {
            if (length >= 3)
            {
                matches.Add(new Match(direction, row, col, length));
            }
        }

        public static BoardState RemoveMatches(BoardState currentState, List<Match> matches)
        {
            if (matches == null || matches.Count == 0)
                return currentState;

            // Шаг 1: Помечаем ячейки для удаления
            Element[,] markedCells = MarkCellsForRemoval(currentState.Board, matches);

            // Шаг 2: Применяем гравитацию
            Element[,] gravityAppliedCells = ApplyGravity(markedCells, currentState.Board.size);

            // Шаг 3: Подсчитываем очки
            int removedCount = matches.Sum(m => m.Length);
            int newScore = currentState.Score + CalculateScore(removedCount);

            return new BoardState(
                new Board(currentState.Board.size) { cells = gravityAppliedCells }, // Create new board with cells
                newScore
            );
        }

        private static Element[,] MarkCellsForRemoval(Board board, List<Match> matches)
        {
            Element[,] newCells = (Element[,])board.cells.Clone();

            foreach (var match in matches)
            {
                for (int i = 0; i < match.Length; i++)
                {
                    int row = match.Direction == MatchDirection.Horizontal ? match.Row : match.Row + i;
                    int col = match.Direction == MatchDirection.Horizontal ? match.Col + i : match.Col;

                    newCells[row, col] = new Element { Symbol = Element.EMPTY };
                }
            }

            return newCells;
        }

        private static Element[,] ApplyGravity(Element[,] cells, int size)
        {
            Element[,] newCells = new Element[size, size];

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    newCells[row, col] = new Element { Symbol = Element.EMPTY };
                }
            }

            for (int col = 0; col < size; col++)
            {
                int newRow = size - 1;
                for (int row = size - 1; row >= 0; row--)
                {
                    if (cells[row, col].Symbol != Element.EMPTY)
                    {
                        newCells[newRow, col] = cells[row, col];
                        newRow--;
                    }
                }
            }

            return newCells;
        }

        private static int CalculateScore(int removedCount)
        {
            return removedCount * 10;
        }

        public static BoardState FillEmptySpaces(BoardState currentState)
        {
            if (currentState.Board.cells == null)
                return currentState;

            Element[,] newCells = (Element[,])currentState.Board.cells.Clone();

            for (int row = 0; row < currentState.Board.size; row++)
            {
                for (int col = 0; col < currentState.Board.size; col++)
                {
                    if (newCells[row, col].Symbol == Element.EMPTY)
                    {
                        newCells[row, col] = new Element
                        {
                            Symbol = symbols[r.Next(symbols.Length)]
                        };
                    }
                }
            }

            return new BoardState(
                new Board(currentState.Board.size) { cells = newCells },
                currentState.Score
            );
        }
    }
}
