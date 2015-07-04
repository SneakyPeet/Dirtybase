using System.Collections.Generic;
using Dirtybase.Core.Options;

namespace Dirtybase.Core.VersionComparison
{
    public interface IVersionComparer
    {
        IList<DirtybaseVersion> GetNewVersions(DirtyOptions options, List<DirtybaseVersion> existingVersions);
    }
}