using EventBus.Interfaces;
using System;
using System.Threading.Tasks;

namespace EventBus.Foundation.Messages
{
    public abstract class AbstractMessage : IMessage
    {
        public Guid Id { get; } = Guid.NewGuid();

        public string Type { get; set; }

        public virtual string Payload { get; set; }

        public DateTime DateTime { get; protected set; } = DateTime.UtcNow;

        public T GetPayload<T>()
        {
            var result = Convert.ChangeType(this.Payload, typeof(T));
            return (T)result;
        }

        public async Task<T> GetPayloadAsync<T>()
        {
            var result = await Task.Run(() => Convert.ChangeType(this.Payload, typeof(T)));
            return (T)result;
        }
    }
}