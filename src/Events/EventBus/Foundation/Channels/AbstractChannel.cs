using EventBus.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace EventBus.Foundation.Channels
{
    public abstract class AbstractChannel : IChannel
    {
        private readonly object locker = new object();

        protected internal IDictionary<string, ICollection<IMessage>> MessageQueue { get; set; }

        protected internal ILogger Logger { get; set; }

        protected AbstractChannel(IChannelFactory factory, ILogger logger)
        {
            this.Logger = logger;
            this.MessageQueue = factory.MessageQueue;
        }

        /// <summary>
        /// Peeks the specified message type.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns></returns>
        public virtual IEnumerable<IMessage> Peek(string messageType)
        {
            lock (this.locker)
            {
                return this.MessageQueue[messageType];
            }
        }

        /// <summary>
        /// Gets the message types.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> MessageTypes => this.MessageQueue.Keys;
    }
}