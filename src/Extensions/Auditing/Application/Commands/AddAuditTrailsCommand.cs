
using Auditing.Domain;
using Auditing.Domain.Implementations;

using CoreXF.Tools.CmdQry;

using System.Collections.Generic;
using System.Linq;

namespace Auditing.Application.Commands
{
    public class AddAuditTrailsCommand : ICommand
    {
        public AddAuditTrailsCommand(IEnumerable<IAuditTrail> auditTrails)
        {
            this.AuditTrails = auditTrails.Cast<AuditTrail>();
        }

        public IEnumerable<AuditTrail> AuditTrails { get; }
    }
}