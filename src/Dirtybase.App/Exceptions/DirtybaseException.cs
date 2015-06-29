using System;

namespace Dirtybase.App.Exceptions
{
    public class DirtybaseException : Exception
    {
        public DirtybaseException(string message) : base(message)
        {
             
        }
    }
}