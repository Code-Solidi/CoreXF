using CoreXF.Abstractions.Base;

using System.Diagnostics.CodeAnalysis;

namespace Auditing
{
    public class Extension : ExtensionBase
    {
        [SuppressMessage("Design", "CC0021:Use nameof", Justification = "<Pending>")]
        public override string Name => "Auditing";

        public override string Authors => "Code Solidi Ltd.";

        public override string Url => "www.codesolidi.com";

        public override string Description => "Audit Microservice, all mutating operations are logged by this service.";
    }
}