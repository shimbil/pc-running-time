using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PCRunningTime
{
    public partial class MainForm : Form
    {
        private DateTime startTime = DateTime.Now;
        private NotifyIcon notifyIcon;
        private Timer tooltipTimer;
        private Timer reminderTimer; 
        private DateTime? idleStartTime = null;
        private string idleText = "";

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        public MainForm()
        {
            InitializeComponent();
            AddToStartup();
            DelayedSetupTrayIcon();
            SetupTimers();
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        private void AddToStartup()
        {
            string appName = "PCRunningTime";
            string exePath = Application.ExecutablePath;
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            object existingValue = regKey.GetValue(appName);

            if (existingValue == null || existingValue.ToString() != exePath)
            {
                regKey.SetValue(appName, exePath);
            }
        }

        private void DelayedSetupTrayIcon()
        {
            new System.Threading.Timer(_ =>
            {
                this.Invoke((MethodInvoker)(() => SetupTrayIcon()));
            }, null, 500, System.Threading.Timeout.Infinite);
        }

        private void SetupTrayIcon()
        {
            notifyIcon = new NotifyIcon();

            // Load custom icon
            try
            {
                string iconPath = Path.Combine(Application.StartupPath, "app_icon.ico");
                notifyIcon.Icon = new Icon(iconPath);
            }
            catch
            {
                notifyIcon.Icon = SystemIcons.Application;
            }

            notifyIcon.Visible = true;
            notifyIcon.Text = "PC Running Time";

            notifyIcon.DoubleClick += (s, e) =>
            {
                TimeSpan uptime = DateTime.Now - startTime;
                MessageBox.Show(
                    $"⏰ PC Running: {uptime.Days}days {uptime.Hours}hour {uptime.Minutes}min {uptime.Seconds}sec\n" +
                    $"⏰ PC Start Time: {startTime.ToString("hh:mm tt")}\n{idleText}",
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // Context menu
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            contextMenu.Items.Add("About", null, (s, e) =>
            {
                MessageBox.Show(
                    "🖥️ PCRunningTime silently monitors your PC's active duration and idle time from the system tray. " +
                    "It auto-starts with Windows and gently reminds you to take breaks for a healthier workflow.\n\n" +
                    "PC Running Time\nVersion 1.0.0\n\n" +
                    "Developed by Al Shimbil Khan\n" +
                    "(+88 01516711976, shimbilmax@gmail.com)\n\n" +
                    "Made with ❤️",
                    "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });

            contextMenu.Items.Add("Show Running Time", null, (s, e) =>
            {
                TimeSpan uptime = DateTime.Now - startTime;
                MessageBox.Show(
                    $"⏰ PC Running: {uptime.Days}days {uptime.Hours}hour {uptime.Minutes}min {uptime.Seconds}sec\n" +
                    $"⏰ PC Start Time: {startTime.ToString("hh:mm tt")}\n{idleText}",
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });

            contextMenu.Items.Add("Exit", null, (s, e) => Application.Exit());

            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void SetupTimers()
        {
            // Auto-refresh Tooltip Timer
            tooltipTimer = new Timer();
            tooltipTimer.Interval = 1000; // 1 second
            tooltipTimer.Tick += (s, e) =>
            {
                TimeSpan uptime = DateTime.Now - startTime;

                // Detect idle time
                uint idleMillis = GetIdleTime();
                if (idleMillis >= 5 * 60 * 1000) // 5 minutes
                {
                    if (idleStartTime == null)
                    {
                        idleStartTime = DateTime.Now.AddMilliseconds(-idleMillis);  // first time
                    }
                    int idleMinutes = (int)((DateTime.Now - idleStartTime.Value).TotalMinutes);
                    idleText = $"💤 Total Idle State Tracked since {idleStartTime.Value:hh:mm tt} — {idleMinutes} min";
                }
                else
                {
                    idleStartTime = null;
                }

                notifyIcon.Text = $"PC Running: {uptime.Days}days {uptime.Hours}hour {uptime.Minutes}min {uptime.Seconds}sec\n⏰ Double click for details.";
            };
            tooltipTimer.Start();

            // Reminder Timer (after 1 hours)
            reminderTimer = new Timer();
            reminderTimer.Interval = 1 * 60 * 60 * 1000; // 1 hours
            reminderTimer.Tick += (s, e) =>
            {
                notifyIcon.BalloonTipTitle = "Friendly Reminder";
                notifyIcon.BalloonTipText = "⏰ You've been running your PC for 1 hours. Maybe take a short break!";
                notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon.ShowBalloonTip(1 * 60* 1000);    //1 min
            };
            reminderTimer.Start();
        }

        private uint GetIdleTime()
        {
            LASTINPUTINFO lastInput = new LASTINPUTINFO();
            lastInput.cbSize = (uint)Marshal.SizeOf(lastInput);
            if (GetLastInputInfo(ref lastInput))
            {
                return ((uint)Environment.TickCount - lastInput.dwTime);
            }
            return 0;
        }
    }
}
