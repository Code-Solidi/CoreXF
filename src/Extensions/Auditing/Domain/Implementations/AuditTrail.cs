using System;

using static Auditing.Domain.IAuditTrail;

namespace Auditing.Domain.Implementations
{
    public class AuditTrail : IAuditTrail
    {
        public string Who { get; set; }    // email of...

        public ActionKind What { get; set; }

        public DateTime When { get; set; }

        public string Type { get; set; }

        public string Identity { get; set; }

        public string Notes { get; set; }

        public string MadeBy { get; set; }
    }
}