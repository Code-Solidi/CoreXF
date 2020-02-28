## Introduction

Another feature not available prior to ASP.NET Core is `TagHelper`s. Many sources exists but you can read about them [here](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro?view=aspnetcore-2.1) and [here](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/built-in/?view=aspnetcore-2.1).  In fact, `ViewComponent`s can be used as `TagHelper`s [too](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/view-components?view=aspnetcore-2.1#invoking-a-view-component-as-a-tag-helper).

## Exporting TagHelpers

No need to do that!  Yes, that's right &ndash; `TagHelper`s are just pieces of code and can be put easily in a class library. 

However, having a `TagHelper` in a context, that is a MVC application, benefits in retaining the initial context as well as a good testing environment for the `TagHelper`.  Provided there's one controller with a single action, having a single view, the overhead of such an environment is not too big. 

This is why, `TagHelper`s acn be turned into extensions and managed as such. 

Needless to way, either `IExtension` must be implemented or `ExtensionBase` must be inherited.

## Realistic scenario

In a future version of CoreXF framework, when an _Extension Manager_ is available, extensions will be turned on and off (made active or inactive), thus changing the application's functionality with just a mouse-click. This is why having `TagHelper`s as extensions is a good path to follow.