using Microsoft.Win32;
using System;

namespace WinFormsApp1
{
    static class StartupHelper
    {
        private const string RunKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string AppName = "WinFormsApp1_DrinkWater";

        public static bool IsStartupEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, false);
            if (key == null) return false;
            var val = key.GetValue(AppName);
            return val != null;
        }

        public static void EnableStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, true);
            if (key == null) throw new InvalidOperationException("无法打开注册表路径");
            var exe = System.Reflection.Assembly.GetExecutingAssembly().Location;
            key.SetValue(AppName, "\"" + exe + "\"");
        }

        public static void DisableStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, true);
            if (key == null) return;
            key.DeleteValue(AppName, false);
        }
    }
}
