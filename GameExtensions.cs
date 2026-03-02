namespace FPgame
{
    public static class GameExtensions
    {
        // Обертка для FillEmptySpaces, чтобы вызывать как метод расширения
        public static BoardState Fill(this BoardState bs)
        {
            return Game.FillEmptySpaces(bs);
        }

        // Обертка для ProcessCascade
        public static BoardState Cascade(this BoardState bs)
        {
            return Game.ProcessCascade(bs);
        }

        // Обертка для Draw (сквозной метод)
        public static BoardState Draw(this BoardState bs)
        {
            Game.Draw(bs.Board);
            return bs;
        }

        // Обертка для ReadMove
        public static BoardState Move(this BoardState bs)
        {
            return Game.ReadMove(bs);
        }
    }
}
