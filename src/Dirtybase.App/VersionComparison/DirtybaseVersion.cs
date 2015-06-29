using System.IO;
using Dirtybase.App.Exceptions;

namespace Dirtybase.App.VersionComparison
{
    public class DirtybaseVersion
    {
        public DirtybaseVersion(string version, string fileName, string path)
        {
            this.Version = version;
            this.FileName = fileName;
            this.FilePath = path;
        }

        public DirtybaseVersion(string filePath)
        {
            this.FilePath = filePath;
            this.FileName = Path.GetFileName(filePath);
            this.Version = GetVersionFromFileName(FileName);
        }

        private string GetVersionFromFileName(string fileName)
        {
            var underscoreIndex = fileName.IndexOf('_');
            if (underscoreIndex < 2)
            {
                throw new VersionFileNameFormatException(fileName);
            }
            return fileName.Substring(0, underscoreIndex);
        }

        public string Version { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        protected bool Equals(DirtybaseVersion other)
        {
            return string.Equals(this.Version, other.Version)
                   && string.Equals(this.FileName, other.FileName);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Version != null ? this.Version.GetHashCode() : 0) * 397)
                       ^ (this.FileName != null ? this.FileName.GetHashCode() : 0);
            }
        }

        public static bool operator ==(DirtybaseVersion left, DirtybaseVersion right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DirtybaseVersion left, DirtybaseVersion right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}