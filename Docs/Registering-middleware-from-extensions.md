Middleware registrations happen in `Configure()` method of `Startup` class. The order middlewares are registered is important, it forms the request/response ASP.NET Core pipeline. Unlike DI container to which new services can be easily added, the middleware pipeline is reluctant to modifications - once a middleware becomes part of the pipeline, it stay in its original position and processes the request coming from its predecessor, processes the request and passes it further to its descendant in the pipeline. The response travels the same path only in reverse direction.

CoreXF allows extensions to register middlewares with the help of "shims" which are inserted between each pair of middlewares plus one at the beginning of the pipeline and one at its end.

Thus, if there are three middleware components registered the number of shims is four, one at the beginning, two between the already made registrations, and one at the end.

The extension's middleware may choose where (with which shim) to register itself. Of course, there maybe more than one registrations in the same shim. At the end all shims that contain registrations are "injected" in their relative positions, Shims with no registrations are just removed.

The overall process is coded like this:

1. The original `Configure` method of Startup.cs is slightly modified:

        public void Configure(IApplicationBuilder original
            , IExtensionsApplicationBuilderFactory factory
            , IWebHostEnvironment env)
        {
            var app = factory.CreateBuilder(original);
            // rest of Configure body

This couple of lines need some explanation: the original appbuilder is renamed (not surprisingly!) to `original` and will be used at the end of the method. Then a replacement, instance of `ExtensionsApplicationBuilder` is created by its dedicated factory service.

All the registrations happen with the replacement. The replacement gives access to the shims to any extension allowing for middleware registrations. 

At the end all registrations, bot defined in the Configure method as well as those registered in extensions are put back in the original builder thus forming a modified request/response pipeline.

In order this to happen the next call should end up the `Configure` method:

        // Configure body ends here 
        app.Populate(original.UseCoreXF());
    }

`UseCoreXF` called on `original` builder adds all extensions middlewares, while `Populate` re-inserts back the registrations from `Configure`.

This call ends up the process.

On the other hand in extensions the implementation of `IExtension` supplies the two methods 

    void ConfigureServices(IServiceCollection services, IConfiguration configuration)

and 

    void ConfigureMiddleware(IExtensionsApplicationBuilder app)

The first one has the same signature as the one in `Startup` and in the default implementation (in `ExtensionBase`) looks for `Startup`, creates an instance and invokes `ConfigureServices`.

The second one, however is something new, a sample code illustrates how the method can be used:

        public override void ConfigureMiddleware(IExtensionsApplicationBuilder app)
        {
            var shims = app.GetShims().ToArray();

            var xp = app.ExpansionPoint(shims[1]);
            xp.UseRequestCulture();

            // simulate getting xp again
            xp = app.ExpansionPoint(shims[1]);
            xp.UseMiddleware<LoggerMiddleware>();
        }

Here, using "shim-1" means that the middleware registrations are "injected" between the two registrations in the `Host`'s `Startup.Configure()` method.

This ends up the explanations how a middleware is registered from within an extension.

