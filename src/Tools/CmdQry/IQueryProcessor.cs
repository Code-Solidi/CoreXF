namespace CoreXF.Tools.CmdQry
{
    public interface IQueryProcessor
    {
        TResult Process<TResult>(IQuery query);
    }
}
