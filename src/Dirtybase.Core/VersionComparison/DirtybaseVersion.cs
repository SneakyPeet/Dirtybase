using System.IO;
using Dirtybase.Core.Exceptions;

namespace Dirtybase.Core.VersionComparison
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
            this.Version = this.GetVersionFromFileName(this.FileName);
        }

        public DirtybaseVersion(string version, string fileName)
        {
            this.FilePath = fileName;
            this.FileName = fileName;
            this.Version = version;
        }

        private string GetVersionFromFileName(string fileName)
        {
            var underscoreIndex = fileName.IndexOf('_');
            if (underscoreIndex < 2)
            {
                throw new VersionFileNameFormatException(fileName);
            }
            return fileName.Substring(1, underscoreIndex-1);
        }

        public string Version { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        protected bool Equals(DirtybaseVersion other)
        {
            return string.Equals(this.Version, other.Version);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Version != null ? this.Version.GetHashCode() : 0) * 397);
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
            var version = obj as DirtybaseVersion;
            if(version == null)
            {
                return false;
            }
            return this.Equals(version);
        }
    }
}