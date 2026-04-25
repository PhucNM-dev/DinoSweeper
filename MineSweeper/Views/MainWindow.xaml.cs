using MineSweeper.Logic;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MineSweeper.Views
{
    public partial class MainWindow : Window
    {
        private MainLogic mainLogic;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            mainLogic = new MainLogic();
            mainLogic.OnBoardUpdated += RefreshAllButtons;
            mainLogic.OnHintChanged += UpdateHintUI;
            mainLogic.OnTimerChanged += (text) => TimerDisplay.Text = text;
            mainLogic.OnGameOver += ShowResultDialog;

            InitializeTimer();
            mainLogic.StartNewGame(9, 9, 10);
            BuildBoardUI();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => mainLogic.TickTimer();
            timer.Start();
        }

        private void BuildBoardUI()
        {
            GameBoard.Children.Clear();
            GameBoard.RowDefinitions.Clear();
            GameBoard.ColumnDefinitions.Clear();

            for (int r = 0; r < mainLogic.Board.Rows; r++)
                GameBoard.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            for (int c = 0; c < mainLogic.Board.Cols; c++)
                GameBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            for (int r = 0; r < mainLogic.Board.Rows; r++)
            {
                for (int c = 0; c < mainLogic.Board.Cols; c++)
                {
                    var cell = mainLogic.Board.Cells[r, c];
                    if (cell == null) continue;

                    Button btn = new Button
                    {
                        Tag = cell,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Padding = new Thickness(0),
                        Margin = new Thickness(1),
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7B3F00")),
                        IsEnabled = true,
                        IsHitTestVisible = true,
                        Content = CreateImage(Assets.Path("empty.png"))
                    };

                    btn.Click += Cell_Click;
                    btn.MouseRightButtonUp += Cell_RightClick;

                    Grid.SetRow(btn, r);
                    Grid.SetColumn(btn, c);
                    GameBoard.Children.Add(btn);
                }
            }

            MineCounter.Text = $"Mines: {mainLogic.Board.Mines}";
        }

        private Image CreateImage(string uri)
        {
            try
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(uri, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                return new Image { Source = bmp, Stretch = Stretch.Fill, IsHitTestVisible = false };
            }
            catch
            {
                return new Image { IsHitTestVisible = false };
            }
        }

        private TextBlock CreateNumberText(int number)
        {
            return new TextBlock
            {
                Text = number.ToString(),
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 18,
                Foreground = GetNumberBrush(number),
                IsHitTestVisible = false
            };
        }

        private Brush GetNumberBrush(int number)
        {
            switch (number)
            {
                case 1: return Brushes.Blue;
                case 2: return Brushes.Green;
                case 3: return Brushes.Red;
                case 4: return Brushes.DarkBlue;
                case 5: return Brushes.Brown;
                case 6: return Brushes.Teal;
                case 7: return Brushes.Black;
                case 8: return Brushes.Gray;
                default: return Brushes.Black;
            }
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn)) return;
            if (!(btn.Tag is Cell cell)) return;
            mainLogic.RevealCell(cell.Row, cell.Col);
        }

        private void Cell_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Button btn)) return;
            if (!(btn.Tag is Cell cell)) return;
            mainLogic.ToggleFlag(cell.Row, cell.Col);
        }

        private void Hint_Click(object sender, MouseButtonEventArgs e)
        {
            var hintCell = mainLogic.UseHint();
            if (hintCell != null)
            {
                var btn = GameBoard.Children.OfType<Button>().FirstOrDefault(b => b.Tag == hintCell);
                if (btn != null) HighlightHint(btn);
            }
        }

        private void HighlightHint(Button btn)
        {
            btn.Background = Brushes.Red;
            btn.BorderBrush = Brushes.Yellow;
            btn.BorderThickness = new Thickness(3);
        }

        private void RefreshAllButtons()
        {
            foreach (var child in GameBoard.Children.OfType<Button>())
            {
                if (!(child.Tag is Cell cell)) continue;

                if (cell.IsRevealed)
                {
                    child.IsEnabled = false;
                    child.IsHitTestVisible = false;
                    child.Background = new SolidColorBrush(Colors.LightGray);

                    if (cell.IsMine)
                        child.Content = CreateImage(Assets.Path("bomb.png"));
                    else if (cell.AdjacentMines > 0)
                        child.Content = CreateNumberText(cell.AdjacentMines);
                    else
                        child.Content = null;
                }
                else
                {
                    child.IsEnabled = !mainLogic.GameOver;
                    child.IsHitTestVisible = !mainLogic.GameOver;

                    if (cell.IsFlagged)
                        child.Content = CreateImage(Assets.Path("flag.png"));
                    else
                        child.Content = CreateImage(Assets.Path("empty.png"));

                    child.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7B3F00"));
                }
            }

            int flaggedCount = mainLogic.Board.Cells.Cast<Cell>().Count(c => c.IsFlagged);
            MineCounter.Text = $"Mines: {mainLogic.Board.Mines - flaggedCount}";
        }

        private void UpdateHintUI(int hintRemaining)
        {
            if (hintRemaining == int.MaxValue)
            {
                HintCounter.Text = "∞";
                HintImage.Source = new BitmapImage(new Uri(Assets.Path("lightbulb.png")));
            }
            else
            {
                HintCounter.Text = hintRemaining.ToString();
                HintImage.Source = new BitmapImage(new Uri(
                    hintRemaining == 0 ? Assets.Path("lightoff.png")
                                       : Assets.Path("lightbulb.png")));
            }
        }

        private void ShowResultDialog(bool won)
        {
            var win = new Window
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.ToolWindow,
                SizeToContent = SizeToContent.WidthAndHeight,
                Background = Brushes.Black,
                ShowInTaskbar = false
            };

            var root = new Grid { Margin = new Thickness(8) };
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var img = new Image
            {
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                MaxWidth = 640,
                MaxHeight = 480
            };

            string resourceUri = won ? Assets.Path("victory.jpg") : Assets.Path("lose.jpg");
            try
            {
                var bmp = new BitmapImage(new Uri(resourceUri, UriKind.Absolute));
                img.Source = bmp;
                Grid.SetRow(img, 0);
                root.Children.Add(img);
            }
            catch
            {
                var fallback = new TextBlock
                {
                    Text = won ? "You Win!" : "You Lose!",
                    FontSize = 24,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    Margin = new Thickness(20),
                    TextAlignment = TextAlignment.Center
                };
                Grid.SetRow(fallback, 0);
                root.Children.Add(fallback);
            }

            var btnPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 12, 0, 0)
            };

            var closeBtn = new Button { Content = "Close", Width = 100, Height = 32, Margin = new Thickness(6, 0, 6, 0) };
            closeBtn.Click += (s, e) => win.Close();

            var newGameBtn = new Button { Content = "New Game", Width = 100, Height = 32, Margin = new Thickness(6, 0, 6, 0) };
            newGameBtn.Click += (s, e) =>
            {
                win.Close();
                mainLogic.StartNewGame(mainLogic.Board.Rows, mainLogic.Board.Cols, mainLogic.Board.Mines);
                BuildBoardUI();
            };

            btnPanel.Children.Add(closeBtn);
            btnPanel.Children.Add(newGameBtn);

            Grid.SetRow(btnPanel, 1);
            root.Children.Add(btnPanel);

            win.Content = root;
            win.ShowDialog();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            mainLogic.StartNewGame(9, 9, 10);
            BuildBoardUI();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow();
            bool? result = settings.ShowDialog();
            if (result == true)
            {
                mainLogic.StartNewGame(settings.Rows, settings.Cols, settings.Mines, settings.IsCountdown, settings.CountdownSeconds);
                BuildBoardUI();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            Window aboutWin = new Window
            {
                Title = "About",
                Width = 500,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize,
                Background = Brushes.Black
            };

            Image img = new Image
            {
                Source = new BitmapImage(new Uri(Assets.Path("aboutus.png"))),
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            aboutWin.Content = img;
            aboutWin.ShowDialog();
        }
    }
}
