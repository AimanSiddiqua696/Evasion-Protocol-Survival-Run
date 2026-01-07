using SemiFinalGame.Entities;
using SemiFinalGame.FileHandling;
using SemiFinalGame.Interfaces;
using SemiFinalGame.Movements;
using SemiFinalGame.Properties;
using SemiFinalGame.Systems;
using SemiFinalGame.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SemiFinalGame
{
    public partial class GameForm : Form
    {
        private UIManager uiManager;
        private LevelManager levelManager;

        private PictureBox playerSprite;
        private GameObject player;

        private HorizontalMovement horizontalMovement;
        private VerticalMovement verticalMovement;

        private bool gameEnded = false;
        private int lives = 3;
        private int maxLives = 3;
        private bool isInvincible = false;
        private int currentLevel;

        private bool moveLeft, moveRight, moveUp, moveDown;
        private int countdownValue = 3;
        private System.Windows.Forms.Timer startTimer;

        private int initialFormWidth;
        private int initialFormHeight;

        public GameForm(int level = 1)
        {
            this.currentLevel = level;

            InitializeComponent(); //Sets up controls before custom logic
            this.Resize += GameForm_Resize;
            
            // Store initial form size on first call
            initialFormWidth = this.ClientSize.Width;
            initialFormHeight = this.ClientSize.Height;

            this.DoubleBuffered = true; // Prevents flickering
            this.KeyPreview = true; // Allows the form to detect key presses even if a control (like a button) has focus

            // Don't run logic during design time (avoids crashes in the Visual Studio designer)
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            uiManager = new UIManager(this);
            levelManager = new LevelManager(this);

            SetupLevelDifficulty(currentLevel);

            CreatePlayer();
            uiManager.CreateLabels(currentLevel);
            uiManager.CreateHealthBar();
            uiManager.UpdateHealth(lives, maxLives);

            levelManager.SetupLevel(currentLevel);
            if (levelManager.Chaser != null) 
            {
                // Re-link chaser to player if it was created without it
                levelManager.Chaser.Movement = new ChasePlayerMovement(player, 3.5f);
            }

            StartCountdown();

            // 14f Level 1, 28f Level 2
            horizontalMovement = new HorizontalMovement(currentLevel >= 2 ? 28f : 14f);
            verticalMovement = new VerticalMovement(currentLevel >= 2 ? 28f : 14f);

            // 20ms = ~50 FPS
            gameTimer.Interval = 20;
            gameTimer.Tick += GameTimer_Tick;

            this.KeyDown += GameForm_KeyDown;
            this.KeyUp += GameForm_KeyUp;

            SemiFinalGame.Sound.SoundManager.PlayMusic(Resources.GameFormsound);
        }

        private void SetupLevelDifficulty(int level)
        {
            if (level == 1) { uiManager.BackgroundSpeed = 2.0f; maxLives = 3; }
            else if (level == 2) { uiManager.BackgroundSpeed = 5.0f; maxLives = 7; }
            else { uiManager.BackgroundSpeed = 6.0f; maxLives = 8; }

            lives = maxLives;
        }

        private void CreatePlayer()
        {
            playerSprite = new PictureBox
            {
                Size = new Size(80, 80), // Player size
                Location = new Point(100, 300),
                Image = Resources.tile000, // Image from resources
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent // Transparent background if the image has it
            };
            this.Controls.Add(playerSprite);

            // Player logic for Level 3
            player = new Player { Position = new PointF(playerSprite.Left, playerSprite.Top) };

            var anims = new Dictionary<string, List<Image>>();
            List<Image> LoadFrames(int start, int count)
            {
                var frames = new List<Image>();
                for (int i = 0; i < count; i++)
                {
                    string resName = $"tile{(start + i).ToString("D3")}";
                    var img = (Image)Resources.ResourceManager.GetObject(resName);
                    if (img != null) frames.Add(img);
                }
                return frames;
            }

            anims["Down"] = LoadFrames(0, 8);
            anims["Up"] = LoadFrames(8, 8);
            anims["Left"] = LoadFrames(16, 8);
            anims["Right"] = LoadFrames(24, 8);

            // Fallback
            if (anims["Down"].Count == 0) anims["Down"].Add(Resources.tile000);
            foreach (var key in new[] { "Up", "Left", "Right" })
                if (anims[key].Count == 0) anims[key].AddRange(anims["Down"]);

            ((Player)player).SetAnimation(anims, "Down");
        }

        private void StartCountdown()
        {
            countdownValue = 3;
            uiManager.ShowCountdown(countdownValue.ToString());

            startTimer = new System.Windows.Forms.Timer { Interval = 1000 }; // Timer ticks every 1000 milliseconds
            startTimer.Tick += (s, e) =>
            {
                countdownValue--;
                if (countdownValue > 0) uiManager.ShowCountdown(countdownValue.ToString());
                else if (countdownValue == 0) uiManager.ShowCountdown("GO!");
                else
                {
                    // Starts the main game loop
                    startTimer.Stop(); //Stops the countdown timer so it won’t tick again.
                    startTimer.Dispose(); //Frees up memory used by the timer.
                    uiManager.HideCountdown();
                    gameTimer.Start();
                }
            };
            startTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (gameEnded) return;

            UpdatePlayerMovement();
            levelManager.UpdateEntities(currentLevel, playerSprite.Bounds, HandlePlayerHit, () => {
                uiManager.UpdateScore(levelManager.Score);
                SemiFinalGame.Sound.SoundManager.PlaySoundEffect(Resources.CoinSound); // Play Coin Sound
                if (levelManager.Coins.Count == 0) ShowWinMessage(); // WIN CONDITION
            });

            uiManager.UpdateBackground();
            this.Invalidate();
        }

        private void UpdatePlayerMovement()
        {
            string newDirection = null;
            if (moveLeft) { horizontalMovement.MoveLeft(player); newDirection = "Left"; }
            if (moveRight) { horizontalMovement.MoveRight(player, this.ClientSize.Width - playerSprite.Width); newDirection = "Right"; }
            if (moveUp) { verticalMovement.MoveUp(player); newDirection = "Up"; }
            if (moveDown) { verticalMovement.MoveDown(player, this.ClientSize.Height - playerSprite.Height); newDirection = "Down"; }

            if (player is Player p && newDirection != null)
            {
                p.ChangeDirection(newDirection);
                p.Update(new GameTime { DeltaTime = 0.02f });
                playerSprite.Image = p.Sprite;
            }

            playerSprite.Left = (int)player.Position.X;
            playerSprite.Top = (int)player.Position.Y;
        }

        private async void HandlePlayerHit()
        {
            if (isInvincible) return; // Reduce lives logic
            isInvincible = true;

            lives--; // Reduce lives
            uiManager.UpdateHealth(lives, maxLives); // Update health UI

            player.Position = new PointF(100, 300);
            playerSprite.Location = new Point(100, 300);

            // Hit - Check if player is still alive
            if (lives <= 0) GameOver();
            else
            {
                // Wait after hit before continuing
                await Task.Delay(500); 
                isInvincible = false;
            }
        }

        private void GameOver()
        {
            if (gameEnded) return;
            gameEnded = true;
            gameTimer.Stop();
            ResetMovementFlags();

            SemiFinalGame.Sound.SoundManager.StopMusic();
            using (var gameOverForm = new GameOver(levelManager.Score, levelManager.CoinsCollectedCount, lives))
            {
                // Show result window
                if (gameOverForm.ShowDialog() == DialogResult.Yes) RestartGame();
                else this.Close();
            }
            // Update stats
            SaveData.SaveStats(currentLevel, levelManager.Score, levelManager.CoinsCollectedCount, lives);
        }

        private void ShowWinMessage()
        {
            gameEnded = true;
            gameTimer.Stop();
            SaveData.SaveStats(currentLevel, levelManager.Score, levelManager.CoinsCollectedCount, lives);

            SemiFinalGame.Sound.SoundManager.StopMusic();
            using (var victoryForm = new VictoryForm(levelManager.Score, levelManager.CoinsCollectedCount, lives, currentLevel))
            {
                if (victoryForm.ShowDialog() == DialogResult.Yes)
                {
                    if (victoryForm.GoToNextLevel) currentLevel++;
                    RestartGame();
                }
                else this.Close();
            }
        }

        private void RestartGame()
        {
            gameEnded = false;
            ResetMovementFlags();
            SetupLevelDifficulty(currentLevel);
            
            player.Position = new PointF(100, 300);
            playerSprite.Location = new Point(100, 300);

            uiManager.UpdateScore(0);
            uiManager.UpdateHealth(lives, maxLives);
            uiManager.UpdateLevel(currentLevel);

            levelManager.SetupLevel(currentLevel);
            if (levelManager.Chaser != null) levelManager.Chaser.Movement = new ChasePlayerMovement(player, 3.5f);

            gameTimer.Start();
            SemiFinalGame.Sound.SoundManager.PlayMusic(Resources.GameFormsound);
            this.Focus();
        }

        protected override void OnPaint(PaintEventArgs e) => uiManager.DrawBackground(e.Graphics);

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

        private void ResetMovementFlags() => moveLeft = moveRight = moveUp = moveDown = false;

        private void GameForm_Resize(object sender, EventArgs e)
        {
            if (gameEnded || uiManager == null || levelManager == null) return;
            
            // Rescale player
            float xRatio = player.Position.X / initialFormWidth;
            float yRatio = player.Position.Y / initialFormHeight;
            playerSprite.Left = (int)(xRatio * this.ClientSize.Width);
            playerSprite.Top = (int)(yRatio * this.ClientSize.Height);
            player.Position = new PointF(playerSprite.Left, playerSprite.Top);

            // Delegate entity resizing to LevelManager
            levelManager.HandleResize(this.ClientSize.Width, this.ClientSize.Height, initialFormWidth, initialFormHeight, currentLevel);
            uiManager.CenterCountdownLabel();
        }

        private void GameForm_Load(object sender, EventArgs e) { }
    }
}
