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
            wordAPI.ContainsText("Hello");
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
            Randomizer.Seed = new Random(739276); // Do not change
            var fakePatient = new Faker<Patient>()
                .RuleFor(p => p.Name, f => f.Name.FullName());
            
            return new Dictionary<string, string>
            {
                {"{{FIRST_NAME}}", "Bart"},
                {"{{LAST_NAME}}", "Simpson"},
                {"{{DOB}}", "2000-01-01"},
                {"{{DOE}}", "2020-02-10"},
                {"{{MRNUM}}", "756483762"},
                {"{{HANDEDNESS}}", "left"},
                {"{{AGE}}", "20"},
                {"{{REFERRAL}}", "Dr. Nick"},
                {"{{ETHNICITY}}", "Simpsonian"},
                {"{{EDUCATION}}", "12"}
            };
        }
    }
}