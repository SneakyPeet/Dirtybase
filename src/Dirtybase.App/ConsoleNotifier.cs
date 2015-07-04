using System;
using Dirtybase.Core;

namespace Dirtybase.App
{
    public class ConsoleNotifier : INotifier
    {
        public void SendInfo(string message)
        {
            Console.WriteLine(message);
        }
    }
}