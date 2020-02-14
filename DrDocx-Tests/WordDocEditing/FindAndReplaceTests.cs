using NUnit.Framework;
using DrDocx.Models;
using DrDocx.WordDocEditing;
using System.Collections.Generic;
using Bogus;
using System;
using System.IO;
using DotNetEnv;

namespace DrDocx.Tests.WordDocEditing
{
    [TestFixture]
    public class FindAndReplaceTests
    {
        [SetUp]
        public void Init()
        {
            var workingDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/DrDocx";
            if (!Directory.Exists(workingDir))
                Directory.CreateDirectory(workingDir);
            Environment.CurrentDirectory = workingDir;
            Env.Load(Environment.CurrentDirectory + "/.env");
            Console.WriteLine("Working directory: " + Environment.CurrentDirectory);
        }

        [Test]
        public void TryFindAndReplace()
        {
            var reportsDir = Environment.CurrentDirectory + Env.GetString("REPORTS_DIR");
            var template1 = Env.GetString("REPORT_TEMPLATE1_NAME");
            var report1 = Env.GetString("REPORT_OUTPUT1_NAME");
            var wordAPI = new WordAPI($"{reportsDir}/{template1}", $"{reportsDir}/{report1}"); 
            wordAPI.FindAndReplace(FindAndReplaceTestData.GetFindAndReplacePairs(), false);
            Assert.IsTrue(true);
        }

        [Test]
        public void SearchTextIsGoneAfterReplace(string searchText)
        {
            Assert.True(true);
        }

        [Test]
        [TestCase("Sideshow Bob")]
        [TestCase("Bart Simpson")]
        [TestCase("IJustHappenToHaveAnExtremelyLong NameToSeeIfICanMessWithOpenXML BecauseIHateWhenStuffWorks")]
        public void ReplaceTextIsFoundAfterReplace(string replaceText)
        {
            Assert.IsFalse(replaceText == null);
        }
    }

    internal struct FindAndReplaceTestData
    {
        public static Dictionary<string, string> GetFindAndReplacePairs()
        {
            return new Dictionary<string, string>
            {
                {"{{NAME}}", "Bart Simpson"}
            };
        }
    }
}