using System;
using Dirtybase.Core;
using Dirtybase.Core.Exceptions;

namespace Dirtybase.App
{
    class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += AppExceptionHandler;
            new DirtybaseApi(new ConsoleNotifier()).Do(args);
            Environment.Exit(0);
        }

        static void AppExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as DirtybaseException;
            if(exception != null)
            {
                Console.Error.WriteLine(exception.Message);
            }
            else
            {
                Console.Error.WriteLine(e.ExceptionObject.ToString());
            }

            Environment.Exit(1);
        }
    }
}
