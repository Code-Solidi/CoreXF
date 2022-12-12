using CoreXF.Abstractions;

using System.Collections.Generic;

namespace HostApp.WebApi.Models
{
    public class ExtensionsModel
    {
        public IEnumerable<WebApiExtension> Extensions { get; set; }
    }
}