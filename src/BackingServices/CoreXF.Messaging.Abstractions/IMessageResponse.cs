namespace CoreXF.Messaging.Abstractions
{
    public interface IMessageResponse
    {
        object Content { get; }

        StatusCode StatusCode { get; }

        IRecipient Recipient { get; }

        string ReasonPhrase { get; }

        //void SetContent(object content);

        //void SetRecipient(IRecipient recipient);
    }

    public enum StatusCode
    {
        Success,
        Failed
    }
}