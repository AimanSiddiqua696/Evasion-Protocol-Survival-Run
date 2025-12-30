using System;
using System.Drawing;
using System.Windows.Forms;
using SemiFinalGame.Entities;
using SemiFinalGame.Movements;


namespace SemiFinalGame
{
    public partial class GameForm : Form
    {
        private List<Obstacle> obstacles = new List<Obstacle>();
        private List<Coin> coins = new List<Coin>();
        private int score = 0;
        private Label scoreLabel;

        private PictureBox playerSprite;
        private GameObject player;

        private HorizontalMovement horizontalMovement;
        private VerticalMovement verticalMovement;
        private bool gameEnded = false;
        private int initialFormWidth;
        private int initialFormHeight;



        // movement flags
        private bool moveLeft, moveRight, moveUp, moveDown;

        private void CreatePlayer()
        {
            playerSprite = new PictureBox();
            playerSprite.Size = new Size(48, 48);   // adjust if needed
            playerSprite.Location = new Point(100, 300);

            //  IMAGE FROM RESOURCES
            playerSprite.Image = Properties.Resources.run_down0;
            playerSprite.SizeMode = PictureBoxSizeMode.StretchImage;
            playerSprite.BackColor = Color.Transparent;

            this.Controls.Add(playerSprite);

            // Link GameObject with sprite
            player = new GameObject();
            player.Position = new PointF(playerSprite.Left, playerSprite.Top);
        }

        public GameForm()
        {
            InitializeComponent();
            this.Resize += GameForm_Resize;
            initialFormWidth = this.ClientSize.Width;
            initialFormHeight = this.ClientSize.Height;



            this.DoubleBuffered = true;
            this.KeyPreview = true;

            CreatePlayer();     // Add player first

            CreateScoreLabel(); // Add score label last
            SpawnCoins();       // Add coins next
            SetupObstacles();   // Add obstacles

            scoreLabel.BringToFront();

            horizontalMovement = new HorizontalMovement(5f);
            verticalMovement = new VerticalMovement(5f);

            gameTimer.Interval = 20;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            this.KeyDown += GameForm_KeyDown;
            this.KeyUp += GameForm_KeyUp;
        }


        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // ================= PLAYER MOVEMENT =================
            if (moveLeft)
                horizontalMovement.MoveLeft(player);

            if (moveRight)
                horizontalMovement.MoveRight(player, this.ClientSize.Width - playerSprite.Width);

            if (moveUp)
                verticalMovement.MoveUp(player);

            if (moveDown)
                verticalMovement.MoveDown(player, this.ClientSize.Height - playerSprite.Height);

            // Apply position to sprite
            playerSprite.Left = (int)player.Position.X;
            playerSprite.Top = (int)player.Position.Y;

            // ================= OBSTACLE UPDATE =================
            bool hitObstacle = false;   // FLAG

            foreach (Obstacle obstacle in obstacles)
            {
                obstacle.Update();

                // Only detect collision, DO NOT call GameOver here
                if (playerSprite.Bounds.IntersectsWith(obstacle.Sprite.Bounds))
                {
                    hitObstacle = true;
                }
            }

            // ================= COINS UPDATE =================
            UpdateCoins();

            // ================= GAME OVER AFTER LOOP =================
            if (hitObstacle)
            {
                GameOver();
            }
        }

        private bool isGameOver = false;

