using System;
using System.IO;

namespace DrDocx.API
{
    public static class Paths
    {
        public static string ApiHostUrl => "localhost";
        public static int ApiHostPort => 1211;
        public static string WorkingDirectory => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/DrDocx";
        public static string RelativeDbDir => "db";
        public static string RelativeLogsDir => "logs";
        public static string RelativeTemplatesDir => "templates";
        public static string RelativeTmpDir => "tmp";
        public static string DbPath => $"{RelativeDbDir}/DrDocx.db";

        public static void EnsureDirsCreated()
        {
            CreateDirIfMissing(WorkingDirectory);
            Environment.CurrentDirectory = WorkingDirectory;
            CreateDirIfMissing(RelativeDbDir);
            CreateDirIfMissing(RelativeLogsDir);
            CreateDirIfMissing(RelativeTemplatesDir);
            CreateDirIfMissing(RelativeTmpDir);
        }

        private static void CreateDirIfMissing(string dirPath)
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }
    }
}