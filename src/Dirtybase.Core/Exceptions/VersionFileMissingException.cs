using System.Collections.Generic;

namespace Dirtybase.Core.Exceptions
{
    public class VersionFileMissingException : DirtybaseException
    {
        public VersionFileMissingException(List<string> files)
            : base(MakeMessage(files))
        {

        }

        private static string MakeMessage(List<string> files)
        {
            if (files.Count > 1)
            {
                return string.Format("The version files {0} cannot be found in script directory. Inconsistent versioning", string.Join(", ", files));
            }
            if (files.Count == 1)
            {
                return string.Format("The version file '{0}' cannot be found in script directory. Inconsistent versioning", files[0]);
            }
            return string.Empty;
        }
    }
}