## Inttroduction

Two things are necessary to explore the functionality of CoreXF: a host application whose functionality is going to be extended and one or more extensions. The following is going to show you how to do that. 

## CoreXF host application

A .NET Core MVC application can easily be turned into a CoreXF host provided it references CoreXF.Framework package. Few changes have to be made to the Startup class. 

The framework has to be added to the existing services like this:

`
services.AddControllersWithViews().AddCoreXF(services, this.Configuration);
`

In general, any of the extensions that can be used to configure `IServiceCollection` and that returns `IMvcBuilder` would do.

For "using" CoreXF see [Registering middleware from extensions](Registering-middleware-from-extensions.md).

Please, note that it has to be called just before the call to `UseMvc()`.

## CoreXF extension

Extensions come in two main forms:
- plain .NET Core libraries (.DLL), and
- ASP.NET Core MVC applications.

What makes such projects extensions is a class either inheriting from `ExtensionBase`, or implementing the `IExtension` interface. 

Take a look at he interface:

```
public interface IExtension
{
    string Name { get; }
    string Description { get; }
    string Url { get; }
    string Version { get; }
    string Authors { get; }
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    void ConfigureMiddleware(IExtensionsApplicationBuilder app);
}
```

and it's simplest possible implementation:
   
```
public class ExtensionBase : IExtension
{
    public virtual string Name => nameof(ExtensionBase);
    public virtual string Description => "Base extension class, inherit to extend functionality.";
    public virtual string Url => "www.codesolidi.com";
    public virtual string Version => "1.0.0";
    public virtual string Authors => "Code Solidi Ltd.";
    public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration) 
    {
        // source code omitted for brevity
    }
    public virtual void ConfigureMiddleware(IExtensionsApplicationBuilder app)
    {
    }
}
```
An application or library in which only such a class is defined despite being an extension is still of no use &ndash; it has to export some functionality. This functionality may be classified as:
- [implementation of an interface known to both the host and the extension](Interface-implementation.md),
- [one or more controllers with their models and views](MVC-application-as-an-extension.md),
- [one or more view components (`ViewComponent`) with their models and views](Exporting-ViewComponents.md),
- [one or more tag helpers (`TagHelper`)](Extending-with-TagHelpers.md).
 
## Making extensions available to host application

Extensions are added as references directly to the host application. As a matter of fact this is not necessary &ndash; extensions can be put in any dedicated folder, say "Extensions" or "Plugins". This folder must be a subfolder to the main project folder. 
_(NB: This functionality is yet to come!)_