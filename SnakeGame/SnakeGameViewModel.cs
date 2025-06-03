using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

namespace SnakeGame
{
    public class SnakeGameViewModel : INotifyPropertyChanged
    {
        private readonly SnakeGameModel model;
        private readonly DispatcherTimer gameTimer;

        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand MoveLeftCommand { get; }
        public ICommand MoveRightCommand { get; }

        public ObservableCollection<Point> SnakeParts => new(model.SnakeParts);
        public Point FoodPosition => model.FoodPosition;
        public int Score => model.Score;

        // Событие для уведомления об изменении счёта
        public event EventHandler<int>? ScoreChanged;

        public SnakeGameViewModel()
        {
            model = new SnakeGameModel();
            model.InitializeGame();

            MoveUpCommand = new RelayCommand(_ => ChangeDirection(Direction.Up));
            MoveDownCommand = new RelayCommand(_ => ChangeDirection(Direction.Down));
            MoveLeftCommand = new RelayCommand(_ => ChangeDirection(Direction.Left));
            MoveRightCommand = new RelayCommand(_ => ChangeDirection(Direction.Right));

            gameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        private void ChangeDirection(Direction newDirection)
        {
            var current = model.SnakeDirection;
            switch (newDirection)
            {
                case Direction.Up:
                    if (current.Y == 0) model.SnakeDirection = new Vector(0, -SnakeGameModel.SnakeSize);
                    break;
                case Direction.Down:
                    if (current.Y == 0) model.SnakeDirection = new Vector(0, SnakeGameModel.SnakeSize);
                    break;
                case Direction.Left:
                    if (current.X == 0) model.SnakeDirection = new Vector(-SnakeGameModel.SnakeSize, 0);
                    break;
                case Direction.Right:
                    if (current.X == 0) model.SnakeDirection = new Vector(SnakeGameModel.SnakeSize, 0);
                    break;
            }
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (!model.IsGameRunning || model.IsGameOver)
                return;

            model.MoveSnake();

            if (model.CheckCollisions())
            {
                model.IsGameOver = true;
                model.IsGameRunning = false;
                MessageBox.Show($"Game Over! Your score: {model.Score}");
                return;
            }

            bool ateFood = model.CheckFoodCollision();

            if (ateFood)
            {
                OnPropertyChanged(nameof(Score));
                ScoreChanged?.Invoke(this, model.Score); // Уведомляем MainWindow
            }

            OnPropertyChanged(nameof(SnakeParts));
            OnPropertyChanged(nameof(FoodPosition));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
