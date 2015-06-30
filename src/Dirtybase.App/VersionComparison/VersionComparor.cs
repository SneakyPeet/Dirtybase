using System.IO;
using System.Linq;
using System.Collections.Generic;
using Dirtybase.App.Exceptions;

namespace Dirtybase.App.VersionComparison
{
    internal class VersionComparor : IVersionComparor
    {
        public IEnumerable<DirtybaseVersion> GetNewVersions(DirtyOptions options, List<DirtybaseVersion> existingVersions)
        {
            var versionFiles = GetVersionFiles(options);
            VerifyConsistentVersioning(existingVersions, versionFiles);
            return versionFiles.Except(existingVersions);
        }

        private List<DirtybaseVersion> GetVersionFiles(DirtyOptions options)
        {
            //todo pass in extension
            var files = Directory.GetFiles(options.ScriptFolder, "v*_*.sql", SearchOption.AllDirectories);
            return files.Select(f => new DirtybaseVersion(f)).ToList();
        }

        private static void VerifyConsistentVersioning(List<DirtybaseVersion> existingVersions, List<DirtybaseVersion> versionFiles)
        {
            var filesInDbNotInFolder = existingVersions.Except(versionFiles).ToList();
            if (filesInDbNotInFolder.Any())
            {
                throw new VersionFileMissingException(filesInDbNotInFolder.Select(x => x.FileName).ToList()); 
            }
        }
    }
}