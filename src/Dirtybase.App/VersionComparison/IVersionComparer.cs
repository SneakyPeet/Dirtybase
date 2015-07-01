using System.Collections.Generic;

namespace Dirtybase.App.VersionComparison
{
    public interface IVersionComparer
    {
        IEnumerable<DirtybaseVersion> GetNewVersions(DirtyOptions options, List<DirtybaseVersion> existingVersions);
    }
}