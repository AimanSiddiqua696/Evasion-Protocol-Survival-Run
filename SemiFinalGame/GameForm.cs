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

        // movement flags
        private bool moveLeft, moveRight, moveUp, moveDown;

        private void CreatePlayer()
        {
            playerSprite = new PictureBox();
            playerSprite.Size = new Size(48, 48);   // adjust if needed
            playerSprite.Location = new Point(100, 300);

            // 👇 IMAGE FROM RESOURCES
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
            //foreach (Obstacle obstacle in obstacles)
            //{
            //    obstacle.Update();

            //    if (playerSprite.Bounds.IntersectsWith(obstacle.Sprite.Bounds))
            //    {
            //        GameOver();
            //    }
            //}
            foreach (Obstacle obstacle in obstacles.ToList())
                obstacle.Update();

            // Update coins
            UpdateCoins(); // 👈 This will move coins and handle collection

            // Collision with obstacles
            foreach (Obstacle obstacle in obstacles)
            {
                if (playerSprite.Bounds.IntersectsWith(obstacle.Sprite.Bounds))
                {
                    GameOver();
                }
            }


        }
        private bool isGameOver = false;

        private void GameOver()
        {
            if (isGameOver) return;

            isGameOver = true;
            gameTimer.Stop();

            // Show message box with Yes/No buttons
            DialogResult result = MessageBox.Show(
                "Game Over!\nDo you want to play again?",   // message
                "Gravity Run",                              // title
                MessageBoxButtons.YesNo,                    // buttons
                MessageBoxIcon.Question                    // icon
            );

            if (result == DialogResult.Yes)
            {
                RestartGame();  // Reset everything
            }
            else
            {
                this.Close();   // Exit the game
            }
        }

        private void RestartGame()
        {
            // Reset player position
            player.Position = new PointF(100, 300);
            playerSprite.Left = (int)player.Position.X;
            playerSprite.Top = (int)player.Position.Y;

            // Remove old obstacle PictureBoxes from the Form
            foreach (Obstacle obs in obstacles)
            {
                if (this.Controls.Contains(obs.Sprite))
                    this.Controls.Remove(obs.Sprite);
                obs.Sprite.Dispose();  // optional, to free resources
            }

            // Clear obstacle list
            obstacles.Clear();

            // Setup new obstacles
            SetupObstacles();

            // Reset movement flags
            moveLeft = moveRight = moveUp = moveDown = false;

            isGameOver = false;
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
            foreach (Coin coin in coins.ToList()) // ToList prevents collection modified error
            {
                coin.Movement.Move(coin.Body, null); // Move horizontally

                // Apply new position to sprite
                coin.Sprite.Left = (int)coin.Body.Position.X;
                coin.Sprite.Top = (int)coin.Body.Position.Y;

                // Collision detection with player
                if (playerSprite.Bounds.IntersectsWith(coin.Sprite.Bounds))
                {
                    score += coin.Value;
                    scoreLabel.Text = "Score: " + score;

                    this.Controls.Remove(coin.Sprite); // Remove from form
                    coins.Remove(coin);                // Remove from list
                }
            }
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




    }
}















































        //    private List<Obstacle> obstacles = new List<Obstacle>();

        //    private GameObject player;

        //    private HorizontalMovement horizontalMovement;
        //    private VerticalMovement verticalMovement;
        //    private bool isGameOver = false;


        //    // Key states
        //    private bool moveLeft = false;
        //    private bool moveRight = false;
        //    private bool moveUp = false;
        //    private bool moveDown = false;

        //    public GameForm()
        //    {
        //        InitializeComponent();
        //        SetupObstacles();
        //        // Initialize GameObject for player
        //        player = new GameObject();
        //        player.Position = new PointF(playerdown.Left, playerdown.Top);

        //        // Initialize movement objects with speed
        //        horizontalMovement = new HorizontalMovement(5f); // Horizontal speed
        //        verticalMovement = new VerticalMovement(5f);     // Vertical speed

        //        // Timer setup
        //        gameTimer.Interval = 20;
        //        gameTimer.Tick += GameTimer_Tick;
        //        gameTimer.Start();

        //        // Key events
        //        this.KeyDown += GameForm_KeyDown;
        //        this.KeyUp += GameForm_KeyUp;
        //    }

        //    private void GameForm_KeyDown(object sender, KeyEventArgs e)
        //    {
        //        if (e.KeyCode == Keys.Left) moveLeft = true;
        //        if (e.KeyCode == Keys.Right) moveRight = true;
        //        if (e.KeyCode == Keys.Up) moveUp = true;
        //        if (e.KeyCode == Keys.Down) moveDown = true;
        //    }

        //    private void GameForm_KeyUp(object sender, KeyEventArgs e)
        //    {
        //        if (e.KeyCode == Keys.Left) moveLeft = false;
        //        if (e.KeyCode == Keys.Right) moveRight = false;
        //        if (e.KeyCode == Keys.Up) moveUp = false;
        //        if (e.KeyCode == Keys.Down) moveDown = false;
        //    }

        //    private void GameTimer_Tick(object sender, EventArgs e)
        //    {
        //        // Horizontal movement
        //        if (moveLeft) horizontalMovement.MoveLeft(player);
        //        if (moveRight) horizontalMovement.MoveRight(player, this.ClientSize.Width - playerdown.Width);

        //        // Vertical movement
        //        if (moveUp) verticalMovement.MoveUp(player);
        //        if (moveDown) verticalMovement.MoveDown(player, this.ClientSize.Height - playerdown.Height);

        //        // Apply updated position to PictureBox
        //        playerdown.Left = (int)player.Position.X;
        //        playerdown.Top = (int)player.Position.Y;
        //        foreach (Obstacle obstacle in obstacles)
        //        {
        //            obstacle.Update();

        //            // Collision check
        //            if (playerdown.Bounds.IntersectsWith(obstacle.Sprite.Bounds))
        //            {
        //                GameOver();
        //            }
        //        }
        //    }
        //private void SetupObstacles()
        //{
        //    obstacles.Add(new Obstacle(
        //        box1,
        //         //new HorizontalPatrolMovement(
        //         //    box1.Left - 100,
        //         //    box1.Left + 100
        //         //   //box2.Top - 100,
        //         //   // box2.Top + 100
        //         //)
        //         new VerticalPatrolMovement(
        //            box1.Top - 100,
        //            box1.Top + 100
        //        //box1.Left - 100,
        //        //box1.Left + 100
        //        )
        //    ));

        //    obstacles.Add(new Obstacle(
        //        box2,
        //        new VerticalPatrolMovement(
        //            box2.Top - 100,
        //            box2.Top + 100
        //        //box1.Left - 100,
        //        //box1.Left + 100
        //        )
        //    ));
        //}

        //    private void GameOver()
        //    {
        //        if (isGameOver) return; // prevent multiple calls

        //        isGameOver = true;
        //        gameTimer.Stop();

        //        MessageBox.Show(
        //            "Game Over!",
        //            "Gravity Run",
        //            MessageBoxButtons.OK,
        //            MessageBoxIcon.Information
        //        );
        //    }


        //}
//    }
//}
