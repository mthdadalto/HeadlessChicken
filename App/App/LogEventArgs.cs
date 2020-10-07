using System;

namespace App.Events
{
    public enum LogType
    {
        External,
        General,
        Debug,
        TriggerBox,
        Middleware,
        DataUploader,
        Camera, 
        DeviceLayer
    }

    public class LogEventArgs : EventArgs
    {
        public LogEventArgs() { }

        public LogEventArgs(string message) { Message = message; }
        //public LogEventArgs(string message, string stackTrace) { Message = message; Exception = stackTrace; }

        public LogEventArgs(Exception ex) { Message = ex.Message; Exception = ex; }
        public LogEventArgs(string message, Exception ex) { Message = message + Environment.NewLine + ex.Message; Exception = ex; }

        public string Message { get; set; }
        public Exception Exception = null;
        public LogType LogType = LogType.General;
        public DateTime Date = DateTime.UtcNow;
        public bool IsDebug = false;
        public bool ShouldSave = true;

        public string Time { get => Date.ToString("HH:mm:ss"); }

        public string DateShort { get => Date.ToString("yyyy-MM-dd"); }

        public string DateFull { get => Date.ToString("yyyy-MM-dd HH:mm:ss"); }

        public override string ToString() { return Message; }
    }
}
