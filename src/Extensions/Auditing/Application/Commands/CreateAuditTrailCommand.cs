
using Auditing.Domain;
using Auditing.Domain.Implementations;

using CoreXF.Tools.CmdQry;

namespace Auditing.Application.Commands
{
    public class CreateAuditTrailCommand : ICommand
    {
        public IAuditTrail AuditTrail => new AuditTrail();
    }
}