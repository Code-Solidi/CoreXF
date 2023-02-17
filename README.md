# CoreXF

Live demo [here](https://infostore.codesolidi.com).

### Description
CoreXF is a ASP.NET **Core** e**X**tensibility **F**ramework. 
CoreXF is available as two NuGet packages: 
1. [CoreXF.Abstractions](https://www.nuget.org/packages/CoreXF.Abstractions/), in PMC use the command **Install-Package CoreXF.Abstractions -Version 3.0.0**.
2. [CoreXF.Framework](https://www.nuget.org/packages/CoreXF.Framework/), in PMC use the command **Install-Package CoreXF.Framework -Version 3.0.0**.

### What's new (v3.0.0)
- Removed rarely used functions from <code>IExtensioin</code> like <code>ConfigureMiddleware</code>.
- Optimized extension loader.
- Extensions can be deployed either using either Visual Studio functionality, or just copying the extension output to a sub-folder in host's extensions folder.
- Support for .NET 5.0 dropped, added support for .NET 6.0. 
- Static file providers for each extension, which allows prioritization of all static resources: CSS, JavaScript, images, videos, etc.
- Other minor optimizations

### What's new (v2.0.0)
- Added support for .NET 5.0 in addition to .NET Core 3.1

### What's new (v1.1.2)

Debugging packages' source code is **enabled** in latest release (1.1.2.). Just select Tools -> Options -> Debugging -> Symbols in VC2019 and add https://symbols.nuget.org/download/symbols to Symbol file (.pdb) locations.
Don't forget to **Enable source server support** and **Enable Source Link support** in ... ->  Debugging -> General.

### Getting Started

Steps to follow:

- Create a ASP.NET MVC (or Web API) project and add **[CoreXF.Framework](https://www.nuget.org/packages/CoreXF.Framework/)** NuGet package to it. This is the host application.
- Create a folder in he host app (usually "Extensions") which will contain the extensions.
- Create as many extensions as needed. Add **[CoreXF.Abstractions](https://www.nuget.org/packages/CoreXF.Abstractions/)** to every extension and either implement <code>IExtension</code> interface or inherit from <code>MvcExtension</code> class. (The class defines some methods and properties and makes the implementation more convenient.)
- (For those implementing WebAPI extensions this is the <code>WebApiExtension</code> class.)
- Either publish the extension (prefered deployment mechanism for MVC extension, esp. if the extension has custom static files - .css, .js, images, etc.), or just copy the output (bin folder) to the corresponding extension folder. The latest is most commonly used for WebAPI extension unless they need to serve something from their web root (usually wwwroot).
The easiest way to get started is to examine the demos. 

### Documentation
_In preparation_, sorry for the inconvenience. Use demos and live demo site till ready.

### Demos
Appart from the demo apps here on GitHub, there's a dedicated live demo [here](https://infostore.codesolidi.com). Source code for the demo can be found [here](https://github.com/Code-Solidi/InfoStore).

NB: v3.0.0 demos replace the old ones and are now hopefully much more helpful, <code>ExtensionBase</code> simplified (user is no longer forced to use/implement methods thet are not in use anymore).

Also, don't forget to change the locations where MVC demos are published (.Net Core 3.1 and .NET 6) according to the path in your local machine.

### Issues
If you've encountered an issue, please post it [here](https://github.com/Code-Solidi/CoreXF/issues). Thank you!

### Like CoreXF?
If you like **CoreXF** please give it a star <g-emoji class="g-emoji" alias="star" fallback-src="https://github.githubassets.com/images/icons/emoji/unicode/2b50.png">⭐</g-emoji>.


