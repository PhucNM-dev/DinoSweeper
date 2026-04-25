using System;
using System.Collections.Generic;
using System.Linq;

namespace MineSweeper.Logic
{
    public class MainLogic
    {
        public Board Board { get; private set; }
        public bool GameOver { get; private set; }
        public bool TimerRunning { get; private set; }
        public DateTime StartTime { get; private set; }
        public bool IsCountdown { get; private set; }
        public int CountdownRemaining { get; private set; }
        public int HintRemaining { get; private set; }
        public Cell CurrentHintCell { get; private set; }

        // Events for UI to subscribe
        public event Action<int> OnHintChanged;
        public event Action<string> OnTimerChanged;
        public event Action<bool> OnGameOver;
        public event Action OnBoardUpdated;

        public void StartNewGame(int rows, int cols, int mines, bool countdownMode = false, int countdownSeconds = 0)
        {
            GameOver = false;
            TimerRunning = false;
            IsCountdown = countdownMode;
            CountdownRemaining = countdownSeconds;
            CurrentHintCell = null;

            // Set number of hints based on difficulty
            if (rows == 9 && cols == 9 && mines == 10) // Beginner
                HintRemaining = int.MaxValue;
            else if (rows == 16 && cols == 16 && mines == 40) // Intermediate
                HintRemaining = 3;
            else if (rows == 30 && cols == 16 && mines == 99) // Expert
                HintRemaining = 0;
            else
                HintRemaining = 0;

            OnHintChanged?.Invoke(HintRemaining);
            OnTimerChanged?.Invoke(IsCountdown ? $"Time: {CountdownRemaining}s" : "Time: 00:00");

            Board = new Board(rows, cols, mines);
            OnBoardUpdated?.Invoke();
        }

        public void StartTimerIfNeeded()
        {
            if (!TimerRunning)
            {
                TimerRunning = true;
                StartTime = DateTime.Now;
            }
        }

        public void TickTimer()
        {
            if (!TimerRunning) return;

            if (IsCountdown)
            {
                CountdownRemaining--;
                OnTimerChanged?.Invoke($"Time: {CountdownRemaining}s");
                if (CountdownRemaining <= 0)
                {
                    HandleGameOver(false);
                }
            }
            else
            {
                var elapsed = DateTime.Now - StartTime;
                OnTimerChanged?.Invoke($"Time: {elapsed:mm\\:ss}");
            }
        }

        public void RevealCell(int row, int col)
        {
            if (GameOver) return;
            var cell = Board.Cells[row, col];
            if (cell.IsRevealed || cell.IsFlagged) return;

            StartTimerIfNeeded();

            try
            {
                Board.Reveal(row, col);
            }
            catch
            {
                HandleGameOver(false);
                return;
            }

            OnBoardUpdated?.Invoke();

            if (Board.CheckWin())
            {
                HandleGameOver(true);
            }
        }

        public void ToggleFlag(int row, int col)
        {
            if (GameOver) return;
            var cell = Board.Cells[row, col];
            if (cell.IsRevealed) return;

            StartTimerIfNeeded();
            cell.IsFlagged = !cell.IsFlagged;

            // Reset hint if the flagged cell was the current hint
            if (cell == CurrentHintCell && cell.IsFlagged)
                CurrentHintCell = null;

            OnBoardUpdated?.Invoke();

            if (Board.CheckWin())
            {
                HandleGameOver(true);
            }
        }

        public Cell UseHint()
        {
            if (Board == null || GameOver) return null;
            if (HintRemaining <= 0) return null;

            if (CurrentHintCell != null) return CurrentHintCell;

            var candidates = Board.Cells.Cast<Cell>()
                .Where(c => c.IsMine && !c.IsRevealed && !c.IsFlagged)
                .Where(c => GetNeighbors(c.Row, c.Col).Any(n => n.IsRevealed && n.AdjacentMines > 0))
                .ToList();

            if (!candidates.Any()) return null;

            var rnd = new Random();
            CurrentHintCell = candidates[rnd.Next(candidates.Count)];

            if (HintRemaining != int.MaxValue)
            {
                HintRemaining--;
                OnHintChanged?.Invoke(HintRemaining);
            }

            return CurrentHintCell;
        }

        // Helper: get neighboring cells
        private IEnumerable<Cell> GetNeighbors(int row, int col)
        {
            int[] dr = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dc = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int i = 0; i < 8; i++)
            {
                int nr = row + dr[i];
                int nc = col + dc[i];
                if (nr >= 0 && nr < Board.Rows && nc >= 0 && nc < Board.Cols)
                    yield return Board.Cells[nr, nc];
            }
        }

        private void HandleGameOver(bool won)
        {
            GameOver = true;
            TimerRunning = false;

            if (!won && Board != null)
            {
                foreach (var c in Board.Cells)
                {
                    if (c.IsMine) c.IsRevealed = true;
                }
            }

            OnBoardUpdated?.Invoke();
            OnGameOver?.Invoke(won);
        }
    }
}
