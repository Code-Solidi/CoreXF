## Defining the service

Some extensions might require services from either the host application or another extensions to be injected and thus made available to them.

Let's say the service is an implementation of an interface. The interface must be known to both the service producer (in order to correctly implement it) and the service consumer (so as to know what exactly the service provides). And, of course, the service has to be loaded and registered with the host application.

To be more concrete, let the service provide a Date and Time retrieved from the OS. The interface for this service is:

`public interface IDateTimeService
{
    DateTime Get();
}`

This is it when it comes to defining a service.

## Implementing the service

Service may be implemented either in the host application or in an extension. In either case service definition must be statically linked so as to make it available to the implementation.

The implementation then defines methods and properties and it is registered with the available DI container (`IServiceCollection`). Only after that it becomes available to all interested parties.

In order to achieve this a Setup class must be available and it is much like the usual Setup found in an ASP.NET Core app. Thus the service implementation has access to both the configuration and the built-in DI container.

Nothing more, but nothing less is required when it comes to service implementation.
