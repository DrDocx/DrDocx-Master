using System;
using NLog;

namespace DrDocx.API.Helpers
{
    public static class NLogHelper
    {
        public static void ConfigureNLog()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logFile = new NLog.Targets.FileTarget("api-logfile")
            {
                FileName = GetLogFilePath(),
                Layout = "${longdate}\t|\t${level}\t|\t${message} ${exception}"
            };
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logFile);
            LogManager.Configuration = config;
            var logger  = LogManager.GetCurrentClassLogger();
            logger.Info($"Logger successfully configured, now writing to {logFile.FileName}");

        }

        private static string GetLogFilePath()
        {
            const string dateFormat = "yyyy-MM-dd_HH-mm-ss";
            var currentTimeString = DateTime.Now.ToString(dateFormat);
            var logFileName = $"DrDocx-{currentTimeString}.log";
            var logFilePath = $"{Paths.WorkingDirectory}/{Paths.RelativeLogsDir}/{logFileName}";
            return logFilePath;
        }
    }
}