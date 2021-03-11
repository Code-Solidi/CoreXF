using System;

namespace Auditing.Domain
{
    public interface IAuditTrail
    {
        public enum ActionKind
        {
            Create, Read, Update, Delete
        }

        string Identity { get; set; }

        string MadeBy { get; set; }

        string Notes { get; set; }

        string Type { get; set; }

        ActionKind What { get; set; }

        DateTime When { get; set; }

        string Who { get; set; }
    }
}