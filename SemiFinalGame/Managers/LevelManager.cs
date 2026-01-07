using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SemiFinalGame.Entities;
using SemiFinalGame.Interfaces;
using SemiFinalGame.Movements;
using SemiFinalGame.Properties;
using SemiFinalGame.Systems;

namespace SemiFinalGame.Managers
{
    public class LevelManager
    {
        private Form parentForm;
        private List<Obstacle> obstacles = new List<Obstacle>();
        private List<Coin> coins = new List<Coin>();
        private List<Stone> stones = new List<Stone>(); // List to hold stones
        private Enemy chaser;
        private PictureBox chaserSprite;
        private PhysicsSystem physicsEngine = new PhysicsSystem();

        public int CoinsCollectedCount { get; private set; } = 0;
        public int Score { get; private set; } = 0;

        public List<Obstacle> Obstacles => obstacles;
        public List<Coin> Coins => coins;
        public List<Stone> Stones => stones;
        public Enemy Chaser => chaser;
        public PictureBox ChaserSprite => chaserSprite;

        public LevelManager(Form form)
        {
            parentForm = form;
        }

        public void SetupLevel(int currentLevel)
        {
            ClearAll();
            SpawnCoins();       // Add coins next
            SetupObstacles(currentLevel);   // Add obstacles
            SetupStones(currentLevel);      // Add stones for Level 2
            SetupChaser(currentLevel, null); // Add chaser for Level 3 (Player will be linked later)
        }

        public void ClearAll()
        {
            // Remove OLD coins from form
            foreach (var coin in coins) parentForm.Controls.Remove(coin.Sprite);
            coins.Clear();

            // Remove OLD obstacles
            foreach (var obs in obstacles) parentForm.Controls.Remove(obs.Sprite);
            obstacles.Clear();

            // Clear existing stones
            foreach (var stone in stones) parentForm.Controls.Remove(stone.Sprite);
            stones.Clear();

            // Clean up existing chaser
            if (chaserSprite != null)
            {
                parentForm.Controls.Remove(chaserSprite);
                chaserSprite.Dispose();
                chaserSprite = null;
            }
            chaser = null;
            
            // Reset score
            Score = 0;
            CoinsCollectedCount = 0;
        }

        public void SpawnCoins()
        {
            int formWidth = parentForm.ClientSize.Width;
            int formHeight = parentForm.ClientSize.Height;
            Random rnd = new Random();

            for (int i = 0; i < 20; i++)
            {
                // Random position but spread across the form
                int x = rnd.Next(50, formWidth - 50);
                int y = rnd.Next(50, formHeight - 50);

                Image coinImage;
                int coinValue;

                // Alternate silver and gold
                if (i % 2 == 0)
                {
                    coinImage = Resources.giphy; // Silver coin
                    coinValue = 2;
                }
                else
                {
                    coinImage = Resources.giphy1; // Gold coin
                    coinValue = 5;
                }

                // Horizontal bounds for patrol ±50 pixels from start X
                float leftBound = Math.Max(0, x - 50);
                float rightBound = Math.Min(formWidth - 32, x + 50); // 32 = coin width

                Coin coin = new Coin(coinImage, new Point(x, y), coinValue, new Size(48, 48), leftBound, rightBound, formWidth, formHeight);
                parentForm.Controls.Add(coin.Sprite);
                coins.Add(coin);
            }
        }

        public void SetupObstacles(int currentLevel)
        {
            int obstacleCount;
            if (currentLevel == 2)
                obstacleCount = 5; // 20 obstacles for Level 2
            else
                obstacleCount = 10; // Default Level 1

            int formWidth = parentForm.ClientSize.Width;
            int formHeight = parentForm.ClientSize.Height;
            Random rnd = new Random();
            
            // Even horizontal spacing
            int spacing = formWidth / (obstacleCount + 1);

            for (int i = 0; i < obstacleCount; i++)
            {
                PictureBox box = new PictureBox
                {
                    Size = new Size(80, 30), // Increased from 60,20
                    Location = new Point(spacing * (i + 1), rnd.Next(0, formHeight - 30)),
                    Image = Resources.box,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.Transparent
                };
                parentForm.Controls.Add(box);

                // Vertical patrol bounds
                float topBound = 0;
                float bottomBound = formHeight - box.Height;

                // Random up/down speed based on Level
                int minSpeed, maxSpeed;
                if (currentLevel == 1)
                {
                    minSpeed = 3;
                    maxSpeed = 6;
                }
                else if (currentLevel == 2)
                {
                    minSpeed = 6;   //  faster than Level 1
                    maxSpeed = 9;
                }
                else
                {
                    minSpeed = 8;
                    maxSpeed = 12;
                }

                float speed = rnd.Next(minSpeed, maxSpeed);
                if (rnd.Next(2) == 0) speed = -speed;

                obstacles.Add(new Obstacle(box, new VerticalPatrolMovement(topBound, bottomBound, speed)));
            }
        }

