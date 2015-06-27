namespace Dirtybase.App.Commands
{
    public interface IDirtyCommand
    {
        void Execute(DirtyOptions options);
    }
}