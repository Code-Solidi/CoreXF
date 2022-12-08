# CoreXF

**NB: built with .NET 6.0: demos changed, ExtensionBase simplified (the I in SOLID), documentation is out of date, will be updated ASAP (sorry for the inconvenience), however still usable.**

**Please, begin with the demos. CoreXF.Abstractions and CoreXF.Framework are directly linked to demos, so you'll need the full source to build and run them. (Later on Abstractions and Framework will be publihsed in nuget.org.)**

### Description
CoreXF is a ASP.NET **Core** e**X**tensibility **F**ramework. 

### Getting Started
To get started read the [documentation](https://code-solidi.github.io/CoreXF/).

Packaged as two NuGet packages: 
1. [CoreXF.Abstractions](https://www.nuget.org/packages/CoreXF.Abstractions/), in PMC use the command **Install-Package CoreXF.Abstractions -Version 1.1.2**.
2. [CoreXF.Framework](https://www.nuget.org/packages/CoreXF.Framework/), in PMC use the command **Install-Package CoreXF.Framework -Version 1.1.2**.

### Change in 1.1.2.

Debugging packages' source code is **enabled** in latest release (1.1.2.). Just select Tools -> Options -> Debugging -> Symbols in VC2019 and add https://symbols.nuget.org/download/symbols to Symbol file (.pdb) locations.
Don't forget to **Enable source server support** and **Enable Source Link support** in ... ->  Debugging -> General.

If you like **CoreXF** give it a **star** <g-emoji class="g-emoji" alias="star" fallback-src="https://github.githubassets.com/images/icons/emoji/unicode/2b50.png">⭐</g-emoji>.

**NB:** You may install the framework *only*, it references the abstractions. Abstractions (w/o the Framework) are helpful in creating a plugin.
