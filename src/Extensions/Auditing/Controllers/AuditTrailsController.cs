
using Auditing.Domain;

using CoreXF.Abstractions.Attributes;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Linq;

namespace Auditing.Controllers
{
    [ApiController, Export]
    public class AuditTrailsController : ControllerBase
    {
        private readonly IAuditTrailsManager auditor;

        public AuditTrailsController(IAuditTrailsManager auditor)
        {
            this.auditor = auditor ?? throw new ArgumentNullException(nameof(auditor));
        }

        [HttpGet, Route("/audit-trails")]
        public IActionResult Get()
        {
            var auditTrails = this.auditor.GetAuditTrails();
            return auditTrails?.Count() != 0 ? this.Ok(auditTrails) : this.NotFound();
        }
    }
}