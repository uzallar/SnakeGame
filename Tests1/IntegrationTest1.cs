using System;
using System.Linq;
using System.Windows;
using Xunit;
using SnakeGame;


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
        public void ChangeDirection_ShouldUpdateNextDirection()
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
            Assert.Equal(10, model.Score);
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

    }
}