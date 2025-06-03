using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SnakeGame
{
    public partial class MainWindow : Window
    {
        private readonly SnakeGameViewModel viewModel;
        private User currentUser;

        public MainWindow(User? user = null)
        {
            InitializeComponent();

            viewModel = new SnakeGameViewModel();
            DataContext = viewModel;

            currentUser = user;

            // Показываем имя в заголовке и максимальный рекорд
            if (currentUser != null)
            {
                Title = $"Snake Game — {currentUser.Username}";
                MaxScoreTextBlock.Text = currentUser.MaxScore.ToString();
            }

            // Подписка на событие обновления счёта
            //viewModel.ScoreChanged += ViewModel_ScoreChanged;

        }

        private void ViewModel_ScoreChanged(object? sender, int newScore)
        {
            if (currentUser != null && newScore > currentUser.MaxScore)
            {
                currentUser.MaxScore = newScore;
                MaxScoreTextBlock.Text = newScore.ToString();

                // Здесь исправленный вызов метода
                DBHelper.UpdateMaxScore(currentUser.Id, newScore);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);
        }
    }
}