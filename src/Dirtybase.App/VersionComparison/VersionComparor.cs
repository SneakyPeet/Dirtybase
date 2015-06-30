using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Dirtybase.App.VersionComparison
{
    internal class VersionComparor : IVersionComparor
    {
        public IEnumerable<DirtybaseVersion> GetNewVersions(DirtyOptions options, IEnumerable<DirtybaseVersion> existingVersions)
        {
            var versionFiles = GetVersionFiles(options);
            return versionFiles.Except(existingVersions);
        }

        private IEnumerable<DirtybaseVersion> GetVersionFiles(DirtyOptions options)
        {
            //todo pass in extension
            var files = Directory.GetFiles(options.ScriptFolder, "v*_*.sql", SearchOption.AllDirectories);
            return files.Select(f => new DirtybaseVersion(f));
        }
    }
}