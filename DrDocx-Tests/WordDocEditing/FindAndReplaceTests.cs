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
        private WordAPI WordInterface { get; set; }
        private string TemplatePath { get; set; }
        private string DocPath { get; set; }
        private Dictionary<string, string> FindAndReplacePairs { get; set; }
        [SetUp]
        public void Init()
        {
            FindAndReplacePairs = FindAndReplaceTestData.GetFindAndReplacePairs();
            
            var workingDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/DrDocx";
            if (!Directory.Exists(workingDir))
                Directory.CreateDirectory(workingDir);
            Environment.CurrentDirectory = workingDir;
            Env.Load(Environment.CurrentDirectory + "/.env");
            Console.WriteLine("Working directory: " + Environment.CurrentDirectory);
            
            var reportsDir = Environment.CurrentDirectory + Env.GetString("REPORTS_DIR");
            TemplatePath = $"{reportsDir}/" + Env.GetString("REPORT_TEMPLATE1_NAME");
            DocPath = $"{reportsDir}/" + Env.GetString("REPORT_OUTPUT1_NAME");
            WordInterface = new WordAPI(TemplatePath, DocPath);
        }

        // NOTE: The two "before" tests do not test functionality but instead verify that the template exists
        // in that state before any replacing. Otherwise, the "after" tests would be meaningless.
        [Test]
        public void SearchTextExistsBeforeReplace()
        {
            foreach (var pair in FindAndReplacePairs)
                Assert.IsTrue(WordAPI.ContainsText(TemplatePath, pair.Key, false));
        }
        
        [Test]
        public void ReplaceTextDoesNotExistBeforeReplace()
        {
            foreach (var pair in FindAndReplacePairs)
                Assert.IsFalse(WordAPI.ContainsText(TemplatePath, pair.Value, false));
        }

        [Test]
        public void SearchTextDoesNotExistAfterReplace()
        {
            WordInterface.FindAndReplace(FindAndReplacePairs, false);
            foreach (var pair in FindAndReplacePairs)
                Assert.IsFalse(WordAPI.ContainsText(DocPath, pair.Key, false));
        }

        [Test]
        public void ReplaceTextDoesNotExistAfterReplace()
        {
            WordInterface.FindAndReplace(FindAndReplacePairs, false);
            foreach (var pair in FindAndReplacePairs)
                Assert.IsTrue(WordAPI.ContainsText(DocPath, pair.Value, false));
        }
    }

    internal struct FindAndReplaceTestData
    {
        public static Dictionary<string, string> GetFindAndReplacePairs()
        {
            // In progress data randomization. Ignore
            Randomizer.Seed = new Random(739276); // Do not change
            var fakePatient = new Faker<Patient>()
                .RuleFor(p => p.Name, f => f.Name.FullName());
            
            return new Dictionary<string, string>
            {
                // Impractical values are used so we don't catch stray text in a template during the before tests
                {"{{FIRST_NAME}}", "Bart"},
                {"{{LAST_NAME}}", "Simpson"},
                {"{{DOB}}", "1924-06-01"},
                {"{{DOE}}", "2020-02-10"},
                {"{{MRNUM}}", "756483762"},
                {"{{HANDEDNESS}}", "lefty-lefty"},
                {"{{AGE}}", "20452"},
                {"{{REFERRAL}}", "Dr. Nick"},
                {"{{ETHNICITY}}", "Simpsonian"},
                {"{{EDUCATION}}", "1322"}
            };
        }
    }
}