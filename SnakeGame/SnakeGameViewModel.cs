using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

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
        private ObservableCollection<Point> _snakeParts;

        // Команды
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand MoveLeftCommand { get; }
        public ICommand MoveRightCommand { get; }
        public ICommand Start { get; }
        public ICommand RestartCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ResumeCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SetSlowSpeedCommand { get; }
        public ICommand SetMediumSpeedCommand { get; }
        public ICommand SetFastSpeedCommand { get; }

        public event EventHandler<int>? ScoreChanged;
        public ObservableCollection<Point> SnakeParts => _snakeParts;
        public Point FoodPosition => model.FoodPosition;
        public int Score => model.Score;

        private bool _gameRunning = true;
        private bool _isGameStarted;
        private bool _isPaused;
        private Vector _nextDirection;
        private int _gameSpeed = 150;

        public Vector NextDirection
        {
            get => _nextDirection;
            set => _nextDirection = value;
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
            _snakeParts = new ObservableCollection<Point>();
            model.InitializeGame();

            NextDirection = model.SnakeDirection;

            MoveUpCommand = new RelayCommand(_ => ChangeDirection(Direction.Up));
            MoveDownCommand = new RelayCommand(_ => ChangeDirection(Direction.Down));
            MoveLeftCommand = new RelayCommand(_ => ChangeDirection(Direction.Left));
            MoveRightCommand = new RelayCommand(_ => ChangeDirection(Direction.Right));

            Start = new RelayCommand(_ =>
            {
                StartGame();
                CommandManager.InvalidateRequerySuggested();
            }, _ => !IsGameStarted);

            PauseCommand = new RelayCommand(_ => PauseGame(), _ => IsGameStarted && !IsPaused);
            ResumeCommand = new RelayCommand(_ => ResumeGame(), _ => IsGameStarted && IsPaused);
            RestartCommand = new RelayCommand(_ => RestartGame(), _ => IsGameStarted);
            ExitCommand = new RelayCommand(_ => ExitGame());

            SetSlowSpeedCommand = new RelayCommand(_ => SetGameSpeed(240), _ => !IsPaused);
            SetMediumSpeedCommand = new RelayCommand(_ => SetGameSpeed(190), _ => !IsPaused);
            SetFastSpeedCommand = new RelayCommand(_ => SetGameSpeed(80), _ => !IsPaused);

            gameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(_gameSpeed) };
            gameTimer.Tick += GameLoop;
        }

        private void SetGameSpeed(int speed)
        {
            _gameSpeed = speed;
            gameTimer.Interval = TimeSpan.FromMilliseconds(_gameSpeed);
            if (!IsPaused && IsGameStarted)
            {
                gameTimer.Stop();
                gameTimer.Start();
            }
        }

        private void ExitGame() => Application.Current.Shutdown();

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
            gameTimer.Interval = TimeSpan.FromMilliseconds(_gameSpeed);
            gameTimer.Start();

            OnPropertyChanged(nameof(SnakeParts));
            OnPropertyChanged(nameof(FoodPosition));
            OnPropertyChanged(nameof(Score));
            ScoreChanged?.Invoke(this, model.Score); // Добавим, чтобы сброс отобразился
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
            if (!GameRunning) return;

            model.SnakeDirection = NextDirection;
            model.MoveSnake();
            UpdateSnakeParts();

            if (model.CheckCollisions())
            {
                gameTimer.Stop();
                MessageBox.Show($"Game Over! Your score: {model.Score}");
                GameRunning = false;
                IsGameStarted = false;
                CommandManager.InvalidateRequerySuggested();
                return;
            }

            if (model.CheckFoodCollision())
            {
                OnPropertyChanged(nameof(Score));
                ScoreChanged?.Invoke(this, model.Score); // <--- Вызов события
            }


            OnPropertyChanged(nameof(SnakeParts));
            OnPropertyChanged(nameof(FoodPosition));
        }

        private void UpdateSnakeParts()
        {
            _snakeParts.Clear();
            foreach (var part in model.SnakeParts)
            {
                _snakeParts.Add(part);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