        private void GameOver()
        {
            if (gameEnded) return;

            gameEnded = true;
            gameTimer.Stop();

            DialogResult result = MessageBox.Show(
                " GAME OVER!\nDo you want to play again?",
                "Gravity Run",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
                RestartGame();
            else
                this.Close();
        }


        private void RestartGame()
        {
            // Reset flags
            gameEnded = false;
            isGameOver = false;

            // Reset score
            score = 0;
            scoreLabel.Text = "Score: 0";

            // Reset player
            player.Position = new PointF(100, 300);
            playerSprite.Left = 100;
            playerSprite.Top = 300;

            // Remove OLD coins from form
            foreach (Coin coin in coins)
            {
                if (this.Controls.Contains(coin.Sprite))
                    this.Controls.Remove(coin.Sprite);
            }
            coins.Clear();

            // Remove OLD obstacles
            foreach (Obstacle obs in obstacles)
            {
                if (this.Controls.Contains(obs.Sprite))
                    this.Controls.Remove(obs.Sprite);
            }
            obstacles.Clear();

            // Spawn fresh game objects
            SpawnCoins();        // IMPORTANT
            SetupObstacles();    // IMPORTANT

            gameTimer.Start();
        }




        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) moveLeft = true;
            if (e.KeyCode == Keys.Right) moveRight = true;
            if (e.KeyCode == Keys.Up) moveUp = true;
            if (e.KeyCode == Keys.Down) moveDown = true;
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) moveLeft = false;
            if (e.KeyCode == Keys.Right) moveRight = false;
            if (e.KeyCode == Keys.Up) moveUp = false;
            if (e.KeyCode == Keys.Down) moveDown = false;
        }
        private void CreateScoreLabel()
        {
            scoreLabel = new Label();
            scoreLabel.Text = "Score: 0";
            scoreLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            scoreLabel.ForeColor = Color.Black;
            scoreLabel.BackColor = Color.Transparent; // optional
            scoreLabel.AutoSize = true;
            scoreLabel.Location = new Point(10, 10);

            this.Controls.Add(scoreLabel);
            scoreLabel.BringToFront(); // make sure it's on top of all PictureBoxes
        }

        private void SpawnCoins()
        {
            coins.Clear();

            int formWidth = this.ClientSize.Width;
            int formHeight = this.ClientSize.Height;

            Random rnd = new Random();

            for (int i = 0; i < 10; i++)
            {
                // Random position but spread across the form
                int x = rnd.Next(50, formWidth - 50);
                int y = rnd.Next(50, formHeight - 50);

                Image coinImage;
                int coinValue;

                // Alternate silver and gold
                if (i % 2 == 0)
                {
                    coinImage = Properties.Resources.giphy; // Silver coin
                    coinValue = 2;
                }
                else
                {
                    coinImage = Properties.Resources.giphy1; // Gold coin
                    coinValue = 5;
                }

                // Horizontal bounds for patrol ±50 pixels from start X
                float leftBound = Math.Max(0, x - 50);
                float rightBound = Math.Min(formWidth - 32, x + 50); // 32 = coin width

                Coin coin = new Coin(coinImage, new Point(x, y), coinValue, new Size(32, 32), leftBound, rightBound);

                this.Controls.Add(coin.Sprite);
                coins.Add(coin);
            }

            scoreLabel.BringToFront(); // Ensure score is on top
        }




        private void UpdateCoins()
        {
            if (gameEnded) return;

            foreach (Coin coin in coins.ToList())
            {
                coin.Movement.Move(coin.Body, null);

                coin.Sprite.Left = (int)coin.Body.Position.X;
                coin.Sprite.Top = (int)coin.Body.Position.Y;

                if (playerSprite.Bounds.IntersectsWith(coin.Sprite.Bounds))
                {
                    score += coin.Value;
                    scoreLabel.Text = "Score: " + score;

                    this.Controls.Remove(coin.Sprite);
                    coins.Remove(coin);
                }
            }

            // ✅ WIN CONDITION
            if (coins.Count == 0 && !gameEnded)
            {
                gameEnded = true;
                gameTimer.Stop();
                ShowWinMessage();
            }
        }
        private void ShowWinMessage()
        {
            DialogResult result = MessageBox.Show(
                "🎉 YOU WON!\nDo you want to play again?",
                "Victory",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information
            );

            if (result == DialogResult.Yes)
                RestartGame();
            else
                this.Close();
        }




        private void SetupObstacles()
        {
            obstacles.Clear(); // Clear old obstacles if any

            int formHeight = this.ClientSize.Height;

            // Define X positions and initial Y positions for 5 obstacles
            int[] xPositions = { 150, 300, 450, 600, 750 };
            float[] startYPositions = { 0, formHeight - 20, 50, formHeight - 70, 100 };
            float[] speeds = { 2.5f, -2.5f, 3f, -3f, 2f }; // mix of down/up

            for (int i = 0; i < 5; i++)
            {
                PictureBox box = new PictureBox();
                box.Size = new Size(60, 20);
                box.Location = new Point(xPositions[i], (int)startYPositions[i]);
                box.Image = Properties.Resources.box; // make sure image exists
                box.SizeMode = PictureBoxSizeMode.StretchImage;
                box.BackColor = Color.Transparent;

                this.Controls.Add(box);

                // Vertical patrol bounds
                float topBound = 0;
                float bottomBound = formHeight - box.Height;

                obstacles.Add(new Obstacle(
                    box,
                    new VerticalPatrolMovement(topBound, bottomBound, speeds[i])
                ));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;   // optional
        }

        //private int initialFormWidth;
        //private int initialFormHeight;

        private void GameForm_Resize(object sender, EventArgs e)
        {
            if (gameEnded) return;

            // Store initial form size on first call
            if (initialFormWidth == 0 || initialFormHeight == 0)
            {
                initialFormWidth = this.ClientSize.Width;
                initialFormHeight = this.ClientSize.Height;
                return;
            }

            int formWidth = this.ClientSize.Width;
            int formHeight = this.ClientSize.Height;

            // ====== Player ======
            float playerXRatio = player.Position.X / (float)initialFormWidth;
            float playerYRatio = player.Position.Y / (float)initialFormHeight;

            playerSprite.Left = (int)(playerXRatio * formWidth);
            playerSprite.Top = (int)(playerYRatio * formHeight);
            player.Position = new PointF(playerSprite.Left, playerSprite.Top);

            // ====== Coins ======
            foreach (Coin coin in coins)
            {
                float coinXRatio = coin.Body.Position.X / (float)initialFormWidth;
                float coinYRatio = coin.Body.Position.Y / (float)initialFormHeight;

                coin.Sprite.Left = (int)(coinXRatio * formWidth);
                coin.Sprite.Top = (int)(coinYRatio * formHeight);
                coin.Body.Position = new PointF(coin.Sprite.Left, coin.Sprite.Top);

                // Update horizontal patrol bounds
                coin.Movement = new HorizontalPatrolMovement(
                    Math.Max(0, coin.Sprite.Left - 50),
                    Math.Min(formWidth - coin.Sprite.Width, coin.Sprite.Left + 50)
                );
            }

            // ====== Obstacles ======
            foreach (Obstacle obs in obstacles)
            {
                float obsXRatio = obs.Sprite.Left / (float)initialFormWidth;
                float obsYRatio = obs.Sprite.Top / (float)initialFormHeight;

                obs.Sprite.Left = (int)(obsXRatio * formWidth);
                obs.Sprite.Top = (int)(obsYRatio * formHeight);
                obs.Body.Position = new PointF(obs.Sprite.Left, obs.Sprite.Top);

                // Update vertical patrol bounds
                if (obs.Movement is VerticalPatrolMovement)
                {
                    obs.Movement = new VerticalPatrolMovement(
                        0, // top bound
                        formHeight - obs.Sprite.Height, // bottom bound
                        2f // default speed
                    );
                }
            }
        }





    }
}

