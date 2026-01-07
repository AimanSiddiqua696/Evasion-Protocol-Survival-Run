namespace SemiFinalGame
{
    partial class GameOver
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameOver));
            btnTryAgain = new Button();
            btnExit = new Button();
            lblScore = new Label();
            lblCoins = new Label();
            lblLives = new Label();
            lblScoreTitle = new Label();
            lblCoinsTitle = new Label();
            lblLivesTitle = new Label();
            label1 = new Label();
            SuspendLayout();
            // 
            // btnTryAgain
            // 
            btnTryAgain.BackColor = SystemColors.GradientInactiveCaption;
            btnTryAgain.FlatAppearance.BorderSize = 0;
            btnTryAgain.FlatStyle = FlatStyle.Flat;
            btnTryAgain.Font = new Font("Century", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnTryAgain.ForeColor = Color.Black;
            btnTryAgain.Location = new Point(417, 673);
            btnTryAgain.Margin = new Padding(5, 6, 5, 6);
            btnTryAgain.Name = "btnTryAgain";
            btnTryAgain.Size = new Size(267, 115);
            btnTryAgain.TabIndex = 1;
            btnTryAgain.Text = "Try Again";
            btnTryAgain.UseVisualStyleBackColor = false;
            btnTryAgain.Click += btnTryAgain_Click;
            // 
            // btnExit
            // 
            btnExit.BackColor = SystemColors.GradientInactiveCaption;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Font = new Font("Century", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExit.ForeColor = Color.Black;
            btnExit.Location = new Point(717, 673);
            btnExit.Margin = new Padding(5, 6, 5, 6);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(267, 115);
            btnExit.TabIndex = 2;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            // 
            // lblScore
            // 
            lblScore.AutoSize = true;
            lblScore.BackColor = Color.Transparent;
            lblScore.Font = new Font("Arial", 20F, FontStyle.Bold);
            lblScore.ForeColor = Color.White;
            lblScore.Location = new Point(667, 385);
            lblScore.Margin = new Padding(5, 0, 5, 0);
            lblScore.Name = "lblScore";
            lblScore.Size = new Size(42, 46);
            lblScore.TabIndex = 3;
            lblScore.Text = "0";
            // 
            // lblCoins
            // 
            lblCoins.AutoSize = true;
            lblCoins.BackColor = Color.Transparent;
            lblCoins.Font = new Font("Arial", 20F, FontStyle.Bold);
            lblCoins.ForeColor = Color.White;
            lblCoins.Location = new Point(667, 481);
            lblCoins.Margin = new Padding(5, 0, 5, 0);
            lblCoins.Name = "lblCoins";
            lblCoins.Size = new Size(42, 46);
            lblCoins.TabIndex = 4;
            lblCoins.Text = "0";
            // 
            // lblLives
            // 
            lblLives.AutoSize = true;
            lblLives.BackColor = Color.Transparent;
            lblLives.Font = new Font("Arial", 20F, FontStyle.Bold);
            lblLives.ForeColor = Color.White;
            lblLives.Location = new Point(667, 577);
            lblLives.Margin = new Padding(5, 0, 5, 0);
            lblLives.Name = "lblLives";
            lblLives.Size = new Size(42, 46);
            lblLives.TabIndex = 5;
            lblLives.Text = "0";
            // 
            // lblScoreTitle
            // 
            lblScoreTitle.AutoSize = true;
            lblScoreTitle.BackColor = Color.Transparent;
            lblScoreTitle.Font = new Font("Arial Black", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblScoreTitle.ForeColor = SystemColors.GradientInactiveCaption;
            lblScoreTitle.Location = new Point(333, 385);
            lblScoreTitle.Margin = new Padding(5, 0, 5, 0);
            lblScoreTitle.Name = "lblScoreTitle";
            lblScoreTitle.Size = new Size(350, 56);
            lblScoreTitle.TabIndex = 6;
            lblScoreTitle.Text = "FINAL SCORE :";
            // 
            // lblCoinsTitle
            // 
            lblCoinsTitle.AutoSize = true;
            lblCoinsTitle.BackColor = Color.Transparent;
            lblCoinsTitle.Font = new Font("Arial Black", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCoinsTitle.ForeColor = SystemColors.GradientInactiveCaption;
            lblCoinsTitle.Location = new Point(333, 481);
            lblCoinsTitle.Margin = new Padding(5, 0, 5, 0);
            lblCoinsTitle.Name = "lblCoinsTitle";
            lblCoinsTitle.Size = new Size(472, 56);
            lblCoinsTitle.TabIndex = 7;
            lblCoinsTitle.Text = "COINS COLLECTED :";
            // 
            // lblLivesTitle
            // 
            lblLivesTitle.AutoSize = true;
            lblLivesTitle.BackColor = Color.Transparent;
            lblLivesTitle.Font = new Font("Arial Black", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLivesTitle.ForeColor = SystemColors.GradientInactiveCaption;
            lblLivesTitle.Location = new Point(333, 577);
            lblLivesTitle.Margin = new Padding(5, 0, 5, 0);
            lblLivesTitle.Name = "lblLivesTitle";
            lblLivesTitle.Size = new Size(455, 56);
            lblLivesTitle.TabIndex = 8;
            lblLivesTitle.Text = "LIVES REMAINING :";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Berlin Sans FB Demi", 96F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.GradientInactiveCaption;
            label1.Location = new Point(419, 128);
            label1.Name = "label1";
            label1.Size = new Size(1172, 216);
            label1.TabIndex = 9;
            label1.Text = "GAME OVER";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // GameOver
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1333, 865);
            Controls.Add(label1);
            Controls.Add(lblLivesTitle);
            Controls.Add(lblCoinsTitle);
            Controls.Add(lblScoreTitle);
            Controls.Add(lblLives);
            Controls.Add(lblCoins);
            Controls.Add(lblScore);
            Controls.Add(btnExit);
            Controls.Add(btnTryAgain);
            Margin = new Padding(5, 6, 5, 6);
            Name = "GameOver";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Game Over";
            ResumeLayout(false);
            PerformLayout();

        }

        private System.Windows.Forms.Button btnTryAgain;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.Label lblCoins;
        private System.Windows.Forms.Label lblLives;
        private System.Windows.Forms.Label lblScoreTitle;
        private System.Windows.Forms.Label lblCoinsTitle;
        private System.Windows.Forms.Label lblLivesTitle;

        #endregion

        private Label label1;
    }
}