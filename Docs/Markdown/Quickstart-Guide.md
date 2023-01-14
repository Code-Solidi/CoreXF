## Inttroduction


- [Interface implementation](Interface-implementation.htm)
- [MVC application as an extension](MVC-application-as-an-extension.htm)
- [Exporting ViewComponents](Exporting-ViewComponents.htm)
- [Extending with TagHelpers](Extending-with-TagHelpers.htm)

Two things are necessary in order to use CoreXF: a `host` application whose functionality is going to be extended by one or more `extensions`. 

## CoreXF host application

A .NET Core MVC (or Web API) application can easily be turned into a CoreXF host provided it references CoreXF.Framework package. Few changes have to be made in `Startup` (.net core v3.1), or `Program` (.net v6.0). 

1.&nbsp;The framework has to be added to the existing services like this:

.netcore3.1

`
services.AddControllersWithViews().AddCoreXF(services, this.Configuration);
`

.net 6.0

`
builder.Services.AddControllersWithViews().AddCoreXF(builder.Services);
`

2.&nbsp;There has to be a folder (usually `Extensions`) right under the project root which will hold the extensions. 

3.&nbsp;The framework has to be `used` as well.

.netcore3.1 and .net 6.0

`
app.UseCoreXF();
`

Place the above right before `app.UseEndpoints(...)`.

*The demo applications are for your convenience, look how it's done there.* 


## CoreXF extension

Depending on the requirements extensions ca be:
- plain .NET Core libraries (.DLL), 
- ASP.NET Core MVC applications,
- ASP.NET Web API applications.

What makes such projects extensions is a class implementing `IExtension`, `IMvcExtension`, or `IWebApiExtension` correspondingly. 
it is easier however to inherit from `AbstractExtension`, `MvcExtension`, or `WebApiExtension` instead. 

Take a look at the interface:

```
public interface IExtension
{
    string Name { get; }

    string Description { get; }

    string Url { get; }

    string Version { get; }

    string Authors { get; }

    string Copyright { get; }

    string Location { get; }

    void ConfigureServices(IServiceCollection services, IConfiguration configuration);

    void Configure(Assembly assembly);
}
```

and it's simplest possible implementation:
   
```
public class AbstractExtension : IExtension
{
    public string Name {get; protected set; }

    public string Description { get; protected set; } = "Base extension class, inherit to extend functionality.";

    public string Url { get; protected set; } = "www.codesolidi.com";

    public string Version { get; protected set; } = "1.0.0";

    public string Authors { get; protected set; } = "Code Solidi Ltd.";

    public string Location { get; protected set; }

    public string Copyright { get; protected set; }

    public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration) 
    {
        // source code omitted for brevity
    }

    public virtual void Configure(Assembly assembly)
    {
        // source code omitted for brevity
    }
}
```

In addition the class defines two methods:
```
   public T Get<T>(string name)
```
and
```
    public void Set<T>(string name, T value)
```

which are responsible for setting/getting named values. Later on these values may be inspected in the host application (see the demos for details).

*In the above code all documentation comments are omitted so as to increase readability.*

An application or library defining such a class is an extension. 
It also needs to export some functionality. The functionality may be classified as:

- [implementation of an interface known to both the host and the extension](https://github.com/achristov/CoreXF/wiki/Interface-implementation),<br />
- [one or more controllers with their models and views](https://github.com/achristov/CoreXF/wiki/MVC-application-as-an-extension),<br />
- [one or more view components (`ViewComponent`) with their models and views](https://github.com/achristov/CoreXF/wiki/Exporting-ViewComponents),<br />
- [one or more tag helpers (`TagHelper`)](https://github.com/achristov/CoreXF/wiki/Extending-with-TagHelpers).<br />
 
## Making extensions available to host application

The extensions are deployed to a dedicated folder, say "Extensions" or "Plugins". This folder must be a subfolder to the main project folder. 

Every extension must have its own subfolder so that the host application can discover, load, register, and use them. 
A good practice is to name this folder after extension's name.

The deployment can be done either by copying the extension's compiled output (`bin` folder), 
or by [publishing](https://learn.microsoft.com/en-us/visualstudio/deployment/deploying-applications-services-and-components-resources?view=vs-2022) it from Visual Studio. 

In the second case static files (CSS, Javascript, fonts, images, videos, etc.) are copied too and the extension is considered *self-contained*. 


(*The web root folder has to retain its original name&mdash;`wwwroot`. In later versions this may change in order to reflect author's preferences.*)


*For more information look at the demo projects.*
