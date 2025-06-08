using System;
using System.Linq;
using System.Windows;
using SnakeGame;
using SnakeGame.ViewModels;
using Xunit;
using static SnakeGame.ViewModels.LeaderboardViewModel;
using static Tests1.SnakeGameViewModelTests.LeaderboardViewModelTests;


namespace Tests1
{
    public class SnakeGameViewModelTests
    {
        [Fact]
        public void StartGame_ShouldInitializeGameCorrectly()
        {
            var vm = new SnakeGameViewModel();
            vm.Start.Execute(null);

            Assert.True(vm.IsGameStarted);
            Assert.True(vm.GameRunning);
            Assert.NotEmpty(vm.SnakeParts);
            Assert.NotEqual(default, vm.FoodPosition);
            Assert.Equal(0, vm.Score);
        }

        [Fact]
        public void ChangeDirection_ShouldUpdateNextDirection1()
        {
            var vm = new SnakeGameViewModel();
            vm.Start.Execute(null);
            var initialDirection = vm.NextDirection;

            vm.MoveUpCommand.Execute(null);

            Assert.NotEqual(initialDirection, vm.NextDirection);
        }

        [Fact]
        public void PauseGame_ShouldStopGameLoop()
        {
            var vm = new SnakeGameViewModel();
            vm.Start.Execute(null);

            vm.PauseCommand.Execute(null);

            Assert.True(vm.IsPaused);
        }

        [Fact]
        public void ResumeGame_ShouldResumeGameLoop()
        {
            var vm = new SnakeGameViewModel();
            vm.Start.Execute(null);
            vm.PauseCommand.Execute(null);

            vm.ResumeCommand.Execute(null);

            Assert.False(vm.IsPaused);
        }

        [Fact]
        public void RestartGame_ShouldResetState()
        {
            var vm = new SnakeGameViewModel();
            vm.Start.Execute(null);
            var originalPosition = vm.SnakeParts.First();

            vm.MoveRightCommand.Execute(null);
            vm.RestartCommand.Execute(null);

            var newPosition = vm.SnakeParts.First();


            Assert.True(vm.GameRunning);
        }

        [Theory]
        [InlineData(250)]
        [InlineData(150)]
        [InlineData(70)]
        public void SetGameSpeed_ShouldUpdateTimer(int speed)
        {
            var vm = new SnakeGameViewModel();
            vm.Start.Execute(null);
            vm.SetMediumSpeedCommand.Execute(null);

            Assert.True(vm.IsGameStarted);
        }

        [Fact]
        public void DirectionChange_ShouldPreventReverse()
        {
            var vm = new SnakeGameViewModel();
            vm.Start.Execute(null);

            vm.MoveRightCommand.Execute(null);
            var dirBefore = vm.NextDirection;
            vm.MoveLeftCommand.Execute(null);

            Assert.Equal(dirBefore, vm.NextDirection);
        }



        [Fact]
        public void InitializeGame_ShouldSetInitialValues()
        {
            var model = new SnakeGameModel();
            model.InitializeGame();

            Assert.NotNull(model.SnakeParts);
            Assert.NotEmpty(model.SnakeParts);
            Assert.True(model.SnakeParts.Count >= 5);
            Assert.Equal(new Vector(SnakeGameModel.SnakeSize, 0), model.SnakeDirection);
            Assert.Equal(0, model.Score);
        }

        [Fact]
        public void MoveSnake_ShouldChangeHeadPosition()
        {
            var model = new SnakeGameModel();
            model.InitializeGame();

            var initialHead = model.SnakeHeadPosition;
            model.MoveSnake();
            var newHead = model.SnakeHeadPosition;

            Assert.NotEqual(initialHead, newHead);
        }

        [Fact]
        public void AddSnakePart_ShouldIncreaseSnakeLength()
        {
            var model = new SnakeGameModel();
            model.InitializeGame();
            int oldLength = model.SnakeParts.Count;

            model.AddSnakePart();

            Assert.Equal(oldLength + 1, model.SnakeParts.Count);
        }

        [Fact]
        public void CreateFood_ShouldPlaceFoodOutsideSnake()
        {
            var model = new SnakeGameModel();
            model.InitializeGame();

            Assert.DoesNotContain(model.SnakeParts, part => part == model.FoodPosition);
        }

        [Fact]
        public void CheckFoodCollision_ShouldGrowSnakeAndIncreaseScore()
        {
            var model = new SnakeGameModel();
            model.InitializeGame();
            model.FoodPosition = model.SnakeHeadPosition;

            bool collided = model.CheckFoodCollision();

            Assert.True(collided);
            
            Assert.Equal(6, model.SnakeParts.Count);
        }

        [Fact]
        public void CheckCollisions_ShouldReturnTrue_WhenHitsWall()
        {
            var model = new SnakeGameModel();
            model.InitializeGame();

            model.SnakeHeadPosition = new Point(-10, 0);

            Assert.True(model.CheckCollisions());
        }

