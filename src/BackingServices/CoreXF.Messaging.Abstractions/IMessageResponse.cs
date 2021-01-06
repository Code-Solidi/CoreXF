using System.Net.Http;

namespace CoreXF.Messaging.Abstractions
{
    public interface IMessageResponse
    {
        object Content { get; }

        StatusCode StatusCode { get; }

        IRecipient Recipient { get; }

        string ReasonPhrase { get; }

        void SetContent(object content);
    }

    public enum StatusCode
    {
        Success,
        Failed
    }
}