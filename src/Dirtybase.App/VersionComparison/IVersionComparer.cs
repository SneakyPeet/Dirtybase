using System.Collections.Generic;

namespace Dirtybase.App.VersionComparison
{
    public interface IVersionComparer
    {
        IList<DirtybaseVersion> GetNewVersions(DirtyOptions options, List<DirtybaseVersion> existingVersions);
    }
}