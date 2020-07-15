using System;
using System.IO;

namespace DrDocx.API
{
    internal static class Paths
    {
        public static string ApiHostUrl => "localhost";
        public static int ApiHostPort => 1211;
        public static string ApiFullUrl => $"http://{ApiHostUrl}:{ApiHostPort}";
        public static string WorkingDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DrDocx");
        public static string RelativeDbDir => "db";
        public static string RelativeLogsDir => "logs";
        public static string RelativeTemplatesDir => "templates";
        public static string RelativeTmpDir => "tmp";
        public static string RelativeReportsDir => "reports";
		public static string RelativeFieldGroupsDir => "fieldGroups";
		public static string RelativeFieldGroupsImportDir => Path.Combine(RelativeFieldGroupsDir,"import");
		public static string RelativeFieldGroupsExportDir => Path.Combine(RelativeFieldGroupsDir,"export");
        public static string DbPath => Path.Combine(RelativeDbDir, "DrDocx.db");

        public static void EnsureDirsCreated()
        {
            CreateDirIfMissing(WorkingDirectory);
            Environment.CurrentDirectory = WorkingDirectory;
            CreateDirIfMissing(RelativeDbDir);
            CreateDirIfMissing(RelativeLogsDir);
            CreateDirIfMissing(RelativeTemplatesDir);
            CreateDirIfMissing(RelativeTmpDir);
            CreateDirIfMissing(RelativeReportsDir);
			CreateDirIfMissing(RelativeFieldGroupsDir);
			CreateDirIfMissing(RelativeFieldGroupsExportDir);
			CreateDirIfMissing(RelativeFieldGroupsImportDir);
        }

        private static void CreateDirIfMissing(string dirPath)
        {
            if (Directory.Exists(dirPath)) return;
            Directory.CreateDirectory(dirPath);
            NLog.LogManager.GetCurrentClassLogger().Info($"{dirPath} not found, created new one.");
        }
        
        
    }
}