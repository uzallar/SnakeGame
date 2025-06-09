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
        private readonly User _currentUser;
        private SnakeGameModel model;
        private DispatcherTimer gameTimer;
        private ObservableCollection<Point> _snakeParts;
        public string WindowTitle => $"Snake Game — {_currentUser?.Username ?? "Guest"}";
        public string MaxScoreText => _currentUser?.MaxScore.ToString() ?? "0";

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
        public ICommand ShowLeaderboardCommand { get; }
        public ICommand ChangeUserCommand { get; }
        public ICommand ChangeDifficultyCommand { get; }


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

        public SnakeGameViewModel(User user = null, SnakeSpeed? selectedSpeed = null)
        {
            ChangeDifficultyCommand = new RelayCommand(_ => ChangeDifficulty());
            model = new SnakeGameModel();
            _snakeParts = new ObservableCollection<Point>();
            _currentUser = user;

            // Устанавливаем скорость в зависимости от выбранной сложности
            var speed = selectedSpeed ?? SnakeSpeed.Medium;

            switch (speed)
            {
                case SnakeSpeed.Slow:
                    _gameSpeed = 240;
                    model.CurrentSpeed = SnakeSpeed.Slow;
                    break;
                case SnakeSpeed.Medium:
                    _gameSpeed = 150;
                    model.CurrentSpeed = SnakeSpeed.Medium;
                    break;
                case SnakeSpeed.Fast:
                    _gameSpeed = 80;
                    model.CurrentSpeed = SnakeSpeed.Fast;
                    break;
            }


            if (_currentUser != null)
            {
                ScoreChanged += OnScoreChanged;
            }

            ShowLeaderboardCommand = new RelayCommand(_ => ShowLeaderboard());
            ChangeUserCommand = new RelayCommand(_ => ChangeUser());

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

            SetSlowSpeedCommand = new RelayCommand(_ => SetGameSpeed(240, SnakeSpeed.Slow), _ => !IsPaused);
            SetMediumSpeedCommand = new RelayCommand(_ => SetGameSpeed(190, SnakeSpeed.Medium), _ => !IsPaused);
            SetFastSpeedCommand = new RelayCommand(_ => SetGameSpeed(80, SnakeSpeed.Fast), _ => !IsPaused);

            gameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(_gameSpeed) };
            gameTimer.Tick += GameLoop;
        }
        private void OnScoreChanged(object sender, int newScore)
        {
            if (_currentUser != null && newScore > _currentUser.MaxScore)
            {
                _currentUser.MaxScore = newScore;
                OnPropertyChanged(nameof(MaxScoreText));
                DBHelper.UpdateMaxScore(_currentUser.Id, newScore);
            }
        }
        private void ShowLeaderboard()
        {
            var leaderboardWindow = new LeaderboardWindow();
            leaderboardWindow.ShowDialog();
        }

        private void ChangeUser()
        {
            var result = MessageBox.Show("Вы действительно хотите сменить пользователя?",
                                       "Подтверждение",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Logout();
            }
        }

        private void Logout()
        {
            var startWindow = new StartWindow();
            startWindow.Show();

            Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Close();
        }

        private void SetGameSpeed(int speed, SnakeSpeed speedLevel)
        {
            _gameSpeed = speed;
            model.CurrentSpeed = speedLevel;
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
            ScoreChanged?.Invoke(this, model.Score);
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
                ScoreChanged?.Invoke(this, model.Score);
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
        private void ChangeDifficulty()
        {
            // Сохраняем текущее состояние паузы
            bool wasPaused = IsPaused;

            // Временно выходим из паузы
            IsPaused = false;

            // Даем время на обновление UI
            Application.Current.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);

            // Показываем окно выбора сложности
            var difficultyWindow = new DifficultySelectionWindow();
            if (difficultyWindow.ShowDialog() == true)
            {
                // Даем время на закрытие окна
                Application.Current.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);

                // Теперь показываем MessageBox
                var result = MessageBox.Show(
                    "При смене сложности текущая игра будет сброшена. Продолжить?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var selectedSpeed = (SnakeSpeed)Application.Current.Properties["SelectedSpeed"];
                    switch (selectedSpeed)
                    {
                        case SnakeSpeed.Slow:
                            SetGameSpeed(240, SnakeSpeed.Slow);
                            break;
                        case SnakeSpeed.Medium:
                            SetGameSpeed(150, SnakeSpeed.Medium);
                            break;
                        case SnakeSpeed.Fast:
                            SetGameSpeed(80, SnakeSpeed.Fast);
                            break;
                    }
                    RestartGame();
                }
                else
                {
                    // Восстанавливаем паузу
                    IsPaused = wasPaused;
                }
            }
            else
            {
                // Восстанавливаем паузу
                IsPaused = wasPaused;
            }
        }
    }
}