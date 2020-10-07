using System;

using App.Events;

namespace App
{
    /// <summary>
    /// Whole Library debug channel
    /// </summary>
    public class Logger
    {

        public static bool ShouldDebug = false;
        internal Logger() { }

        /// <summary>
        /// Log Messages Event
        /// </summary>
        public static event EventHandler<LogEventArgs> Added = (sender, args) => System.Diagnostics.Debug.WriteLine(args.Message);
        internal static void AddLog(LogEventArgs log) => Added.Invoke(null, log);
        internal static void AddLog(object sender, LogEventArgs log) => Added.Invoke(sender, log);

        internal static void AddLog(string message) => Added.Invoke(null, new LogEventArgs(message));
        internal static void AddLog(Exception exception) => Added.Invoke(null, new LogEventArgs(exception));
        internal static void AddLog(string message, Exception exception) => Added.Invoke(null, new LogEventArgs(message, exception));
        internal static void AddLog(object sender, string message) => Added.Invoke(sender, new LogEventArgs(message));
        internal static void AddLog(object sender, Exception exception) => Added.Invoke(sender, new LogEventArgs(exception));
        internal static void AddLog(object sender, string message, Exception exception) => Added.Invoke(sender, new LogEventArgs(message, exception));

        //Specific

        public static void External(string message, bool isDebug = false) => Added.Invoke(null, new LogEventArgs(message) { LogType = LogType.External, IsDebug = isDebug });
        public static void External(string message, Exception exeption) => Added.Invoke(null, new LogEventArgs(message + "[" + exeption.Message + "]") { LogType = LogType.External, Exception = exeption, IsDebug = true });

        public static void Debug(string message, bool shouldSave = true) => Added.Invoke(null, new LogEventArgs(message){ IsDebug = true, ShouldSave = shouldSave });
        public static void Debug(string message, Exception exeption) => Added.Invoke(null, new LogEventArgs(message + "[" + exeption.Message + "]") { Exception = exeption, IsDebug = true });
        public static void Main(string message, bool shouldSave = true) => Added.Invoke(null, new LogEventArgs(message) { ShouldSave = shouldSave });
        public static void Main(string message, Exception exeption) => Added.Invoke(null, new LogEventArgs(message + "[" + exeption.Message + "]") { Exception = exeption, IsDebug = true });

        public static void Triggerbox(string message, bool shouldSave = true) => Added.Invoke(null, new LogEventArgs(message) { LogType = LogType.TriggerBox, ShouldSave = shouldSave });
        public static void Triggerbox(string message, Exception exeption) => Added.Invoke(null, new LogEventArgs(message + "[" + exeption.Message + "]") { LogType = LogType.TriggerBox, Exception = exeption, IsDebug = true });

        public static void Middleware(string message, bool shouldSave = true) => Added.Invoke(null, new LogEventArgs(message) { LogType = LogType.Middleware, ShouldSave = shouldSave });
        public static void Middleware(string message, Exception exeption) => Added.Invoke(null, new LogEventArgs(message + "[" + exeption.Message + "]") { LogType = LogType.Middleware, Exception = exeption, IsDebug = true });

        public static void Camera(string message, bool shouldSave = true) => Added.Invoke(null, new LogEventArgs(message) { LogType = LogType.Camera, ShouldSave = shouldSave });
        public static void Camera(string message, Exception exeption) => Added.Invoke(null, new LogEventArgs(message + "[" + exeption.Message + "]") { LogType = LogType.Camera, Exception = exeption, IsDebug = true });

        public static void DeviceLayer(string message, bool shouldSave = true) => Added.Invoke(null, new LogEventArgs(message) { LogType = LogType.DeviceLayer, ShouldSave = shouldSave });
        public static void DeviceLayer(string message, Exception exeption) => Added.Invoke(null, new LogEventArgs(message + "[" + exeption.Message + "]") { LogType = LogType.DeviceLayer, Exception = exeption, IsDebug = true });

        public static void DataUploader(string message, bool shouldSave = true) => Added.Invoke(null, new LogEventArgs(message) { LogType = LogType.DataUploader, ShouldSave = shouldSave });
        public static void DataUploader(string message, Exception exeption) => Added.Invoke(null, new LogEventArgs(message + "[" + exeption.Message + "]") { LogType = LogType.DataUploader, Exception = exeption, IsDebug = true });

    }

}
