using System.IO;
using System.Linq;
using System.Collections.Generic;
using Dirtybase.Core.Exceptions;
using Dirtybase.Core.Options;

namespace Dirtybase.Core.VersionComparison
{
    internal class VersionComparer : IVersionComparer
    {
        public IList<DirtybaseVersion> GetNewVersions(DirtyOptions options, List<DirtybaseVersion> existingVersions)
        {
            var versionFiles = this.GetVersionFiles(options);
            VerifyConsistentVersioning(existingVersions, versionFiles);
            var newversions = versionFiles.Except(existingVersions);
            return newversions.OrderBy(x => x.Version, new NaturalComparer()).ToList();
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