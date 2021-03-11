using Auditing.Application.Commands;

using CoreXF.Tools.CmdQry;

namespace Auditing.Persistence.Handlers
{
    public class CreateAuditTrailHandler : ICommandHandler<CreateAuditTrailCommand>
    {
        private readonly AuditBoundedContext context;

        public CreateAuditTrailHandler(AuditBoundedContext context)
        {
            this.context = context;
        }

        public void Handle(CreateAuditTrailCommand command)
        {
            // do nothing (so far)
        }
    }
}