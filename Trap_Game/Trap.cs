using System.Drawing;

namespace TrapsGame
{
    public class Trap
    {
        public Point Position { get; private set; }
        public Rectangle Bounds => new Rectangle(Position.X - 10, Position.Y - 10, 20, 20);

        public Trap(Point position)
        {
            Position = position;
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Black, Bounds);
            g.DrawRectangle(Pens.Gray, Bounds);

            // Рисуем "шипы"
            for (int i = 0; i < 4; i++)
            {
                int x = Position.X - 10 + i * 5;
                int y = Position.Y - 10 + i * 5;
                g.DrawLine(Pens.White, x, Position.Y - 10, x, Position.Y + 10);
                g.DrawLine(Pens.White, Position.X - 10, y, Position.X + 10, y);
            }
        }
    }
}