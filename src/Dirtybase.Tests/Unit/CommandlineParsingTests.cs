using System;
using Dirtybase.App;
using NUnit.Framework;
using SharpTestsEx;

namespace Dirtybase.Tests.Unit
{
    [TestFixture]
    public class CommandlineParsingTests
    {
        private static readonly object[] positiveTestCases =
            {
                new object[] { "init", new DirtyOptions(DirtyCommand.Init) },
                new object[] { "migrate", new DirtyOptions(DirtyCommand.Migrate) },
                new object[] { "help", new DirtyOptions(DirtyCommand.Help) }
            };

        [Test]
        [TestCaseSource("positiveTestCases")]
        public void GivenPositiveTestCaseOptionsShouldMatch(string input, DirtyOptions expectedOptions)
        {
            var args = input.Split(' ');
            var options = new DirtyOptions(args);
            options.Should().Be.EqualTo(expectedOptions);
        }

        private static readonly object[] negativeTestCases =
            {
                new object[] { "", typeof(ArgumentException), "use 'help' option for help" },
                new object[] { "foo", typeof(ArgumentException), "foo is not an option. use 'help' option for help" }
            };

        [Test]
        [TestCaseSource("negativeTestCases")]
        public void GivenNegativeTestCaseThrowException(string input, Type expectedException, string message)
        {
            try
            {
                var args= input.Split(' ');
                new DirtyOptions(args);
                throw new Exception("exception not thrown");
            }
            catch(Exception e)
            {
                if(e.GetType() == expectedException)
                {
                    e.Message.Should().Be.EqualTo(message);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
