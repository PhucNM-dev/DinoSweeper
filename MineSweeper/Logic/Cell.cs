namespace MineSweeper.Logic
{
    public class Cell
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public bool IsMine { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsFlagged { get; set; }
        public int AdjacentMines { get; set; }

        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
            IsMine = false;
            IsRevealed = false;
            IsFlagged = false;
            AdjacentMines = 0;
        }
    }
}
