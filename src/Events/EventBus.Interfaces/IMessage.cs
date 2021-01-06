using System;
using System.Threading.Tasks;

namespace EventBus.Interfaces
{
    public interface IMessage
    {
        Guid Id { get; }

        DateTime DateTime { get; }

        string Payload { get;  set; }

        string Type { get; set; }

        T GetPayload<T>();

        Task<T> GetPayloadAsync<T>();
    }
}