using System;
using System.Collections.Generic;
using System.Drawing;

namespace SnakeGame.Models
{
    internal class Snake
    {
        public List<Point> Body { get; set; } = new();
        public Direction CurrentDirection { get; set; }

        public void Move()
        {
            // TODO: Реализовать движение змейки
        }

        public void Grow()
        {
            // TODO: Реализовать рост змейки
        }
    }
}
