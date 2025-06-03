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
            viewModel.ScoreChanged += ViewModel_ScoreChanged;

            // Запуск рендера игры
            CompositionTarget.Rendering += RenderGame;
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

        private void RenderGame(object sender, System.EventArgs e)
        {
            GameCanvas.Children.Clear();

            foreach (var part in viewModel.SnakeParts)
            {
                var rect = new Rectangle
                {
                    Width = SnakeGameModel.SnakeSize,
                    Height = SnakeGameModel.SnakeSize,
                    Fill = Brushes.Green
                };
                Canvas.SetLeft(rect, part.X);
                Canvas.SetTop(rect, part.Y);
                GameCanvas.Children.Add(rect);
            }

            var foodRect = new Rectangle
            {
                Width = SnakeGameModel.SnakeSize,
                Height = SnakeGameModel.SnakeSize,
                Fill = Brushes.Red
            };
            Canvas.SetLeft(foodRect, viewModel.FoodPosition.X);
            Canvas.SetTop(foodRect, viewModel.FoodPosition.Y);
            GameCanvas.Children.Add(foodRect);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);
        }
    }
}