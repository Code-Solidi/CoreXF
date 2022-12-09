using CoreXF.Abstractions.Base;

using static CoreXF.Abstractions.Base.IWebApiExtension;

namespace CoreXF.Abstractions
{
    public class WebApiExtension : AbstractExtension, IWebApiExtension
    {
        public ExtensionStatus Status { get; private set; } = ExtensionStatus.Running;

        public virtual void Start()
        {
            this.Status = ExtensionStatus.Running;
        }

        public virtual void Stop()
        {
            this.Status = ExtensionStatus.Stopped;
        }
    }
}