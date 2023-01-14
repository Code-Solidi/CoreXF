Another MVC application can be turned into an extension. Again, there must be a class implementing `IExtension` interface. Of course, the class may be derived from `ExtensionBase` and override some (or all) of its properties, instead of implementing the interface.

## Exporting controllers
In order to make a controller "visible" to host application it must be decorated with the `ExportAttribute`. Otherwise, the controller is "internal" to the application and may be used during development and testing but not in the deployed version. 

In the example bellow the DefaultController is accessible by host application:

```
[Export]
public class DefaultController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
```

while `SkippedController` is accessed only internaly by the extension:


```
public class SkippedController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
```

## Routing

Here's how the `Index` action is accessed in the host application:

```
<div class="navbar-collapse collapse">
    <ul class="nav navbar-nav">
        <li><a asp-area="" asp-controller="Home" asp-action="Index">Home</a></li>
        <li><a asp-area="" asp-controller="Home" asp-action="About">About</a></li>
        <li><a asp-area="" asp-controller="Home" asp-action="Contact">Contact</a></li>
        <li><a asp-area="" asp-controller="Default" asp-action="Index">Default</a></li>
    </ul>
</div>
```
There is _no difference_ between the routes to the host application controllers and actions and the extensions' ones. This clearly imposes a restriction on controller names &ndash; they must be different because otherwise the MVC framework would complain that there are ore than one controller capable of serving the request.

## Accessing views and other resources

Views, as well as scripts, styles, and other resources like fonts pictures etc. must be embedded in the final assembly so as to be made accessible by the extension once deployed to host. If, let's say, the Index.cshtml is required by the extension controller it can be embedded like this:

```
<ItemGroup>
  <EmbeddedResource Include="Views\Default\Index.cshtml" />
  <EmbeddedResource Include="wwwroot\css\extension2.css" />
</ItemGroup>
```
This snippet is part of the `.csproj` file.

_(NB: in future version views may be referenced directly form the compiled views assembly.)_
 
## A realistic scenario

In this scenario a MVC application tending to get larger and fatter in order to cover the ever growing requirements can be made simpler, slimmer, and manageable. 

A controller together with its views and models can be "extracted" and put into extension, which, when developed and tested can be "returned back" to the main application. 

What is important to note is that services can be used "as is" if they are the built-in ones, like ILoggerFactory, or replaced with fakes or mocks if they are custom ones. When the extension is loaded by the host application the "real" ones are used instead.

Services, which come from the extension itself _are not visible to the host application_. Extra efforts have to be put so as to make them _available to the extension after deploying it to host_.

_(NB:  If the extension requires some special configuration like, say, a separate connection string, or anything else, it has to be part of the host configuration. In future version this might change.)_