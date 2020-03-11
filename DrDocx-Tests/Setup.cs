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
            var workingDir = Environment.CurrentDirectory + "/TestFiles";
            if (!Directory.Exists(workingDir))
                Directory.CreateDirectory(workingDir);
            Environment.CurrentDirectory = workingDir;
            if (Environment.GetEnvironmentVariable("THIS_ENV") != "CI")
                Env.Load(Environment.CurrentDirectory + "/.env");
            Console.WriteLine("Working directory: " + Environment.CurrentDirectory);
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            
        }
    }
}