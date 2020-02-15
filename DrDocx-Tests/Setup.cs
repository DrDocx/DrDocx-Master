using System;
using System.IO;
using NUnit.Framework;
using DotNetEnv;

namespace DrDocx.Tests
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public void GlobalSetup()
        {
            string workingDir;
            if (Environment.GetEnvironmentVariable("THIS_ENV") != "CI")
                workingDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/DrDocx";
            else
                workingDir = Environment.CurrentDirectory + "/TestFiles";
            if (!Directory.Exists(workingDir))
                Directory.CreateDirectory(workingDir);
            Environment.CurrentDirectory = workingDir;
            Env.Load(Environment.CurrentDirectory + "/.env");
            Console.WriteLine("Working directory: " + Environment.CurrentDirectory);
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            
        }
    }
}