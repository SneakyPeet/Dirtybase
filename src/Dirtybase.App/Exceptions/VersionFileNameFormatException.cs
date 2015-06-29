namespace Dirtybase.App.Exceptions
{
    class VersionFileNameFormatException : DirtybaseException
    {
        public VersionFileNameFormatException(string fileName)
            : base(fileName + " does not conform to the file naming convention")
        {
            
        }
    }
}