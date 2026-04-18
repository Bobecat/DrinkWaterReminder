using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private System.Timers.Timer reminderTimer;
        private readonly int intervalMinutes = 40; // 半小时
        private int totalWaterMl = 0;
        private int dailyGoalMl = 2000;
        private DateTime nextReminder;
        private System.Windows.Forms.Timer uiTimer;
        private System.Windows.Forms.Timer responseTimer;
        private bool exitRequested = false;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip trayMenu;

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            reminderTimer = new System.Timers.Timer(intervalMinutes * 60 * 1000);
            reminderTimer.AutoReset = true;
            reminderTimer.Elapsed += ReminderTimer_Elapsed;

            // 初始化 UI 状态
            LoadTotalFromStorage();
            UpdateTotalLabel();
            UpdateProgress();

            // 启动时自动开始提醒，并设置下一次提醒时间
            nextReminder = DateTime.Now.AddMinutes(intervalMinutes);
            reminderTimer.Start();
            // 立即更新倒计时显示
            UpdateNextLabel();
            uiTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            uiTimer.Tick += (s, e) => UpdateNextLabel();
            uiTimer.Start();

            // 响应超时定时器（2分钟），用于提醒时用户在主界面上反馈
            responseTimer = new System.Windows.Forms.Timer { Interval = 2 * 60 * 1000 };
            responseTimer.Tick += (s, e) => OnResponseTimeout();

            // 初始化托盘图标和菜单（使程序可独立在后台运行）
            trayMenu = new System.Windows.Forms.ContextMenuStrip();
            trayMenu.Items.Add("显示", null, (s, e) => { ShowMainWindow(); });
            trayMenu.Items.Add("退出", null, (s, e) => { ExitApp(); });

            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = System.Drawing.SystemIcons.Application;
            notifyIcon.Visible = true;
            notifyIcon.Text = "喝水提醒";
            notifyIcon.ContextMenuStrip = trayMenu;
            notifyIcon.DoubleClick += (s, e) => ShowMainWindow();

            // 启动时最小化到托盘，独立运行
            //this.WindowState = FormWindowState.Minimized;
            //this.ShowInTaskbar = false;
            //this.Hide();

            // 确保倒计时标签可见（恢复到主窗口时能正确显示）
            try { lblNext.Visible = true; } catch { }

            // 初始化界面反馈按钮状态
            try { btnDrank.Visible = false; btnDrank.Enabled = false; btnNotDrank.Visible = false; btnNotDrank.Enabled = false; } catch { }
        }

        // 开始/停止按钮已移除，相关逻辑由程序自动管理
        private void UpdateNextLabel()
        {
            try
            {
                if (reminderTimer.Enabled)
                {
                    var now = DateTime.Now;
                    var remaining = nextReminder - now;
                    if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;
                    lblNext.Text = "下次提醒：" + remaining.ToString(@"hh\:mm\:ss");
                }
                else
                {
                    lblNext.Text = "提醒已停止";
                }
            }
            catch { }
        }

        private async void ReminderTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 停止计时器，确保后续逻辑执行期间不再触发
            reminderTimer.Stop();
            this.Invoke(() => UpdateNextLabel());

            // 中午休息时间 12:40 - 13:30 不提醒
            var now = DateTime.Now;
            var startBreak = new TimeSpan(12, 40, 0);
            var endBreak = new TimeSpan(13, 30, 0);
            if (now.TimeOfDay >= startBreak && now.TimeOfDay <= endBreak)
            {
                // 跳过本次提醒，下一次安排在午休结束后再过 intervalMinutes
                nextReminder = DateTime.Today.Add(endBreak).AddMinutes(intervalMinutes);
                reminderTimer.Start();
                this.Invoke(() => UpdateNextLabel());
                return;
            }

            try
            {
                // 最小化所有窗口
                try { WindowHelper.MinimizeAllWindows(); } catch { }

                // 设置音量到30%
                try { AudioHelper.SetMasterVolume(100); } catch { }



                // 弹出主窗口，让程序弹出以便用户反馈；在主窗口显示反馈按钮（非弹窗）
                try
                {
                    this.Invoke(() =>
                    {
                        // 显示主窗口并置顶一次
                        ShowMainWindow();
                        this.TopMost = true;
                        this.BringToFront();
                        this.TopMost = false;
                    });
                    // 在主窗口上显示反馈按钮并启动响应超时计时器
                    this.Invoke(() =>
                    {
                        if (btnDrank != null && btnNotDrank != null)
                        {
                            btnDrank.Visible = true;
                            btnDrank.Enabled = true;
                            btnNotDrank.Visible = true;
                            btnNotDrank.Enabled = true;
                        }
                        try { responseTimer.Stop(); responseTimer.Start(); } catch { }
                    });
                }
                catch { }

                // 播放提示音 10s
                try { await AudioHelper.PlayNotificationSoundAsync(5000); } catch { }

                try { AudioHelper.SetMasterVolume(0); } catch { }
            }
            finally
            {
                // 安排并启动下一次提醒，确保无论如何都会继续提醒
                nextReminder = DateTime.Now.AddMinutes(intervalMinutes);
                reminderTimer.Start();
                this.Invoke(() => UpdateNextLabel());
            }
        }

        private void OnResponseTimeout()
        {
            try
            {
                responseTimer.Stop();
                // 视为没有喝水，隐藏按钮并最小化到托盘
                this.Invoke(() =>
                {
                    if (btnDrank != null && btnNotDrank != null)
                    {
                        btnDrank.Enabled = false;
                        btnDrank.Visible = false;
                        btnNotDrank.Enabled = false;
                        btnNotDrank.Visible = false;
                    }
                    this.WindowState = FormWindowState.Minimized;
                    this.ShowInTaskbar = false;
                    this.Hide();
                });
            }
            catch { }
        }

        private void btnDrank_Click(object sender, EventArgs e)
        {
            try
            {
                responseTimer.Stop();
            }
            catch { }
            totalWaterMl += 300;
            UpdateTotalLabel();
            UpdateProgress();
            SaveTotalToStorage();
            // 隐藏按钮并最小化到托盘
            try
            {
                btnDrank.Enabled = false; btnDrank.Visible = false;
                btnNotDrank.Enabled = false; btnNotDrank.Visible = false;
            }
            catch { }
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();
            // 保存当前计数
            SaveTotalToStorage();
        }

        private void btnNotDrank_Click(object sender, EventArgs e)
        {
            try
            {
                responseTimer.Stop();
            }
            catch { }
            // 不增加水量，隐藏按钮
            try
            {
                btnDrank.Enabled = false; btnDrank.Visible = false;
                btnNotDrank.Enabled = false; btnNotDrank.Visible = false;
            }
            catch { }
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();
        }

        private void UpdateTotalLabel()
        {
            lblTotal.Text = $"今日累计：{totalWaterMl} ml";
        }

        private void UpdateProgress()
        {
            try
            {
                if (pbProgress != null)
                {
                    int value = Math.Min(totalWaterMl, dailyGoalMl);
                    pbProgress.Maximum = dailyGoalMl > 0 ? dailyGoalMl : 1000;
                    pbProgress.Value = Math.Min(value, pbProgress.Maximum);
                }
                lblGoal.Text = $"目标：{dailyGoalMl} ml";
            }
            catch { }
        }

        private string storageFile => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "water_total.txt");

        private void LoadTotalFromStorage()
        {
            try
            {
                var path = storageFile;
                if (System.IO.File.Exists(path))
                {
                    var text = System.IO.File.ReadAllText(path);
                    if (int.TryParse(text, out var v))
                    {
                        // 如果不是今天的数据，则重置（简单实现：文件仅存当日数值）
                        totalWaterMl = v;
                    }
                }
            }
            catch { }
        }

        private void SaveTotalToStorage()
        {
            try
            {
                var path = storageFile;
                System.IO.File.WriteAllText(path, totalWaterMl.ToString());
            }
            catch { }
        }

        private void ShowMainWindow()
        {
            this.Invoke(() =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.BringToFront();
            });
        }

        private void ExitApp()
        {
            exitRequested = true;
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Application.Exit();
            File.Delete(storageFile);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!exitRequested)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                this.Hide();
                notifyIcon.ShowBalloonTip(1000, "喝水提醒", "程序已最小化到托盘，仍在运行。", System.Windows.Forms.ToolTipIcon.Info);
            }
            else
            {
                base.OnFormClosing(e);
            }
        }

        private void lblNext_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    static class WindowHelper
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const byte VK_LWIN = 0x5B;
        private const byte VK_M = 0x4D;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        public static void MinimizeAllWindows()
        {
            // 发送 Win+M
            keybd_event(VK_LWIN, 0, 0, UIntPtr.Zero);
            keybd_event(VK_M, 0, 0, UIntPtr.Zero);
            keybd_event(VK_M, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }

    static class AudioHelper
    {
        public static void SetMasterVolume(int percent)
        {
            try
            {
                // 使用 NAudio CoreAudioApi
                var enumerator = new NAudio.CoreAudioApi.MMDeviceEnumerator();
                var device = enumerator.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.Role.Multimedia);
                device.AudioEndpointVolume.MasterVolumeLevelScalar = Math.Max(0, Math.Min(1, percent / 100f));
            }
            catch
            {
                // ignore
            }
        }

        public static Task PlayNotificationSoundAsync(int milliseconds)
        {
            return Task.Run(() =>
            {
                // 使用系统提示音别名异步播放，然后等待并停止
                NativeMethods.PlaySound("SystemAsterisk", IntPtr.Zero, NativeMethods.SND_ALIAS | NativeMethods.SND_ASYNC);
                Task.Delay(milliseconds).Wait();
                NativeMethods.PlaySound(null, IntPtr.Zero, 0);
            });
        }
    }

    static class NativeMethods
    {
        public const uint SND_ALIAS = 0x00010000;
        public const uint SND_ASYNC = 0x0001;

        [System.Runtime.InteropServices.DllImport("winmm.dll", SetLastError = true)]
        public static extern bool PlaySound(string pszSound, IntPtr hmod, uint fdwSound);
    }
}
