using System;
using Dirtybase.Core.Commands;
using Dirtybase.Core.Options;
using Dirtybase.Core.VersionComparison;

namespace Dirtybase.Core
{
    public class DirtybaseApi
    {
        private INotifier notifier;

        public DirtybaseApi(INotifier notifier)
        {
            if(notifier == null)
            {
                throw new ArgumentNullException("notifier");
            }
            this.notifier = notifier;
        }
        public void Do(string[] args)
        {
            var options = new DirtyOptions(args);
            var commandFactory = new CommandFactory();
            var command = commandFactory.Make(options);
            command.Execute(options, new VersionComparer(), notifier);
        }
    }
}
