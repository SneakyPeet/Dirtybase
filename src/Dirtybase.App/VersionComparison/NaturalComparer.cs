using System;
using System.Collections.Generic;

namespace Dirtybase.App.VersionComparison
{
    public class NaturalComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var xParser = new Parser(x);
            var yParser = new Parser(y);
            while (xParser.HasMoreToRead || yParser.HasMoreToRead)
            {
                xParser.ReadNext();
                yParser.ReadNext();
                if (xParser.CurrentStringType == yParser.CurrentStringType)
                {
                    var compareValue = xParser.Compare(yParser);
                    if (compareValue != 0)
                    {
                        return compareValue;
                    }
                }
                else if (xParser.CurrentStringType > yParser.CurrentStringType)
                {
                    return 1;
                }
                else if (xParser.CurrentStringType < yParser.CurrentStringType)
                {
                    return -1;
                }
            }
            return 0;
        }

        class Parser
        {
            private string input;
            private int currentIndex;
            private int length;

            public bool HasMoreToRead { get; private set; }

            public StringType CurrentStringType { get; private set; }

            public object Value { get; private set; }

            public Parser(string input)
            {
                CurrentStringType = StringType.Empty;
                Value = null;
                HasMoreToRead = !string.IsNullOrWhiteSpace(input);
                currentIndex = 0;
                if (HasMoreToRead)
                {
                    this.input = input;
                    length = input.Length;
                }
            }

            public void ReadNext()
            {
                if (HasMoreToRead)
                {
                    if(length - 1 == currentIndex)
                    {
                        this.SetEmpty();
                    }
                    else
                    {
                        var ch = input[currentIndex];
                        if(ch == '\0')
                        {
                            SetEmpty();
                        }
                        else if(char.IsLetter(ch))
                        {
                            this.CurrentStringType = StringType.Text;
                            var count = 1;
                            var nch = input[currentIndex + count];
                            while (currentIndex + count  < length && char.IsLetter(nch))
                            {
                                nch = input[currentIndex + count];
                                count++;
                            }
                            this.Value = input.Substring(currentIndex, count);
                            currentIndex = currentIndex + count;
                        }
                        else if(char.IsDigit(ch))
                        {
                            this.CurrentStringType = StringType.Number;
                            var count = 1;
                            var nch = input[currentIndex + count];
                            while (currentIndex + count < length && char.IsDigit(nch))
                            {
                                nch = input[currentIndex + count];
                                count++;
                            }
                            var valtoparse = input.Substring(currentIndex, count);
                            this.Value = Int32.Parse(valtoparse);
                            currentIndex = currentIndex + count;
                        }
                        else
                        {
                            currentIndex++;
                            ReadNext();
                        }
                    }
                    if (length == currentIndex)
                    {
                        HasMoreToRead = false;
                    }
                }
            }

            private void SetEmpty()
            {
                this.HasMoreToRead = false;
                this.CurrentStringType = StringType.Empty;
                this.Value = null;
            }

            public int Compare(Parser y)
            {
                if(CurrentStringType != y.CurrentStringType)
                {
                    return CurrentStringType - y.CurrentStringType;
                }
                switch (CurrentStringType)
                {
                    case StringType.Text:
                        return string.Compare((string)Value, (string)y.Value);
                    case StringType.Number:
                        if ((int)Value > (int)y.Value)
                        {
                            return 1;
                        }
                        if ((int)Value < (int)y.Value)
                        {
                            return -1;
                        }
                        return 0;
                    default:
                        return 0;
                }
            }
        }

        enum StringType
        {
            Empty = 0,
            Text = 1,
            Number = 2
        }
    }

}