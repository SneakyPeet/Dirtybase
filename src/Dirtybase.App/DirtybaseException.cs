using System;

namespace Dirtybase.App.Implementations.Sqlite
{
    public class DirtybaseException : Exception
    {
        public DirtybaseException(string message) : base(message)
        {
             
        }
    }
}