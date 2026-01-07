using SemiFinalGame.FileHandling;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SemiFinalGame
{
    public partial class MenuForm : Form
    {

        public MenuForm()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Click Sound
            SemiFinalGame.Sound.SoundManager.PlaySoundEffect(Properties.Resources.computer_mouse_click_02_383961__1_);

            // Stop menu music
            SemiFinalGame.Sound.SoundManager.StopMusic();

            // Hide menu
            this.Hide();

            // Show Game Form (Start Level 1)
            using (var gameForm = new GameForm(1))
            {
                gameForm.ShowDialog();
            }

            // Show menu again when game closes
            this.Show();
            
            // Restart menu music
            SemiFinalGame.Sound.SoundManager.PlayMusic(Properties.Resources.MenuFormsound);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // Click Sound - though app might exit too fast to hear it perfectly, it's good practice
            SemiFinalGame.Sound.SoundManager.PlaySoundEffect(Properties.Resources.computer_mouse_click_02_383961__1_);
            Application.Exit();
        }

        private void MenuForm_Load(object sender, EventArgs e)
        {

             // Make full screen like game
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            
            // Play Menu Music
            SemiFinalGame.Sound.SoundManager.PlayMusic(Properties.Resources.MenuFormsound);

            // Center Title (Horizontal) and position slightly above vertical center
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            label1.Top = (this.ClientSize.Height / 2) - label1.Height - 50; // Slightly above center

            // Position Buttons at the bottom center
            int buttonSpacing = 50;
            int totalWidth = btnStart.Width + buttonSpacing + btnExit.Width;
            int startX = (this.ClientSize.Width - totalWidth) / 2;
            int buttonY = this.ClientSize.Height - btnStart.Height - 100;

            btnStart.Location = new Point(startX, buttonY);
            btnExit.Location = new Point(startX + btnStart.Width + buttonSpacing, buttonY);

        }
    }
    }

