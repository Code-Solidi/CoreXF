using CoreXF.Abstractions.Base;

namespace Eventing
{
    public class EventingService : ExtensionBase, IBackingService
    {
        public override string Name => nameof(EventingService);
    }
}