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
        [TestCase("va","vb",-1)]
        [TestCase("vb", "vb", 0)]
        [TestCase("vb", "va", 1)]
        [TestCase("v22", "v113", -1)]
        [TestCase("v1.1.115", "v1.1.15", 1)]
        [TestCase("va.2.b", "va.3.b", -1)]
        [TestCase("v....", "va", -1)]
        [TestCase("v1.5c", "v1.5c", 0)]
        [TestCase("vaaa", "vaa", 1)]
        [TestCase("v1.2.515", "v1.15.1", -1)]
        public void Test(string item1, string item2, int expectedOutput)
        {
            var output = comparer.Compare(item1, item2);
            output.Should().Be.EqualTo(expectedOutput);
        }
    }

   
}
