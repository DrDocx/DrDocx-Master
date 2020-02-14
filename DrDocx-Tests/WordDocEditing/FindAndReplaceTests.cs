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
        private string ReportsDir { get; set; }
        private string TemplatePath { get; set; }
        private string DocPath { get; set; }
        private Dictionary<string, string> FindAndReplacePairs { get; set; }
        
        [SetUp]
        public void Init()
        {
            FindAndReplacePairs = FindAndReplaceTestData.GetFindAndReplacePairs();

            ReportsDir = Environment.CurrentDirectory + Env.GetString("REPORTS_DIR");
            if (!Directory.Exists(ReportsDir))
                Directory.CreateDirectory(ReportsDir);
            TemplatePath = $"{ReportsDir}/" + Env.GetString("REPORT_TEMPLATE1_NAME");
            DocPath = $"{ReportsDir}/" + Env.GetString("REPORT_OUTPUT1_NAME");
        }

        [TearDown]
        public void Cleanup()
        {
            // Comment this line if you want to see the file after the test.
            File.Delete(DocPath);
        }

        [Test]
        public void TemplateFileExists()
        {
            var potentialErrorMessage = $"You need to place the template in the directory {Environment.CurrentDirectory}. " +
                                        $"Then, update your .env file in {Environment.CurrentDirectory} with the template file name for key REPORT_TEMPLATE1_NAME";
            Assert.IsTrue(File.Exists(TemplatePath), potentialErrorMessage);
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
            WordInterface = new WordAPI(TemplatePath, DocPath);
            WordInterface.FindAndReplace(FindAndReplacePairs, false);
            foreach (var pair in FindAndReplacePairs)
                Assert.IsFalse(WordAPI.ContainsText(DocPath, pair.Key, false));
        }

        [Test]
        public void ReplaceTextDoesNotExistAfterReplace()
        {
            WordInterface = new WordAPI(TemplatePath, DocPath);
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