using System;
using System.Drawing;

namespace TrapsGame
{
    public class Enemy
    {
        public Point Position { get; private set; }
        public Rectangle Bounds => new Rectangle(Position.X - 7, Position.Y - 7, 15, 15);
        private Point direction;
        private Random random;
        private int speed = 3;

        public Enemy(Point startPosition)
        {
            Position = startPosition;
            random = new Random();

            // Случайное начальное направление
            direction = new Point(
                random.Next(-1, 2),
                random.Next(-1, 2)
            );

            if (direction.X == 0 && direction.Y == 0)
                direction.X = 1;
        }

        public void Update(int maxWidth, int maxHeight, int minY)
        {
            // Случайное изменение направления
            if (random.Next(100) < 5) // 5% шанс изменить направление
            {
                direction = new Point(
                    random.Next(-1, 2),
                    random.Next(-1, 2)
                );

                if (direction.X == 0 && direction.Y == 0)
                    direction.X = 1;
            }

            // Движение
            int newX = Position.X + direction.X * speed;
            int newY = Position.Y + direction.Y * speed;

            // Проверка границ и отскок
            if (newX - 7 <= 10 || newX + 7 >= maxWidth)
            {
                direction.X *= -1;
                newX = Position.X + direction.X * speed;
            }

            if (newY - 7 <= minY || newY + 7 >= maxHeight)
            {
                direction.Y *= -1;
                newY = Position.Y + direction.Y * speed;
            }

            Position = new Point(newX, newY);
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Red, Bounds);
            g.DrawRectangle(Pens.DarkRed, Bounds);

            // Отрисовка глаз
            g.FillEllipse(Brushes.White, Position.X - 3, Position.Y - 3, 4, 4);
            g.FillEllipse(Brushes.White, Position.X + 1, Position.Y - 3, 4, 4);
            g.FillEllipse(Brushes.Black, Position.X - 2, Position.Y - 2, 2, 2);
            g.FillEllipse(Brushes.Black, Position.X + 2, Position.Y - 2, 2, 2);
        }
    }
}