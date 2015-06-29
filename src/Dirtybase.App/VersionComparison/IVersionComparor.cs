using System.Collections.Generic;

namespace Dirtybase.App.VersionComparison
{
    public interface IVersionComparor
    {
        IEnumerable<DirtybaseVersion> GetNewVersions(DirtyOptions options, IEnumerable<DirtybaseVersion> existingVersions);
    }
}