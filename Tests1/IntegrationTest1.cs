using System;
using System.Linq;
using System.Windows;
using Xunit;
using SnakeGame;


namespace Tests1 // ← это должно соответствовать имени файла или структуре проекта
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

            Assert.NotEqual(originalPosition, newPosition);
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
    }
}