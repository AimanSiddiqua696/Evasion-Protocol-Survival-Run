using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SemiFinalGame
{
    public partial class VictoryForm : Form
    {
        public bool GoToNextLevel { get; private set; } = false;
        private int completedLevel;

        public VictoryForm(int score, int coins, int lives, int level)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.completedLevel = level;

            lblScore.Text = score.ToString();
            lblCoins.Text = coins.ToString();
            lblLives.Text = lives.ToString();

            // Logic for Level 2 Eligibility
            // Logic for Level Eligibility
            if (completedLevel < 3) // Assuming 3 is the max level for now
            {
                lblMessage.Visible = true;
                btnLevel2.Visible = true;
                btnLevel2.Text = "Level " + (completedLevel + 1); // Dynamic button text
                btnPlayAgain.Text = "Restart Level " + completedLevel; 
            }
            else
            {
                // ALL Levels Completed
                lblMessage.Visible = true;
                btnLevel2.Visible = false;
                btnPlayAgain.Text = "Play Again";
            }
            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateLayout();
            // Play Victory Sound
            SemiFinalGame.Sound.SoundManager.PlayMusic(Properties.Resources.VictoryFormsound);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            SemiFinalGame.Sound.SoundManager.StopMusic();
        }

        private void UpdateLayout()
        {
            int centerX = this.ClientSize.Width / 2;
            int centerY = this.ClientSize.Height / 2;

            // Center Victory Title
            if (label1 != null)
            {
                label1.Left = (this.ClientSize.Width - label1.Width) / 2;
                label1.Top = centerY - label1.Height - 120; // Slightly above center
            }

            // Align all numbers to the same X coordinate, slightly to the right of the center
            int labelX = centerX + 20; 

            // Adjust Y coordinates to match the lines on the background image
            // "FINAL SCORE" line
            if (lblScore != null) lblScore.Location = new Point(labelX, centerY - 25);
            if (lblScoreTitle != null) lblScoreTitle.Location = new Point(labelX - lblScoreTitle.Width - 10, centerY - 25);
            
            // "COINS COLLECTED" line
            if (lblCoins != null) lblCoins.Location = new Point(labelX, centerY + 25);
            if (lblCoinsTitle != null) lblCoinsTitle.Location = new Point(labelX - lblCoinsTitle.Width - 10, centerY + 25);
            
            // "LIVES REMAINING" line
            if (lblLives != null) lblLives.Location = new Point(labelX, centerY + 75);
            if (lblLivesTitle != null) lblLivesTitle.Location = new Point(labelX - lblLivesTitle.Width - 10, centerY + 75);

            if (btnPlayAgain != null && btnExit != null)
            {
                int gap = 20;
                // If Level 2 button is visible, include it in layout
                int totalWidth;
                bool isLevel2Visible = (btnLevel2 != null && btnLevel2.Visible);

                if (isLevel2Visible)
                {
                    totalWidth = btnPlayAgain.Width + btnLevel2.Width + btnExit.Width + (gap * 2);
                }
                else
                {
                    totalWidth = btnPlayAgain.Width + btnExit.Width + gap;
                }

                int startX = (this.ClientSize.Width - totalWidth) / 2;
                int y = this.ClientSize.Height - btnPlayAgain.Height - 50; 

                btnPlayAgain.Location = new Point(startX, y);
                
                if (isLevel2Visible)
                {
                    // Order: PlayAgain -> Exit -> Level2
                    btnExit.Location = new Point(startX + btnPlayAgain.Width + gap, y);
                    btnLevel2.Location = new Point(startX + btnPlayAgain.Width + btnExit.Width + (gap * 2), y);
                }
                else
                {
                    btnExit.Location = new Point(startX + btnPlayAgain.Width + gap, y);
                }
            }

            if (lblMessage != null)
            {
                // Positioned below the VICTORY ACHIEVED title
                lblMessage.Location = new Point((this.ClientSize.Width - lblMessage.Width) / 2, label1.Bottom + 5);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        private void btnPlayAgain_Click(object sender, EventArgs e)
        {
            SemiFinalGame.Sound.SoundManager.PlaySoundEffect(Properties.Resources.computer_mouse_click_02_383961__1_);
            GoToNextLevel = false; // Just restart
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnLevel2_Click(object sender, EventArgs e)
        {
            SemiFinalGame.Sound.SoundManager.PlaySoundEffect(Properties.Resources.computer_mouse_click_02_383961__1_);
            GoToNextLevel = true; // Signal to GameForm to increment level
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            SemiFinalGame.Sound.SoundManager.PlaySoundEffect(Properties.Resources.computer_mouse_click_02_383961__1_);
            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
