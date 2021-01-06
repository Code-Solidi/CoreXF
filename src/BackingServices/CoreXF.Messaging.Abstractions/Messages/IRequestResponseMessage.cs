namespace CoreXF.Messaging.Abstractions.Messages
{
    public interface IRequestResponseMessage : IMessage
    {
        /// <summary>
        /// Gets the recipient Identity.
        /// </summary>
        /// <value>
        /// The recipient Identity.
        /// </value>
        string Recipient { get; }
    }
}