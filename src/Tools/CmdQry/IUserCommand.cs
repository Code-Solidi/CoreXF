namespace CoreXF.Tools.CmdQry
{
    public interface IUserCommand : ICommand
    {
        string User { get; }
    }
}