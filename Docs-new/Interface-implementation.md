We'll do it the simplest possible way &ndash; the interface would be the well-known `IExtension` and we'll inherit from `ExtensionBase` redefining the `Name`:


```
using CoreXF.Abstractions;

namespace Extension1
{
    public class TheExtension : ExtensionBase
    {
        public override string Name => "Some name";
    }
}
```

That's all &ndash; the simplest possible extension, inheriting from `ExtensionBase` and overriding one of its properties.

The `IExtensionRegistry` is used to retrieve the required extension:

```
public IActionResult Index()
{
    var extension = this.registry.GetExtension<TheExtension>();
    this.ViewBag.ExtName = extension?.Name;
    return this.View();
}
```

## A realistic scenario

Of course, in a more realistic scenario, the interface should be put in a separate assembly and referenced by the host application.  The extension then can be refactored as many times as needed in order to achieve the required functionality.