using System;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class FeedbackForm : Form
    {
        private System.Windows.Forms.Timer closeTimer;
        public bool Drank { get; private set; } = false;

        public FeedbackForm(int timeoutSeconds)
        {
            InitializeComponent();
            closeTimer = new System.Windows.Forms.Timer { Interval = timeoutSeconds * 1000 };
            closeTimer.Tick += (s, e) => { closeTimer.Stop(); this.DialogResult = DialogResult.Cancel; this.Close(); };
            closeTimer.Start();
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            Drank = true;
            closeTimer.Stop();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            Drank = false;
            closeTimer.Stop();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
