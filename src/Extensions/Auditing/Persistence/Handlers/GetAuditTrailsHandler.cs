using Auditing.Application.Queries;
using Auditing.Domain;
using Auditing.Domain.Implementations;

using CoreXF.Tools.CmdQry;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;

namespace Auditing.Persistence.Handlers
{
    public class GetAuditTrailsHandler : IQueryHandler<GetAuditTrailsQuery, IEnumerable<IAuditTrail>>
    {
        private readonly AuditBoundedContext context;

        public GetAuditTrailsHandler(AuditBoundedContext context)
        {
            this.context = context;
        }

        public IEnumerable<IAuditTrail> Handle(GetAuditTrailsQuery query)
        {
            var auditSet = this.context.Set<AuditTrail>();
            return auditSet.AsNoTracking().Select(x => new AuditTrail
            {
                Identity = x.Identity,
                MadeBy = x.MadeBy,
                Notes = x.Notes,
                Type = x.Type,
                What = x.What,
                When = x.When,
                Who = x.Who
            });
        }
    }
}