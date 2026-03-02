namespace FPgame
{
    public static class GameExtensions
    {
        public static BoardState Fill(this BoardState bs)
        {
            return Game.FillEmptySpaces(bs);
        }

        public static BoardState Cascade(this BoardState bs)
        {
            return Game.ProcessCascade(bs);
        }

        public static BoardState Draw(this BoardState bs)
        {
            Game.Draw(bs.Board);
            return bs;
        }

        public static BoardState Move(this BoardState bs)
        {
            return Game.ReadMove(bs);
        }
    }
}
