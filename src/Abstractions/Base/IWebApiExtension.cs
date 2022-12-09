namespace CoreXF.Abstractions.Base
{
    public interface IWebApiExtension : IExtension
    {
        public enum ExtensionStatus { Stopped, Running }

        /// <summary>
        /// The extension status.
        /// </summary>
        ExtensionStatus Status { get; }

        void Start();

        void Stop();
    }
}