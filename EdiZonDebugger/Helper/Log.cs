using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace EdiZonDebugger.Helper
{
    public enum LogLevel
    {
        INFO,
        LUA,
        WARNING,
        ERROR,
        FATAL
    }

    public class LogConsole
    {
        private static readonly Lazy<LogConsole> _lazy = new Lazy<LogConsole>(() => new LogConsole());

        public static LogConsole Instance { get { return _lazy.Value; } }

        public static RichTextBox LogBox { set { lock (_logBox) _logBox = value; } }
        private static RichTextBox _logBox = new RichTextBox();

        public void Log(string message, LogLevel level)
        {
            lock (_logBox)
                switch (level)
                {
                    case LogLevel.INFO:
                        _logBox.AppendText("[INFO] " + message + "\n", Color.White);
                        break;
                    case LogLevel.LUA:
                        _logBox.AppendText("[LUA] " + message + "\n", Color.LightBlue);
                        break;
                    case LogLevel.WARNING:
                        _logBox.AppendText("[WARNING] " + message + "\n", Color.Orange);
                        break;
                    case LogLevel.ERROR:
                        _logBox.AppendText("[ERROR] " + message + "\n", Color.IndianRed);
                        break;
                    case LogLevel.FATAL:
                        _logBox.AppendText("[FATAL] " + message + "\n", Color.Red);
                        break;
                }
        }
    }
}
