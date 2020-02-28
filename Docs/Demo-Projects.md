There are few demo projects which demonstrate the use of CoreXF framework. Here they are listed with a very short descriptions.

### Extension1

This is a simple class exported as a library. In fact the class inherits from ExtensionBase so, from a certain point of view, this is the most minimal extension possible.

### Extension2

A .NET Core MVC application defining two controllers: `DefaultControler` and `SkippedController`. The second one is not decorated with `Export` attribute and thus is not accessible in the host application.

### Extension3

A .NET Core MVC application defining a controller - `HomeController` which is not exported and two '`ViewComponent`'s - `MenuItemViewComponent` and `PriorityLisViewComponent`. Both are decorated with the `Export` attribute and are accessible in host application.

### Extension4

A .NET Core MVC appcliation which defines the `EmailTagHelper` (which does not need to be exported).
   
### HostApp 
This is the .NET Core MVC appcliation which hosts all of the above.
