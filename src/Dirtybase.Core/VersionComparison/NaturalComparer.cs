using System.Collections.Generic;

namespace Dirtybase.Core.VersionComparison
{
    public class NaturalComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var xParser = new NaturalValueParser(x);
            var yParser = new NaturalValueParser(y);
            while (xParser.HasMoreToRead || yParser.HasMoreToRead)
            {
                xParser.ReadNext();
                yParser.ReadNext();
                if (xParser.CurrentNaturalStringType == yParser.CurrentNaturalStringType)
                {
                    var compareValue = xParser.Compare(yParser);
                    if (compareValue != 0)
                    {
                        return compareValue;
                    }
                }
                else if (xParser.CurrentNaturalStringType > yParser.CurrentNaturalStringType)
                {
                    return 1;
                }
                else if (xParser.CurrentNaturalStringType < yParser.CurrentNaturalStringType)
                {
                    return -1;
                }
            }
            return 0;
        }

    }
}