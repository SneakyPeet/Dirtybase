using System.Collections.Generic;
using System.Linq;
using Dirtybase.Core.Exceptions;

namespace Dirtybase.Core.Options.Validators
{
    public class Errors: List<string>
    {
        public void ThrowIfErrors()
        {
            if (this.Any())
            {
                throw new DirtybaseException(this.Message);
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