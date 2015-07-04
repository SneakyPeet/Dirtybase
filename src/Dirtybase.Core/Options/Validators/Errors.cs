using System;
using System.Collections.Generic;
using System.Linq;

namespace Dirtybase.Core.Options.Validators
{
    public class Errors: List<string>
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