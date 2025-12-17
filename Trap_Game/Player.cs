using System;
using System.Collections.Generic;
using System.Drawing;
using Trap_game;

namespace TrapsGame
{
    public class Player
    {
        public Point Position { get; private set; }
        public Rectangle Bounds => new Rectangle(Position.X - 10, Position.Y - 10, 20, 20);
        private const int SPEED = 5;

        public Player(Point startPosition)
        {
            Position = startPosition;
        }

        public void Move(int dx, int dy, int maxWidth, int maxHeight, int minY)
        {
            int newX = Position.X + dx;
            int newY = Position.Y + dy;

            // Проверка границ
            if (newX - 10 >= 10 && newX + 10 <= maxWidth)
            {
                Position = new Point(newX, Position.Y);
            }

            if (newY - 10 >= minY && newY + 10 <= maxHeight)
            {
                Position = new Point(Position.X, newY);
            }
        }

        public void MoveToTarget(Point target, List<Trap> traps)
        {
            // Простой поиск пути с обходом ловушек
            int dx = 0, dy = 0;

            if (Math.Abs(target.X - Position.X) > SPEED)
            {
                dx = target.X > Position.X ? SPEED : -SPEED;
            }

            if (Math.Abs(target.Y - Position.Y) > SPEED)
            {
                dy = target.Y > Position.Y ? SPEED : -SPEED;
            }

            // Проверка на столкновение с ловушками
            Rectangle newBounds = new Rectangle(
                Position.X - 10 + dx,
                Position.Y - 10 + dy,
                20, 20
            );

            bool canMove = true;
            foreach (var trap in traps)
            {
                if (newBounds.IntersectsWith(trap.Bounds))
                {
                    canMove = false;
                    break;
                }
            }

            if (canMove)
            {
                Position = new Point(Position.X + dx, Position.Y + dy);
            }
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Blue, Bounds);
            g.DrawRectangle(Pens.DarkBlue, Bounds);

            // Отрисовка глаз
            g.FillEllipse(Brushes.White, Position.X - 5, Position.Y - 5, 6, 6);
            g.FillEllipse(Brushes.White, Position.X + 1, Position.Y - 5, 6, 6);
            g.FillEllipse(Brushes.Black, Position.X - 3, Position.Y - 3, 3, 3);
            g.FillEllipse(Brushes.Black, Position.X + 3, Position.Y - 3, 3, 3);
        }
    }
}