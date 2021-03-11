using Auditing.Application.Commands;

using CoreXF.Tools.CmdQry;

namespace Auditing.Persistence.Handlers
{
    public class AddAuditTrailsHandler : ICommandHandler<AddAuditTrailsCommand>
    {
        private readonly AuditBoundedContext context;

        public AddAuditTrailsHandler(AuditBoundedContext context)
        {
            this.context = context;
        }

        public void Handle(AddAuditTrailsCommand command)
        {
            foreach (var auditTrail in command.AuditTrails)
            {
                this.context.Add(auditTrail);
            }

            this.context.SaveChanges();
        }
    }
}