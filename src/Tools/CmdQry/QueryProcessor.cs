using System.Diagnostics;

using Microsoft.Extensions.DependencyInjection;

namespace CoreXF.Tools.CmdQry
{
    public sealed class QueryProcessor : IQueryProcessor
    {
        private readonly IServiceCollection container;

        public QueryProcessor(IServiceCollection container)
        {
            this.container = container;
        }

        [DebuggerStepThrough]
        public TResult Process<TResult>(IQuery query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic handler = this.container.BuildServiceProvider().GetRequiredService(handlerType);
            return handler.Handle((dynamic)query);
        }
    }
}
