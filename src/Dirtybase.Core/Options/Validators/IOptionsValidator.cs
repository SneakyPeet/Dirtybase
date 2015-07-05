namespace Dirtybase.Core.Options.Validators
{
    internal interface IOptionsValidator
    {
        Errors Errors(DirtyOptions options);
    }
}