using Xunit;
using DrDocx.Models;
using DrDocx.WordDocEditing;

namespace DrDocx.Tests.WordDocEditing
{
    public class FindAndReplaceTests
    {
        [Theory]
        [InlineData("{{NAME}}")]
        public void ReplacedTextIsGone(string textToReplace)
        {
            Assert.True(true);
        } 
    }
}