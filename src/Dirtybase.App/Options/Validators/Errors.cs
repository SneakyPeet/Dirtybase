using System;
using System.Collections.Generic;
using System.Linq;

namespace Dirtybase.App.Options.Validators
{
    internal class Errors: List<string>
    {
        public void ThrowIfErrors()
        {
            if (this.Any())
            {
                throw new ArgumentException(this.Message);
            }
        }

        public string Message
        {
            get
            {
                return string.Join("\n", this);
            }
        }
    }
}