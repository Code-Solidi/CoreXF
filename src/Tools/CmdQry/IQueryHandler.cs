namespace CoreXF.Tools.CmdQry
{
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery
    {
        TResult Handle(TQuery query);
    }
}