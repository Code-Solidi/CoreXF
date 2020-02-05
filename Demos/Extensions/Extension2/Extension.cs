using System.Linq;

using CoreXF.Abstractions;

using Extension2.Middlewares;

using Microsoft.AspNetCore.Builder;

namespace Extension2
{
    public class Extension : ExtensionBase
    {
        public override string Name => nameof(Extension2);

        public override void ConfigureMiddleware(IExtensionsApplicationBuilder app)
        {
            var shims = app.GetShims().ToArray();

            var xp = app.ExpansionPoint(shims[1]);
            xp.UseRequestCulture();

            // simulate getting xp again
            xp = app.ExpansionPoint(shims[1]);
            xp.UseMiddleware<LoggerMiddleware>();
        }
    }
}