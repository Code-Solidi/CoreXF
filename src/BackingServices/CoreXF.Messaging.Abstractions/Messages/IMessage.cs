using System;
using System.Threading.Tasks;

namespace CoreXF.Messaging.Abstractions.Messages
{
    public interface IMessage
    {
        Guid Id { get; }

        DateTime DateTime { get; }

        string Type { get; set; }

        T GetPayload<T>();

        Task<T> GetPayloadAsync<T>();
    }
}