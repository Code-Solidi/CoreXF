using System.Linq;
using CoreXF.Abstractions.Base;
using CoreXF.Abstractions.Builder;
using Extension2.Middlewares;

using Microsoft.AspNetCore.Builder;

namespace Extension2
{
    public class Extension : ExtensionBaseWithViews
    {
        public override string Name => nameof(Extension2);

        public override string Views => $"{typeof(Extension).Assembly.GetName().Name}.Views.dll";

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