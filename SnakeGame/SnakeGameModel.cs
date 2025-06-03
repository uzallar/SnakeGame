using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SnakeGame
{
    public class SnakeGameModel
    {
        public const int SnakeSize = 20;
        public const int GameAreaWidth = 600;
        public const int GameAreaHeight = 400;

        public List<Point> SnakeParts { get; private set; }
        public Point SnakeHeadPosition { get; set; }
        public Vector SnakeDirection { get; set; }
        public int SnakeLength { get; set; }
        public Point FoodPosition { get; set; }
        public int Score { get; set; }

        private Random random;

        public SnakeGameModel()
        {
            random = new Random();
            SnakeParts = new List<Point>();
        }

        public void InitializeGame()
        {
            SnakeParts.Clear();
            SnakeHeadPosition = new Point(GameAreaWidth / 2, GameAreaHeight / 2);
            SnakeDirection = new Vector(SnakeSize, 0);
            SnakeLength = 5;
            Score = 0;

            SnakeParts.Add(SnakeHeadPosition);

            for (int i = 1; i < SnakeLength; i++)
            {
                AddSnakePart();
            }

            CreateFood();
        }

        public void AddSnakePart()
        {
            Point newPart = new Point(
                SnakeParts[SnakeParts.Count - 1].X - SnakeDirection.X,
                SnakeParts[SnakeParts.Count - 1].Y - SnakeDirection.Y);
            SnakeParts.Add(newPart);
        }

        public void CreateFood()
        {
            int maxX = (int)(GameAreaWidth / SnakeSize) - 1;
            int maxY = (int)(GameAreaHeight / SnakeSize) - 1;

            FoodPosition = new Point(
                random.Next(0, maxX) * SnakeSize,
                random.Next(0, maxY) * SnakeSize);

            // Проверка, чтобы еда не появилась на змейке
            foreach (var part in SnakeParts)
            {
                if (part == FoodPosition)
                {
                    CreateFood();
                    return;
                }
            }
        }

        public void MoveSnake()
        {
            Point previousPosition = SnakeHeadPosition;
            SnakeHeadPosition = new Point(
                SnakeHeadPosition.X + SnakeDirection.X,
                SnakeHeadPosition.Y + SnakeDirection.Y
            );

            SnakeParts[0] = SnakeHeadPosition;

            for (int i = 1; i < SnakeParts.Count; i++)
            {
                Point currentPosition = SnakeParts[i];
                SnakeParts[i] = previousPosition;
                previousPosition = currentPosition;
            }
        }

        public bool CheckCollisions()
        {
            // Проверка столкновения с границами
            if (SnakeHeadPosition.X < 0 || SnakeHeadPosition.X > GameAreaWidth - SnakeSize ||
                SnakeHeadPosition.Y < 0 || SnakeHeadPosition.Y > GameAreaHeight - SnakeSize)
            {
                return true;
            }

            // Проверка столкновения с собой
            for (int i = 3; i < SnakeParts.Count; i++)
            {
                if (SnakeHeadPosition == SnakeParts[i])
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckFoodCollision()
        {
            if (SnakeHeadPosition == FoodPosition)
            {
                Score += 10;
                AddSnakePart();
                CreateFood();
                return true;
            }
            return false;
        }
    }
}
