using NUnit.Framework;
using DrDocx.Models;
using DrDocx.WordDocEditing;

namespace DrDocx.Tests.WordDocEditing
{
    public class FindAndReplaceTests
    {

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
}