        public void SetupStones(int currentLevel)
        {
            // Only spawn stones in Level 2 or higher
            if (currentLevel < 2) return;

            int stoneCount = 10; // Number of stones
            int formWidth = parentForm.ClientSize.Width;
            int formHeight = parentForm.ClientSize.Height;
            Random rnd = new Random();

            for (int i = 0; i < stoneCount; i++)
            {
                PictureBox stoneBox = new PictureBox
                {
                    Size = new Size(48, 48),
                    // Random starting position (some on screen, some above)
                    Location = new Point(rnd.Next(50, formWidth - 50), rnd.Next(-600, -50)), // Start above the screen
                    Image = Resources.stone, // stone resource exists
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.Transparent
                };
                parentForm.Controls.Add(stoneBox);

                if (currentLevel == 3)
                {
                    // In Level 3, we use real physics!
                    stoneBox.BackColor = Color.Transparent; // Fix transparency
                    Stone physicalStone = new Stone(stoneBox, null); // PhysicsSystem will handle movement
                    physicalStone.Body.HasPhysics = true;
                    // Reduced gravity range for better playability
                    physicalStone.Body.CustomGravity = 0.5f + (float)rnd.NextDouble() * 0.10f;
                    stones.Add(physicalStone);
                }
                else
                {
                    // Use existing VerticalPatrolMovement for Level 2
                    // Movement: VerticalPatrolMovement but configured to just fall endlessly
                    // We set bottomBound very high so it doesn't bounce back up automatically
                    float speed = rnd.Next(5, 12); // Random falling speed
                    stones.Add(new Stone(stoneBox, new VerticalPatrolMovement(-1000, formHeight + 2000, speed)));
                }
            }
        }

