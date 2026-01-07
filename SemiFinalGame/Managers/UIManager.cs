using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SemiFinalGame.Properties;

namespace SemiFinalGame.Managers
{
    public class UIManager
    {
        private Form parentForm;
        private Label scoreLabel;
        private Label levelLabel; // Logic Level Indicator
        private Label livesLabel;
        private Label countdownLabel;
        private Panel healthBarBackground;
        private Panel healthBarForeground;

        public float BackgroundX { get; set; } = 0;
        public float BackgroundSpeed { get; set; } = 2.0f; // Positive for Left-to-Right
        public bool IsCurrentTileFlipped { get; set; } = false;
        private Image backgroundImage;
        private Image backgroundImageFlipped; //two images used for seamless looping

        public UIManager(Form form)
        {
            parentForm = form;
            InitializeBackground();
        }

        private void InitializeBackground()
        {
            backgroundImage = Resources.background_still;
            
            // Create the flipped version for seamless tiling
            backgroundImageFlipped = (Image)backgroundImage.Clone();
            backgroundImageFlipped.RotateFlip(RotateFlipType.RotateNoneFlipX);
        }

        public void CreateLabels(int currentLevel)
        {
            // Score Label
            scoreLabel = new Label
            {
                Text = "Score: 0",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.Transparent, // optional
                AutoSize = true,
                Location = new Point(10, 10)
            };
            parentForm.Controls.Add(scoreLabel);
            scoreLabel.BringToFront(); // make sure it's on top of all PictureBoxes

            // Level Label
            levelLabel = new Label
            {
                Text = "Level: " + currentLevel,
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Cyan,
                BackColor = Color.Transparent,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            //levelLabel.Location = new Point(this.ClientSize.Width - 120, 10);
            levelLabel.Location = new Point(parentForm.ClientSize.Width - levelLabel.PreferredWidth - 10, 60);
            parentForm.Controls.Add(levelLabel);
            levelLabel.BringToFront();

            // Lives Label
            livesLabel = new Label
            {
                Text = "Lives: 3",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            // Place near top-right initially
            livesLabel.Location = new Point(parentForm.ClientSize.Width - livesLabel.PreferredWidth - 10, 10);
            parentForm.Controls.Add(livesLabel);
            livesLabel.BringToFront();

            // Countdown Label
            countdownLabel = new Label
            {
                Text = "",
                Font = new Font("Arial", 72, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter, //Aligns text centered inside the label
                Visible = false
            };
            
            // Center it (approximate, will be refined in StartCountdown or Update)
            countdownLabel.Location = new Point(parentForm.ClientSize.Width / 2 - 50, parentForm.ClientSize.Height / 2 - 50);
            //ClientSize → usable area of the form (excluding borders)
            parentForm.Controls.Add(countdownLabel); //Adds the label to the form’s control collection
            
            //Without this, the label would not appear on screen
            countdownLabel.BringToFront();
            // Prevents it from being hidden behind player, enemies, or background
        }

        public void CreateHealthBar()
        {
            // Background (gray)
            healthBarBackground = new Panel
            {
                Size = new Size(150, 20),
                BackColor = Color.Gray,
                Location = new Point(parentForm.ClientSize.Width - 160, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            parentForm.Controls.Add(healthBarBackground);

            // Foreground (red)
            healthBarForeground = new Panel
            {
                Size = healthBarBackground.Size,
                BackColor = Color.Green,
                Location = healthBarBackground.Location,
                Anchor = AnchorStyles.Top | AnchorStyles.Right //This line locks the control to the top and right edges of the form, so its position stays fixed relative to those edges when the window is resized.
            };
            parentForm.Controls.Add(healthBarForeground);
            healthBarForeground.BringToFront();
        }

        public void UpdateHealth(int lives, int maxLives)
        {
            // Update health
            float healthPercentage = (lives / (float)maxLives);
            healthBarForeground.Width = (int)(healthPercentage * healthBarBackground.Width);
            livesLabel.Text = "Lives: " + lives;
        }

        public void UpdateScore(int score)
        {
            scoreLabel.Text = "Score: " + score;
        }

        public void UpdateLevel(int level)
        {
            levelLabel.Text = "Level: " + level;
        }

        public void ShowCountdown(string text)
        {
            countdownLabel.Text = text; //Converts the integer/text into text
            countdownLabel.Visible = true;
            CenterCountdownLabel(); // Recenter
        }

        public void HideCountdown()
        {
            countdownLabel.Visible = false; //Hides the countdown label after it finishes.
        }

        public void CenterCountdownLabel()
        {
            // Keep centered
            countdownLabel.Location = new Point((parentForm.ClientSize.Width - countdownLabel.Width) / 2, (parentForm.ClientSize.Height - countdownLabel.Height) / 2);
        }

        public void UpdateBackground()
        {
            // Move Background Left to Right
            BackgroundX += BackgroundSpeed;
            
            // Reset based on FORM WIDTH (since we stretch the image to form width)
            if (BackgroundX >= parentForm.ClientSize.Width)
            {
                BackgroundX = 0;
                // Toggle the tile type to alternate between Normal and Flipped
                IsCurrentTileFlipped = !IsCurrentTileFlipped;
            }
        }

        public void DrawBackground(Graphics g)
        {
            if (backgroundImage == null || backgroundImageFlipped == null) return;

            // STRETCH Logic to fix "patches"
            int drawWidth = parentForm.ClientSize.Width;
            int drawHeight = parentForm.ClientSize.Height;

            // OPTIMIZATION: Use NearestNeighbor for performance and pixel-perfect look
            // HighQualityBicubic is too slow for large scrolling backgrounds
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.None; // or HighSpeed
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.SmoothingMode = SmoothingMode.HighSpeed;

            // Determine which image is "Current" (leaving right) and "Incoming" (entering from left)
            Image currentImage = IsCurrentTileFlipped ? backgroundImageFlipped : backgroundImage;
            Image incomingImage = IsCurrentTileFlipped ? backgroundImage : backgroundImageFlipped;

            // Convert float to int to prevent sub-pixel rendering gaps
            int x = (int)Math.Round(BackgroundX);

            // 1. Draw current cycle (Moving from 0 to Width)
            // Draw slightly wider (+1) or ensure overlap logic
            g.DrawImage(currentImage, x, 0, drawWidth, drawHeight);
            
            // 2. Draw incoming cycle (Moving from -Width to 0)
            if (x > 0)
            {
                // Overlap by 1 pixel to remove the seam line
                // Position: x - drawWidth + 1 (The +1 moves it 1 pixel to the right, creating overlap)
                g.DrawImage(incomingImage, x - drawWidth + 1, 0, drawWidth, drawHeight);
            }
        }
    }
}
