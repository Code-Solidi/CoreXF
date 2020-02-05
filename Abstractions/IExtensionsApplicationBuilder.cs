using System.Collections.Generic;

using Microsoft.AspNetCore.Builder;

namespace CoreXF.Abstractions
{
    public interface IExtensionsApplicationBuilder : IApplicationBuilder
    {
        IApplicationBuilder ExpansionPoint(string shimId);

        IEnumerable<string> GetShims();

        void Populate(IApplicationBuilder app);
    }
}