        public void SetupChaser(int currentLevel, GameObject player)
        {
            if (currentLevel < 3) return;

            // Create Sprite
            chaserSprite = new PictureBox
            {
                Size = new Size(100, 100),
                BackColor = Color.Transparent,
                Image = Resources.rightside,
                Location = new Point(parentForm.ClientSize.Width - 100, 100), // Start top-right
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            parentForm.Controls.Add(chaserSprite);
            chaserSprite.BringToFront();

            // Create Entity
            chaser = new Enemy
            {
                Position = new PointF(chaserSprite.Left, chaserSprite.Top),
                Size = new SizeF(chaserSprite.Width, chaserSprite.Height)
            };

            // Movement - using the existing class ChasePlayerMovement
            // Make sure player is created before this!
            if (player != null)
            {
                // Faster than stones, but escapable
                chaser.Movement = new ChasePlayerMovement(player, 3.5f);
            }
        }

        public void UpdateEntities(int currentLevel, Rectangle playerBounds, Action onHit, Action onCoinCollected)
        {
            // OBSTACLE UPDATE 
            foreach (var obs in obstacles)
            {
                obs.Update();
                // Only detect collision, DO NOT call GameOver here
                if (playerBounds.IntersectsWith(obs.Sprite.Bounds)) onHit?.Invoke();
            }

            // COINS UPDATE
            foreach (var coin in coins.ToList())
            {
                coin.Movement.Move(coin.Body, null);
                coin.Sprite.Left = (int)coin.Body.Position.X;
                coin.Sprite.Top = (int)coin.Body.Position.Y;

                if (playerBounds.IntersectsWith(coin.Sprite.Bounds))
                {
                    Score += coin.Value;
                    CoinsCollectedCount++; // Track count
                    parentForm.Controls.Remove(coin.Sprite);
                    coins.Remove(coin);
                    onCoinCollected?.Invoke();
                }
            }

            // STONES UPDATE (Level 2) 
            UpdateStones(currentLevel, playerBounds, onHit);

            // CHASER UPDATE (Level 3) 
            UpdateChaser(playerBounds, onHit);
        }

        private void UpdateStones(int currentLevel, Rectangle playerBounds, Action onHit)
        {
            if (currentLevel < 2) return;

            // Apply Physics to stones in Level 3
            if (currentLevel == 3 && stones.Count > 0)
            {
                physicsEngine.Apply(stones.Select(s => s.Body).ToList());
            }

            Random rnd = new Random();
            int formHeight = parentForm.ClientSize.Height;
            int formWidth = parentForm.ClientSize.Width;

            foreach (var stone in stones)
            {
                if (currentLevel != 3)
                {
                    stone.Update(); // Uses standard Movement (VerticalPatrolMovement)
                }
                else
                {
                    // For Physics stones, we only sync the sprite to the body
                    stone.Sprite.Left = (int)stone.Body.Position.X;
                    stone.Sprite.Top = (int)stone.Body.Position.Y;
                }

                // Recycle: If stone goes below screen
                if (stone.Sprite.Top > formHeight)
                {
                    // Reset to top with new random X
                    int newX = rnd.Next(50, formWidth - 50);
                    stone.Body.Position = new PointF(newX, rnd.Next(-100, -10));
                    stone.Body.Velocity = PointF.Empty; // Reset momentum for physics stones
                    stone.Sprite.Top = (int)stone.Body.Position.Y;
                    stone.Sprite.Left = (int)stone.Body.Position.X;
                }

                // Collision with Player
                if (playerBounds.IntersectsWith(stone.Sprite.Bounds))
                {
                    onHit?.Invoke();
                    // Optional: Reset this specific stone so it doesn't hit again immediately
                    stone.Body.Position = new PointF(stone.Body.Position.X, -100);
                }
            }
        }

        private void UpdateChaser(Rectangle playerBounds, Action onHit)
        {
            if (chaser == null) return;

            // Update logical position
            chaser.Update(new GameTime { DeltaTime = 0.02f }); // approx for 20ms interval
            
            // Sync Sprite
            chaserSprite.Left = (int)chaser.Position.X;
            chaserSprite.Top = (int)chaser.Position.Y;
            if (chaser.Sprite != null) chaserSprite.Image = chaser.Sprite;

            // Collision
            if (playerBounds.IntersectsWith(chaserSprite.Bounds))
            {
                onHit?.Invoke();
                // Optional: Pushback or reset chaser to give player a chance
                chaser.Position = new PointF(parentForm.ClientSize.Width - 50, chaser.Position.Y);
            }
        }

        public void HandleResize(int formWidth, int formHeight, int initialWidth, int initialHeight, int currentLevel)
        {
            // Coins 
            foreach (var coin in coins)
            {
                float xRatio = coin.Body.Position.X / initialWidth;
                float yRatio = coin.Body.Position.Y / initialHeight;
                coin.Sprite.Left = (int)(xRatio * formWidth);
                coin.Sprite.Top = (int)(yRatio * formHeight);
                coin.Body.Position = new PointF(coin.Sprite.Left, coin.Sprite.Top);
                
                // Update horizontal patrol bounds
                coin.Movement = new HorizontalPatrolMovement(Math.Max(0, coin.Sprite.Left - 50), Math.Min(formWidth - coin.Sprite.Width, coin.Sprite.Left + 50));
            }

            // Obstacles
            foreach (var obs in obstacles)
            {
                float xRatio = obs.Sprite.Left / (float)initialWidth;
                float yRatio = obs.Sprite.Top / (float)initialHeight;
                obs.Sprite.Left = (int)(xRatio * formWidth);
                obs.Sprite.Top = (int)(yRatio * formHeight);
                obs.Body.Position = new PointF(obs.Sprite.Left, obs.Sprite.Top);
                
                // Update vertical patrol bounds WITH LEVEL-BASED SPEED
                float speed = currentLevel == 1 ? 4f : (currentLevel == 2 ? 7f : 10f); // medium speed for lvl 2
                obs.Movement = new VerticalPatrolMovement(0, formHeight - obs.Sprite.Height, speed);
            }
        }
    }
}
