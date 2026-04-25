using System.Windows;

namespace MineSweeper.Views
{
    public partial class SettingsWindow : Window
    {
        public int Rows { get; private set; }
        public int Cols { get; private set; }
        public int Mines { get; private set; }

        public bool IsCountdown { get; private set; }
        public int CountdownSeconds { get; private set; }

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            // Difficulty
            if (BeginnerRadio.IsChecked == true)
            {
                Rows = 9; Cols = 9; Mines = 10;
            }
            else if (IntermediateRadio.IsChecked == true)
            {
                Rows = 16; Cols = 16; Mines = 40;
            }
            else if (ExpertRadio.IsChecked == true)
            {
                Rows = 30; Cols = 16; Mines = 99;
            }

            // Mode
            if (CountdownModeRadio.IsChecked == true)
            {
                IsCountdown = true;
                if (CountdownBeginnerRadio.IsChecked == true) CountdownSeconds = 300;
                else if (CountdownIntermediateRadio.IsChecked == true) CountdownSeconds = 180;
                else if (CountdownExpertRadio.IsChecked == true) CountdownSeconds = 60;
            }
            else
            {
                IsCountdown = false;
                CountdownSeconds = 0;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
