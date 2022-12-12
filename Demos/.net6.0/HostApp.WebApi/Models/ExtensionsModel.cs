using CoreXF.Abstractions;
using CoreXF.Abstractions.Base;

using System.Collections.Generic;

namespace HostApp.WebApi.Models
{
    public class ExtensionsModel
    {
        public IEnumerable<WebApiExtension> Extensions { get; set; }
    }
}