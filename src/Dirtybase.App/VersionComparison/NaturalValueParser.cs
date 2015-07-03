using System;

namespace Dirtybase.App.VersionComparison
{
    class NaturalValueParser
    {
        private readonly string input;
        private int currentIndex;
        private readonly int length;

        public bool HasMoreToRead { get; private set; }

        public NaturalStringType CurrentNaturalStringType { get; private set; }

        private object value;

        public NaturalValueParser(string input)
        {
            this.CurrentNaturalStringType = NaturalStringType.Empty;
            this.value = null;
            this.HasMoreToRead = !string.IsNullOrWhiteSpace(input);
            this.currentIndex = 0;
            if (this.HasMoreToRead)
            {
                this.input = input;
                this.length = input.Length;
            }
        }

        public void ReadNext()
        {
            if (this.HasMoreToRead)
            {
                if (this.length - 1 == this.currentIndex)
                {
                    this.SetEmpty();
                }
                else
                {
                    var ch = this.input[this.currentIndex];
                    if (ch == '\0')
                    {
                        this.SetEmpty();
                    }
                    else if (char.IsLetter(ch))
                    {
                        this.value = this.SetStringTypeAndGetValueToParse(NaturalStringType.Text, char.IsLetter);
                    }
                    else if (char.IsDigit(ch))
                    {
                        var valueToParse = this.SetStringTypeAndGetValueToParse(NaturalStringType.Number, char.IsDigit);
                        this.value = Int32.Parse(valueToParse);
                    }
                    else
                    {
                        this.currentIndex++;
                        this.ReadNext();
                    }
                }
                if (this.length == this.currentIndex)
                {
                    this.HasMoreToRead = false;
                }
            }
        }

        private string SetStringTypeAndGetValueToParse(NaturalStringType naturalStringType, Func<char, bool> charChecker)
        {
            this.CurrentNaturalStringType = naturalStringType;
            var count = this.GetSubStringLength(charChecker);
            var returnValue = this.input.Substring(this.currentIndex, count);
            this.currentIndex = this.currentIndex + count;
            return returnValue;
        }

        private int GetSubStringLength(Func<char, bool> charChecker)
        {
            var count = 1;
            while (this.currentIndex + count < this.length)
            {
                var nch = this.input[this.currentIndex + count];
                if (charChecker(nch))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            return count;
        }

        private void SetEmpty()
        {
            this.HasMoreToRead = false;
            this.CurrentNaturalStringType = NaturalStringType.Empty;
            this.value = null;
        }

        public int Compare(NaturalValueParser y)
        {
            if (this.CurrentNaturalStringType != y.CurrentNaturalStringType)
            {
                return this.CurrentNaturalStringType - y.CurrentNaturalStringType;
            }
            switch (this.CurrentNaturalStringType)
            {
                case NaturalStringType.Text:
                    return string.Compare((string)this.value, (string)y.value);
                case NaturalStringType.Number:
                    if ((int)this.value > (int)y.value)
                    {
                        return 1;
                    }
                    if ((int)this.value < (int)y.value)
                    {
                        return -1;
                    }
                    return 0;
                default:
                    return 0;
            }
        }
    }
}