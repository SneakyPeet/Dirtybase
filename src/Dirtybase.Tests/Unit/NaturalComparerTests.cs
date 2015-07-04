using Dirtybase.App.VersionComparison;
using NUnit.Framework;
using SharpTestsEx;

namespace Dirtybase.Tests.Unit
{
    [TestFixture]
    [Category(TestTypes.Unit)]
    public class NaturalComparerTests
    {
        private NaturalComparer comparer;

        [TestFixtureSetUp]
        public void SetUp()
        {
            comparer = new NaturalComparer();
        }

        [Test]
        [TestCase("a","b",-1)]
        [TestCase("113", "113", 0)]
        [TestCase("b", "b", 0)]
        [TestCase("b", "a", 1)]
        [TestCase("22", "113", -1)]
        [TestCase("1.1.115", "1.1.15", 1)]
        [TestCase("a.2.b", "a.3.b", -1)]
        [TestCase("....", "a", -1)]
        [TestCase("1.5c", "1.5c", 0)]
        [TestCase("aaa", "aa", 1)]
        [TestCase("1.2.515", "1.15.1", -1)]
        [TestCase("a", "1", -1)]
        public void Test(string item1, string item2, int expectedOutput)
        {
            var output = comparer.Compare(item1, item2);
            output.Should().Be.EqualTo(expectedOutput);
        }
    }

   
}
