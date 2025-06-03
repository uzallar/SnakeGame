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
        private SnakeGameModel model;
        private DispatcherTimer gameTimer;

        // Команды управления
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand MoveLeftCommand { get; }
        public ICommand MoveRightCommand { get; }
        public ICommand Start { get; }
        public ICommand RestartCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ResumeCommand { get; }
        public ICommand ExitCommand { get; }

        // Команды для скорости
        public ICommand SetSlowSpeedCommand { get; }
        public ICommand SetMediumSpeedCommand { get; }
        public ICommand SetFastSpeedCommand { get; }

        public ObservableCollection<Point> SnakeParts => new(model.SnakeParts);
        public Point FoodPosition => model.FoodPosition;
        public int Score => model.Score;

        private bool _gameRunning = true;
        private bool _isGameStarted;
        private bool _isPaused;
        private Vector _nextDirection;
        private int _gameSpeed = 150; // Средняя скорость по умолчанию

        public Vector NextDirection
        {
            get { return _nextDirection; }
            set { _nextDirection = value; }
        }

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                _isPaused = value;
                OnPropertyChanged(nameof(IsPaused));
            }
        }

        public bool IsGameStarted
        {
            get => _isGameStarted;
            set
            {
                if (_isGameStarted != value)
                {
                    _isGameStarted = value;
                    OnPropertyChanged(nameof(IsGameStarted));
                }
            }
        }

        public bool GameRunning
        {
            get => _gameRunning;
            set => _gameRunning = value;
        }

        public SnakeGameViewModel()
        {
            model = new SnakeGameModel();
            model.InitializeGame();

            NextDirection = model.SnakeDirection;

            // Инициализация команд управления
            MoveUpCommand = new RelayCommand(_ => ChangeDirection(Direction.Up));
            MoveDownCommand = new RelayCommand(_ => ChangeDirection(Direction.Down));
            MoveLeftCommand = new RelayCommand(_ => ChangeDirection(Direction.Left));
            MoveRightCommand = new RelayCommand(_ => ChangeDirection(Direction.Right));
            Start = new RelayCommand(_ => StartGame());
            PauseCommand = new RelayCommand(_ => PauseGame(), _ => IsGameStarted && !IsPaused);
            ResumeCommand = new RelayCommand(_ => ResumeGame(), _ => IsGameStarted && IsPaused);
            RestartCommand = new RelayCommand(_ => RestartGame(), _ => IsGameStarted);
            ExitCommand = new RelayCommand(_ => ExitGame());

            // Инициализация команд скорости
            // Быстро
            // В конструкторе ViewModel измените команды скорости:
            SetSlowSpeedCommand = new RelayCommand(_ => SetGameSpeed(250), _ => !IsPaused);
            SetMediumSpeedCommand = new RelayCommand(_ => SetGameSpeed(150), _ => !IsPaused);
            SetFastSpeedCommand = new RelayCommand(_ => SetGameSpeed(70), _ => !IsPaused);

            gameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(_gameSpeed) };
            gameTimer.Tick += GameLoop;
        }
        private void SetGameSpeed(int speed)
        {
            _gameSpeed = speed;
            if (gameTimer != null)
            {
                gameTimer.Interval = TimeSpan.FromMilliseconds(_gameSpeed);

                // Перезапускаем таймер только если игра активна
                if (IsGameStarted && !IsPaused)
                {
                    gameTimer.Stop();
                    gameTimer.Start();
                }
            }
        }

        private void ExitGame()
        {
            Application.Current.Shutdown();
        }
        private void ResumeGame()
        {
            if (IsPaused)
            {
                IsPaused = false;
                gameTimer.Start();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void PauseGame()
        {
            if (!IsGameStarted || !GameRunning) return;
            IsPaused = !IsPaused;
            if (IsPaused)
                gameTimer.Stop();
            else
                gameTimer.Start();

            // Обновляем доступность команд
            CommandManager.InvalidateRequerySuggested();
        }
        private void RestartGame()
        {
            StartGame();
            if (IsPaused)
            {
                IsPaused = false;
                gameTimer.Start();
            }
            CommandManager.InvalidateRequerySuggested();
        }
        private void StartGame()
        {
            model.InitializeGame();
            NextDirection = model.SnakeDirection;
            UpdateSnakeParts();
            IsGameStarted = true;
            GameRunning = true;
            gameTimer.Interval = TimeSpan.FromMilliseconds(_gameSpeed); // Устанавливаем текущую скорость
            gameTimer.Start();
            OnPropertyChanged(nameof(SnakeParts));
            OnPropertyChanged(nameof(FoodPosition));
            OnPropertyChanged(nameof(Score));
        }

        private void ChangeDirection(Direction direction)
        {
            if (!IsPaused)
            {
                Vector newDir = direction switch
                {
                    Direction.Up => new Vector(0, -SnakeGameModel.SnakeSize),
                    Direction.Down => new Vector(0, SnakeGameModel.SnakeSize),
                    Direction.Left => new Vector(-SnakeGameModel.SnakeSize, 0),
                    Direction.Right => new Vector(SnakeGameModel.SnakeSize, 0),
                    _ => model.SnakeDirection
                };

                if (newDir + model.SnakeDirection != new Vector(0, 0))
                {
                    NextDirection = newDir;
                }
            }
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (!GameRunning)
                return;
            model.SnakeDirection = NextDirection;
            model.MoveSnake();
            UpdateSnakeParts();

            if (model.CheckCollisions())
            {
                MessageBox.Show($"Game Over! Your score: {model.Score}");
                GameRunning = false;
                StartGame();
                return;
            }

            if (model.CheckFoodCollision())
            {
                OnPropertyChanged(nameof(Score));
            }

            OnPropertyChanged(nameof(SnakeParts));
            OnPropertyChanged(nameof(FoodPosition));
        }

        private void UpdateSnakeParts()
        {
            SnakeParts.Clear();
            foreach (var part in model.SnakeParts)
            {
                SnakeParts.Add(part);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}