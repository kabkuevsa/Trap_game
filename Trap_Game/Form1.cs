using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Trap_game
{
    public partial class Form1 : Form
    {
        private int playerX = 290; // начальная позиция X (600/2 - 10)
        private int playerY = 290; // начальная позиция Y
        private const int playerSize = 20;

        private List<int[]> enemies = new List<int[]>(); // [x, y, dx, dy]
        private const int enemySize = 15;
        private Random rand = new Random();

        private System.Windows.Forms.Timer enemyTimer;

        private List<Point> traps = new List<Point>();
        private const int trapSize = 20;
        private int maxTraps;
        private int trapsLeft;

        private int score = 0;

        private bool gameRunning = true;

        private int lastAddedScore = -1;

        private int enemySpawnTimer = 0; // счётчик тиков
        private const int spawnInterval = 100; // каждые 100 тиков (5 сек при 20 FPS)


        //public Form1()
        //{
        //    InitializeComponent();
        //}




        public Form1()
        {
            InitializeComponent();

            // Создаём таймер для движения врагов
            enemyTimer = new System.Windows.Forms.Timer();
            enemyTimer.Interval = 50; // 20 раз в секунду
            enemyTimer.Tick += EnemyTimer_Tick;
            enemyTimer.Start();

            // Лимит ловушек: 1 на каждые 2500 пикселей площади (600×600 → 144 ловушки)
            maxTraps = (panel1.Width * panel1.Height) / 2500;
            trapsLeft = maxTraps;
            label1.Text = $"Счёт: 0 | Ловушек: {trapsLeft}";
        }



        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Игрок
            e.Graphics.FillRectangle(Brushes.Blue, playerX, playerY, playerSize, playerSize);

            // Враги
            foreach (int[] enemy in enemies)
            {
                e.Graphics.FillRectangle(Brushes.Red, enemy[0], enemy[1], enemySize, enemySize);
            }

            // Ловушки
            foreach (Point trap in traps)
            {
                e.Graphics.FillRectangle(Brushes.Black, trap.X, trap.Y, trapSize, trapSize);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Сброс всей игры
            gameRunning = true;
            score = 0;
            traps.Clear();
            trapsLeft = maxTraps;
            enemies.Clear();
            enemySpawnTimer = 0;

            // Добавляем 3 начальных врага
            for (int i = 0; i < 3; i++)
            {
                AddEnemy(); // используем общий метод
            }

            // Обновляем надпись
            label1.Text = $"Счёт: {score} | Ловушек: {trapsLeft}";

            // Перерисовываем поле
            panel1.Invalidate();

            // Убедимся, что таймер работает
            if (enemyTimer.Enabled == false)
            {
                enemyTimer.Start();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!gameRunning)
                return base.ProcessCmdKey(ref msg, keyData);

            if (keyData == Keys.Space)
            {
                if (trapsLeft > 0)
                {
                    // Проверим, нет ли уже ловушки здесь
                    bool alreadyHasTrap = false;
                    foreach (var trap in traps)
                    {
                        if (trap.X == playerX && trap.Y == playerY)
                        {
                            alreadyHasTrap = true;
                            break;
                        }
                    }

                    if (!alreadyHasTrap)
                    {
                        traps.Add(new Point(playerX, playerY));
                        trapsLeft--;
                        label1.Text = $"Счёт: {score} | Ловушек: {trapsLeft}";
                        panel1.Invalidate();
                    }
                }
                return true;
            }

            // Управление стрелками
            int step = 10;
            switch (keyData)
            {
                case Keys.Left: playerX -= step; break;
                case Keys.Right: playerX += step; break;
                case Keys.Up: playerY -= step; break;
                case Keys.Down: playerY += step; break;
                default: return base.ProcessCmdKey(ref msg, keyData);
            }

            // Границы
            playerX = Math.Max(0, Math.Min(panel1.Width - playerSize, playerX));
            playerY = Math.Max(0, Math.Min(panel1.Height - playerSize, playerY));
            panel1.Invalidate();

            return true;
        }

        private void EnemyTimer_Tick(object sender, EventArgs e)
        {
            if (!gameRunning) return;

            // === ДОБАВЛЕНИЕ ВРАГОВ КАЖДЫЕ 5 СЕКУНД ===
            enemySpawnTimer++;
            if (enemySpawnTimer >= spawnInterval && enemies.Count < 12)
            {
                AddEnemy();
                enemySpawnTimer = 0; // сброс
            }

            // === ДВИЖЕНИЕ И КОЛЛИЗИИ ===
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                int[] enemy = enemies[i];
                enemy[0] += enemy[2];
                enemy[1] += enemy[3];

                // Отскок от стен
                if (enemy[0] <= 0 || enemy[0] >= panel1.Width - enemySize)
                {
                    enemy[2] = -enemy[2];
                    enemy[0] = Math.Max(0, Math.Min(panel1.Width - enemySize, enemy[0]));
                }
                if (enemy[1] <= 0 || enemy[1] >= panel1.Height - enemySize)
                {
                    enemy[3] = -enemy[3];
                    enemy[1] = Math.Max(0, Math.Min(panel1.Height - enemySize, enemy[1]));
                }

                // Столкновение игрока с врагом → проигрыш
                if (enemy[0] < playerX + playerSize && enemy[0] + enemySize > playerX &&
                    enemy[1] < playerY + playerSize && enemy[1] + enemySize > playerY)
                {
                    EndGame(false); // false = проигрыш
                    return;
                }

                // Столкновение врага с ловушкой → +1 очко
                bool hitTrap = false;
                foreach (Point trap in traps)
                {
                    if (enemy[0] < trap.X + trapSize && enemy[0] + enemySize > trap.X &&
                        enemy[1] < trap.Y + trapSize && enemy[1] + enemySize > trap.Y)
                    {
                        hitTrap = true;
                        break;
                    }
                }

                if (hitTrap)
                {
                    enemies.RemoveAt(i);
                    score++;

                    // Победа при 20 очках
                    if (score >= 20)
                    {
                        EndGame(true); // true = победа
                        return;
                    }

                    label1.Text = $"Счёт: {score} | Ловушек: {trapsLeft}";
                }
            }

            panel1.Invalidate();
        }


        private void AddEnemy()
        {
            int x, y;
            do
            {
                x = rand.Next(0, panel1.Width - enemySize);
                y = rand.Next(0, panel1.Height - enemySize);
            }
            while (x + enemySize > playerX && x < playerX + playerSize &&
                   y + enemySize > playerY && y < playerY + playerSize);

            int dx = rand.Next(-3, 4);
            int dy = rand.Next(-3, 4);
            if (dx == 0 && dy == 0) dx = 1;

            enemies.Add(new int[] { x, y, dx, dy });
        }



        private void EndGame(bool isWin)
        {
            gameRunning = false;
            enemyTimer.Stop();

            if (isWin)
            {
                MessageBox.Show("Победа! Вы поймали 20 врагов.", "Победа!");
                label1.Text = $"Победа! Счёт: {score}";
            }
            else
            {
                MessageBox.Show("Вы проиграли!", "Игра окончена");
                label1.Text = $"Игра окончена. Счёт: {score}";
            }
        }
    }
}
