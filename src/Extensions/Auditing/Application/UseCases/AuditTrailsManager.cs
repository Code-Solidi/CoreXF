
using Auditing.Application.Commands;
using Auditing.Application.Queries;
using Auditing.Domain;

using CoreXF.Tools.CmdQry;

using System.Collections.Generic;

namespace Auditing.Application.UseCases
{
    internal class AuditTrailsManager : IAuditTrailsManager
    {
        private readonly ICommandHandler<CreateAuditTrailCommand> createAuditTrailHandler;
        private readonly ICommandHandler<AddAuditTrailsCommand> addAuditTrailsHandler;
        private readonly IQueryHandler<GetAuditTrailsQuery, IEnumerable<IAuditTrail>> getAuditTrailsHandler;

        public AuditTrailsManager(ICommandHandler<CreateAuditTrailCommand> createAuditTrailHandler
            , ICommandHandler<AddAuditTrailsCommand> addAuditTrailsHandler
            , IQueryHandler<GetAuditTrailsQuery, IEnumerable<IAuditTrail>> getAuditTrailsHandler)
        {
            this.createAuditTrailHandler = createAuditTrailHandler;
            this.addAuditTrailsHandler = addAuditTrailsHandler;
            this.getAuditTrailsHandler = getAuditTrailsHandler;
        }

        //public string Name => nameof(AuditTrailsManager);

        public IAuditTrail CreateAuditTrail()
        {
            var command = new CreateAuditTrailCommand();
            this.createAuditTrailHandler.Handle(command);
            return command.AuditTrail;
        }

        public IEnumerable<IAuditTrail> GetAuditTrails()
        {
            var query = new GetAuditTrailsQuery();
            return this.getAuditTrailsHandler.Handle(query);
        }

        public void Save(IEnumerable<IAuditTrail> auditTrails)
        {
            var command = new AddAuditTrailsCommand(auditTrails);
            this.addAuditTrailsHandler.Handle(command);
        }
    }
}