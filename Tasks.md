Чистая игровая логика
Прекрасная сильная математическая сторона функционального стиля проявляется ещё и в том, что мы повсеместно используем рекурсию.

За счёт рекурсии нам не нужно лепить дополнительные циклы "до тех пор пока...": мы просто последовательно описываем основные логические шаги -- и всё.

Мы уже получили некоторое представление о том, как должен работать основной алгоритм, и по большому счёту остаётся лишь обернуть это представление в рекурсию.

1. Находим комбинации (пресловутые три-в-ряд и любые другие заданные).
2. Если не найдено ни одной, завершаем работу с текущим состоянием (полученным как параметр).
3. Удаляем комбинации, считаем статистику/бонусы.
4. Заполняем пустые клетки сдвигом сверху вниз и набрасыванием в самый первый ряд.
5. Возвращаем рекурсивный вызов с этим локальным состоянием (которое иммутабельно).

ProcessCascade:

FindMatches
RemoveMatches
FillEmptySpaces
ProcessCascade

FindMatches
Очень хорош функциональный стиль ещё и тем, что все функции у нас чистые, а AI очень силён в небольших алгоритмических задачах без внешних зависимостей -- на уровне лучших программистов мира, поэтому его удобно привлекать для реализации функций.

Структура, описывающая комбинацию:


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

        public static List<Match> FindMatches(Board board)
        {
            var matches = new List<Match>();

            // Горизонтальные комбинации
            for (int row = 0; row < board.size; row++)
            {
                int startCol = 0;
                for (int col = 1; col < board.size; col++)
                {
                    // Пропускаем пустые ячейки в начале строки
                    if (board.cells[row, startCol].Symbol == Element.EMPTY)
                    {
                        startCol = col;
                        continue;
                    }

                    // Если текущая ячейка пустая, обрываем текущую последовательность
                    if (board.cells[row, col].Symbol == Element.EMPTY)
                    {
                        AddMatchIfValid(matches, row, startCol, col - startCol, MatchDirection.Horizontal);
                        startCol = col + 1;
                        continue;
                    }

                    // Проверяем совпадение символов для непустых ячеек
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
                    // Пропускаем пустые ячейки в начале столбца
                    if (board.cells[startRow, col].Symbol == Element.EMPTY)
                    {
                        startRow = row;
                        continue;
                    }

                    // Если текущая ячейка пустая, обрываем текущую последовательность
                    if (board.cells[row, col].Symbol == Element.EMPTY)
                    {
                        AddMatchIfValid(matches, startRow, col, row - startRow, MatchDirection.Vertical);
                        startRow = row + 1;
                        continue;
                    }

                    // Проверяем совпадение символов для непустых ячеек
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

        private static void AddMatchIfValid(List<Match> matches, int row, int col, 
                int length, MatchDirection direction)
        {
            // Учитываем только комбинации из 3 и более элементов (ТЗ)
            if (length >= 3)
            {
                matches.Add(new Match(direction, row, col, length));
            }
        }


RemoveMatches

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

            // Возвращаем НОВОЕ состояние
            return new BoardState(
                new Board { size = currentState.Board.size, cells = gravityAppliedCells },
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
            // Базовая система подсчета очков: 10 за каждый элемент
            return removedCount * 10;
        }


FillEmptySpaces

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
                new Board { size = currentState.Board.size, cells = newCells },
                currentState.Score
            );
        }


