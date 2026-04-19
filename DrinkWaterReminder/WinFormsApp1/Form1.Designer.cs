namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                try { appIcon?.Dispose(); } catch { }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblNext = new Label();
            lblGoal = new Label();
            pbProgress = new ProgressBar();
            btnDrank = new Button();
            btnNotDrank = new Button();
            lblTotal = new Label();
            SuspendLayout();
            // 
            // lblNext
            // 
            lblNext.Font = new Font("Segoe UI", 28F);
            lblNext.ForeColor = Color.FromArgb(0, 204, 209);
            lblNext.Location = new Point(12, -3);
            lblNext.Name = "lblNext";
            lblNext.Size = new Size(367, 106);
            lblNext.TabIndex = 3;
            lblNext.Text = "下次提醒：00:00:00";
            lblNext.TextAlign = ContentAlignment.MiddleCenter;
            lblNext.Click += lblNext_Click;
            // 
            // lblGoal
            // 
            lblGoal.AutoSize = true;
            lblGoal.Location = new Point(32, 165);
            lblGoal.Name = "lblGoal";
            lblGoal.Size = new Size(90, 17);
            lblGoal.TabIndex = 2;
            lblGoal.Text = "目标：2000 ml";
            // 
            // pbProgress
            // 
            pbProgress.Location = new Point(32, 185);
            pbProgress.Name = "pbProgress";
            pbProgress.Size = new Size(336, 22);
            pbProgress.TabIndex = 3;
            // 
            // btnDrank
            // 
            btnDrank.Location = new Point(46, 106);
            btnDrank.Name = "btnDrank";
            btnDrank.Size = new Size(120, 44);
            btnDrank.TabIndex = 5;
            btnDrank.Text = "我已喝水";
            btnDrank.UseVisualStyleBackColor = true;
            btnDrank.Visible = false;
            btnDrank.Click += btnDrank_Click;
            // 
            // btnNotDrank
            // 
            btnNotDrank.Location = new Point(208, 106);
            btnNotDrank.Name = "btnNotDrank";
            btnNotDrank.Size = new Size(120, 44);
            btnNotDrank.TabIndex = 6;
            btnNotDrank.Text = "未喝";
            btnNotDrank.UseVisualStyleBackColor = true;
            btnNotDrank.Visible = false;
            btnNotDrank.Click += btnNotDrank_Click;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(32, 220);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(93, 17);
            lblTotal.TabIndex = 4;
            lblTotal.Text = "今日累计：0 ml";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(394, 246);
            Controls.Add(lblTotal);
            Controls.Add(pbProgress);
            Controls.Add(lblGoal);
            Controls.Add(btnNotDrank);
            Controls.Add(btnDrank);
            Controls.Add(lblNext);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "Form1";
            Text = "喝水提醒";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label lblNext;
        private System.Windows.Forms.Label lblGoal;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnDrank;
        private System.Windows.Forms.Button btnNotDrank;

        #endregion
    }
}
