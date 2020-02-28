## Introduction

Unlike partial views `ViewComponent`'s are self-contained and may be regarded as stand-alone functional units. Having model, code (think of it as a _controller_), and a view they are very much like any other front-end web component, especially if bundled with the related styles, scripts, and other resources.

## Exporting ViewComponents

As already mentioned, in a project that is to become an extension, there must be a class either implementing `IExtension`, or deriving from `ExtensionBase` class. 

Creating `ViewComponent`'s is described in many places out there but good staring points can be found [here](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/view-components?view=aspnetcore-2.1) and [here](https://visualstudiomagazine.com/articles/2018/02/07/view-components.aspx).

Very much like controllers, `ViewComponent`s have to be decorated with `Export` attribute so as to make them "visible" to a host application:

```
[Export]
public class PriorityListViewComponent : ViewComponent
{
    //...
}
```

If not decorated this way, despite being invoked the view component will be unavailable at run time. A message like the one bellow informs about this:

`A view component named '...' could not be found.`

## Accessing views and other resources
Again, much like views, scripts, styles, as well as resources must be embedded in the final assembly. And yet again, they can embedded like this:

```
<ItemGroup>
    <EmbeddedResource Include="Views\Shared\Components\PriorityList\Default.cshtml" />
    ...
</ItemGroup>
```

_(NB: in future version views may be referenced directly form the compiled views assembly.)_

## A realistic scenario

`ViewComponent`s being _components_ and therefore self-contained pieces of functionality can be shared among projects. This is an easy way to make them reusable, and, as a consequence make them easy to maintain as well as have their value increased. In this respect a view component library is a very realistic outcome when using `ViewComponent`s from extensions.

Services, configuration, and all the other topics relevant to controllers are true for `ViewComponent`s too.

