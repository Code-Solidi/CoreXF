using System.Net.Http;

namespace EventBus.Interfaces
{
    public interface IMessageResponse
    {
        byte[] Content { get; }

        HttpResponseMessage Response { get; set; }
    }
}