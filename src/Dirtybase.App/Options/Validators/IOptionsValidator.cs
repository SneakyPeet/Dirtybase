namespace Dirtybase.App.Options.Validators
{
    internal interface IOptionsValidator
    {
        Errors Errors(DirtyOptions options);
    }
}