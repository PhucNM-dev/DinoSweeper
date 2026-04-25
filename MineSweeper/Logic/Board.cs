using System;
using System.Collections.Generic;

namespace MineSweeper.Logic
{
    public class Board
    {
        public int Rows { get; }
        public int Cols { get; }
        public int Mines { get; }
        public Cell[,] Cells { get; }
        private bool minesPlaced = false;
        private Random rnd = new Random();

        public Board(int rows, int cols, int mines)
        {
            if (rows <= 0 || cols <= 0) throw new ArgumentException("Invalid board size");
            if (mines < 0 || mines >= rows * cols) throw new ArgumentException("Invalid mine count");

            Rows = rows;
            Cols = cols;
            Mines = mines;
            Cells = new Cell[Rows, Cols];

            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    Cells[r, c] = new Cell(r, c);

            minesPlaced = false;
        }

        public void PlaceMinesDeferred(int safeRow, int safeCol)
        {
            var allowed = new List<(int r, int c)>();
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    bool isSafeZone = Math.Abs(r - safeRow) <= 1 && Math.Abs(c - safeCol) <= 1;
                    if (!isSafeZone) allowed.Add((r, c));
                }
            }

            for (int i = 0; i < Mines && allowed.Count > 0; i++)
            {
                int idx = rnd.Next(allowed.Count);
                var pos = allowed[idx];
                allowed.RemoveAt(idx);
                Cells[pos.r, pos.c].IsMine = true;
            }

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Cells[r, c].IsMine)
                    {
                        Cells[r, c].AdjacentMines = -1;
                        continue;
                    }

                    int count = 0;
                    for (int dr = -1; dr <= 1; dr++)
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            if (dr == 0 && dc == 0) continue;
                            int nr = r + dr, nc = c + dc;
                            if (nr >= 0 && nr < Rows && nc >= 0 && nc < Cols)
                                if (Cells[nr, nc].IsMine) count++;
                        }

                    Cells[r, c].AdjacentMines = count;
                }
            }

            minesPlaced = true;
        }

        public void Reveal(int r, int c)
        {
            if (r < 0 || r >= Rows || c < 0 || c >= Cols) return;

            if (!minesPlaced) PlaceMinesDeferred(r, c);

            var start = Cells[r, c];
            if (start == null) return;

            if (start.IsMine)
            {
                start.IsRevealed = true;
                throw new Exception("HitMine");
            }

            var q = new Queue<Cell>();
            q.Enqueue(start);

            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                if (cur == null) continue;
                if (cur.IsRevealed || cur.IsFlagged) continue;

                cur.IsRevealed = true;

                if (cur.AdjacentMines == 0)
                {
                    for (int dr = -1; dr <= 1; dr++)
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            int nr = cur.Row + dr, nc = cur.Col + dc;
                            if (nr >= 0 && nr < Rows && nc >= 0 && nc < Cols)
                            {
                                var neighbor = Cells[nr, nc];
                                if (neighbor != null && !neighbor.IsRevealed && !neighbor.IsMine && !neighbor.IsFlagged)
                                    q.Enqueue(neighbor);
                            }
                        }
                }
            }
        }

        public bool CheckWin()
        {
            foreach (var c in Cells)
            {
                if (!c.IsMine && !c.IsRevealed) return false;
            }
            return true;
        }
    }
}
