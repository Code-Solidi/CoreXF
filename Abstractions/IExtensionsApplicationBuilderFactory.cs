using Microsoft.AspNetCore.Builder;

namespace CoreXF.Abstractions
{
    public interface IExtensionsApplicationBuilderFactory
    {
        IExtensionsApplicationBuilder CreateBuilder(IApplicationBuilder builder);
    }
}