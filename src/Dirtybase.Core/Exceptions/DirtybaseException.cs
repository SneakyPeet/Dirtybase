using System;

namespace Dirtybase.Core.Exceptions
{
    public class DirtybaseException : Exception
    {
        public DirtybaseException(string message) : base(message)
        {
             
        }
    }
}