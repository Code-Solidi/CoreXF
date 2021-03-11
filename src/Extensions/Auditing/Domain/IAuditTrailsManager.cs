
using System.Collections.Generic;

namespace Auditing.Domain
{
    public interface IAuditTrailsManager
    {
        IAuditTrail CreateAuditTrail();

        IEnumerable<IAuditTrail> GetAuditTrails();

        void Save(IEnumerable<IAuditTrail> auditTrails);
    }
}