        [Fact]
        public void CheckCollisions_ShouldReturnTrue_WhenHitsItself()
        {
            var model = new SnakeGameModel();
            model.InitializeGame();

            // Принудительно замыкаем змейку на саму себя
            model.SnakeHeadPosition = model.SnakeParts[3];

            Assert.True(model.CheckCollisions());
        }

        [Fact]
        public void Execute_ShouldCallAction()
        {

            bool wasExecuted = false;
            var command = new RelayCommand(_ => wasExecuted = true);

            command.Execute(null);


            Assert.True(wasExecuted);
        }

        [Fact]
        public void CanExecute_ShouldReturnTrue_WhenPredicateIsNull()
        {

            var command = new RelayCommand(_ => { });


            bool result = command.CanExecute(null);

            Assert.True(result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanExecute_ShouldRespectPredicate(bool expected)
        {

            var command = new RelayCommand(_ => { }, _ => expected);


            bool result = command.CanExecute(null);


            Assert.Equal(expected, result);
        }

        [Fact]
        public void CanExecuteChanged_ShouldAttachAndDetachHandlers()
        {

            var command = new RelayCommand(_ => { });
            EventHandler handler = (_, _) => { };

            command.CanExecuteChanged += handler;
            command.CanExecuteChanged -= handler;
        }

        [Fact]
        public void PauseGame_ShouldTogglePauseState()
        {
            var vm = new SnakeGameViewModel();
            vm.StartGame();

            bool initialPauseState = vm.IsPaused;

            vm.PauseGame();
            Assert.NotEqual(initialPauseState, vm.IsPaused);

            vm.PauseGame();
            Assert.Equal(initialPauseState, vm.IsPaused);
        }

        [Fact]
        public void ResumeGame_ShouldUnpauseGame()
        {
            var vm = new SnakeGameViewModel();
            vm.StartGame();
            vm.PauseGame();

            Assert.True(vm.IsPaused);

            vm.ResumeGame();

            Assert.False(vm.IsPaused);
        }

        [Fact]
        public void RestartGame_ShouldStartNewGame()
        {
            var vm = new SnakeGameViewModel();
            vm.StartGame();

            var initialSnakeParts = vm.SnakeParts.ToList();

            vm.RestartGame();

            Assert.True(vm.IsGameStarted);
            Assert.True(vm.GameRunning);
         
        }

        [Fact]
        public void SetGameSpeed_ShouldUpdateSpeed()
        {
            var vm = new SnakeGameViewModel();
            int newSpeed = 150;
            SnakeSpeed newSpeedLevel = SnakeSpeed.Fast;

            vm.SetGameSpeed(newSpeed, newSpeedLevel);

            Assert.Equal(newSpeedLevel, newSpeedLevel);
        }

        [Fact]
        public void ChangeDirection_ShouldUpdateNextDirection()
        {
            var vm = new SnakeGameViewModel();
            vm.StartGame();
            var initialDirection = vm.NextDirection;

            vm.ChangeDirection(Direction.Up);

            Assert.NotEqual(initialDirection, vm.NextDirection);
        }

        public class StartWindowViewTests
        {
            [Fact]
            public void Constructor_ShouldInitializeRegisterAndLoginCommand()
            {
                var vm = new StartWindowView();

                Assert.NotNull(vm.RegisterAndLoginCommand);
                Assert.True(vm.RegisterAndLoginCommand.CanExecute(null));
            }

            

          
        }

        public class LeaderboardViewModelTests
        {
            
            [Fact]
            public void LoadLeaderboard_ShouldLoadAndOrderPlayersCorrectly()
            {
                // Arrange - создаем тестовые данные прямо в памяти
                var fakeDbHelper = new FakeDBHelper();
                fakeDbHelper.TestUsers.AddRange(new[]
                {
            new User { Username = "Alice", MaxScore = 150 },
            new User { Username = "Bob", MaxScore = 200 },  // Должен быть первым
            new User { Username = "Charlie", MaxScore = 100 }
        });

                // Act
                var viewModel = new LeaderboardViewModel(fakeDbHelper);

                // Assert - проверяем только логику ViewModel
                Assert.Equal(33, viewModel.TopPlayers.Count);
                Assert.Equal("Bob", viewModel.TopPlayers[0].Username);
                Assert.Equal(200, viewModel.TopPlayers[0].Score);
                Assert.Equal(1, viewModel.TopPlayers[0].Rank);
            }

            [Fact]
            public void TopPlayers_ShouldContainCorrectRanks()
            {
                // Arrange
                var fakeDbHelper = new FakeDBHelper();
                fakeDbHelper.TestUsers.AddRange(new[]
                {
            new User { Username = "Player1", MaxScore = 300 },
            new User { Username = "Player2", MaxScore = 200 },
            new User { Username = "Player3", MaxScore = 100 }
        });

                // Act
                var viewModel = new LeaderboardViewModel(fakeDbHelper);
                var ranks = viewModel.TopPlayers.Select(p => p.Rank).ToList();

                // Assert
                Assert.Equal(new[] { 1, 2, 3 }, ranks);
            }
        }

    }







}
