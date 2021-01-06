using CoreXF.Messaging.Abstractions.Messages;

using System;
using System.Threading.Tasks;

namespace CoreXF.Messaging.Messages
{
    public abstract class AbstractMessage : IMessage
    {
        protected AbstractMessage(string messageType)
        {
            this.Type = string.IsNullOrWhiteSpace(messageType) ? throw new ArgumentException("Message type cannot be null or empty") : messageType;
        }

        protected AbstractMessage(string messageType, object payload)
        {
            this.Type = string.IsNullOrWhiteSpace(messageType) ? throw new ArgumentException("Message type cannot be null or empty") : messageType;
            this.Payload = payload;
        }

        public Guid Id { get; } = Guid.NewGuid();

        public string Type { get; set; }

        public object Payload { get; private set; }

        public DateTime DateTime { get; protected set; } = DateTime.UtcNow;

        public T GetPayload<T>()
        {
            var result = default(T);
            try
            {
                result = (T)this.Payload;
            }
            catch (InvalidCastException)
            {
                result = (T)Convert.ChangeType(this.Payload, typeof(T));
            }

            return result;
        }

        public async Task<T> GetPayloadAsync<T>()
        {
            return await Task.Run(() => this.GetPayload<T>()).ConfigureAwait(false);
        }

        protected void SetPayload(IMessage message)
        {
            this.Payload = ((message as AbstractMessage).Payload);
        }
    }
}