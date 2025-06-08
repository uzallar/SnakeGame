using System.Windows;
using System.Windows.Input;
namespace SnakeGame
{
    public class DifficultySelectionViewModel
    {
        public ICommand SetSlowCommand { get; }
        public ICommand SetMediumCommand { get; }
        public ICommand SetFastCommand { get; }

        public DifficultySelectionViewModel()
        {
            SetSlowCommand = new RelayCommand(_ => CloseWindow(SnakeSpeed.Slow));
            SetMediumCommand = new RelayCommand(_ => CloseWindow(SnakeSpeed.Medium));
            SetFastCommand = new RelayCommand(_ => CloseWindow(SnakeSpeed.Fast));
        }

        private void CloseWindow(SnakeSpeed speed)
        {
            Application.Current.Properties["SelectedSpeed"] = speed;
            foreach (Window window in Application.Current.Windows)
            {
                if (window is DifficultySelectionWindow difficultyWindow)
                {
                    difficultyWindow.DialogResult = true;
                    break;
                }
            }
        }
    }
}